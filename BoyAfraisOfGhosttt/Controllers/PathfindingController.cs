using System;
using System.Collections.Generic;
using System.Linq;
using BoyAfraidOfGhosts.Models;
using BoyAfraidOfGhosts.Helpers;

namespace BoyAfraidOfGhosts.Controllers
{
    public class PathfindingController
    {
        private readonly PathfindingGraph graph;
        private readonly GameModel gameModel;
        private readonly HeatMap heatMap;
        private readonly Dictionary<(int fromX, int fromY, int toX, int toY), List<Vector2D>> pathCache;
        private const int MaxCacheSize = 200;
        private int heatMapUpdateCounter = 0;
        private const int HeatMapUpdateInterval = 5;

        public PathfindingController(GameModel gameModel)
        {
            this.gameModel = gameModel;
            graph = new PathfindingGraph(gameModel);
            heatMap = new HeatMap(graph);
            pathCache = new Dictionary<(int, int, int, int), List<Vector2D>>();
        }

        public List<Vector2D> FindPath(Vector2D startWorld, Vector2D targetWorld, GhostModel movingGhost)
        {
            var (startX, startY) = graph.WorldToGrid(startWorld);
            var (targetX, targetY) = graph.WorldToGrid(targetWorld);
            if (!graph.IsWalkable(targetX, targetY, movingGhost))
                return new List<Vector2D>();

            var cacheKey = (startX, startY, targetX, targetY);
            lock (pathCache)
            {
                if (pathCache.TryGetValue(cacheKey, out var cachedPath))
                    return cachedPath.ToList();
            }

            var path = FindPathInternal(startX, startY, targetX, targetY, movingGhost);
            lock (pathCache)
            {
                if (pathCache.Count >= MaxCacheSize)
                {
                    var toRemove = pathCache.Take(MaxCacheSize / 2).ToList();
                    foreach (var key in toRemove)
                        pathCache.Remove(key.Key);
                }
                pathCache[cacheKey] = path.ToList();
            }
            return path;
        }

        private List<Vector2D> FindPathInternal(int startX, int startY, int targetX, int targetY, GhostModel movingGhost)
        {
            var openList = new List<PathfindingNode>();
            var closedSet = new HashSet<(int x, int y)>();
            var startNode = new PathfindingNode(startX, startY)
            {
                GCost = 0,
                HCost = graph.GetHeuristic(startX, startY, targetX, targetY)
            };
            openList.Add(startNode);

            while (openList.Count > 0)
            {
                var bestIndex = 0;
                var bestFCost = openList[0].FCost;
                var bestHCost = openList[0].HCost;

                for (var i = 1; i < openList.Count; i++)
                {
                    var fCost = openList[i].FCost;
                    if (fCost < bestFCost || (Math.Abs(fCost - bestFCost) < 0.0001 && openList[i].HCost < bestHCost))
                    {
                        bestIndex = i;
                        bestFCost = fCost;
                        bestHCost = openList[i].HCost;
                    }
                }
                var currentNode = openList[bestIndex];
                if (bestIndex != openList.Count - 1)
                    openList[bestIndex] = openList[openList.Count - 1];
                openList.RemoveAt(openList.Count - 1);

                closedSet.Add((currentNode.GridX, currentNode.GridY));
                if (currentNode.GridX == targetX && currentNode.GridY == targetY)
                    return ReconstructPath(currentNode);

                var neighbors = graph.GetWalkableNeighbors(currentNode.GridX, currentNode.GridY, movingGhost);
                foreach (var (nx, ny) in neighbors)
                {
                    if (closedSet.Contains((nx, ny)))
                        continue;

                    var heatPenalty = heatMap.GetGhostPressure(nx, ny) * Settings.HeatMapPathWeight;
                    var newGCost = currentNode.GCost + 1 + heatPenalty;
                    PathfindingNode neighborNode = null;

                    for (var i = 0; i < openList.Count; i++)
                    {
                        if (openList[i].GridX == nx && openList[i].GridY == ny)
                        {
                            neighborNode = openList[i];
                            break;
                        }
                    }

                    if (neighborNode == null)
                    {
                        neighborNode = new PathfindingNode(nx, ny)
                        {
                            GCost = newGCost,
                            HCost = graph.GetHeuristic(nx, ny, targetX, targetY),
                            Parent = currentNode
                        };
                        openList.Add(neighborNode);
                    }
                    else if (newGCost < neighborNode.GCost)
                    {
                        neighborNode.GCost = newGCost;
                        neighborNode.Parent = currentNode;
                    }
                }
            }
            return new List<Vector2D>();
        }

        private List<Vector2D> ReconstructPath(PathfindingNode endNode)
        {
            var pathStack = new Stack<Vector2D>();
            var current = endNode;

            while (current != null)
            {
                pathStack.Push(graph.GridToWorld(current.GridX, current.GridY));
                current = current.Parent;
            }
            return pathStack.ToList();
        }

        public void UpdateHeatMap()
        {
            heatMapUpdateCounter++;
            if (heatMapUpdateCounter < HeatMapUpdateInterval)
                return;

            heatMapUpdateCounter = 0;
            lock (gameModel.Ghosts)
            {
                heatMap.Update(gameModel.Player.Position, gameModel.Ghosts);
            }
        }

        public Vector2D GetBestSurroundTarget(GhostModel movingGhost)
        {
            var playerPosition = gameModel.Player.Position;
            var playerCell = graph.WorldToGrid(playerPosition);
            var ghostCell = graph.WorldToGrid(movingGhost.Position);
            var candidates = new List<(int x, int y, double score)>();
            var minDist = Settings.SurroundMinDistanceCells;
            var maxDist = Settings.SurroundMaxDistanceCells;

            for (var dx = -maxDist; dx <= maxDist; dx++)
            {
                for (var dy = -maxDist; dy <= maxDist; dy++)
                {
                    var distanceFromPlayer = Math.Abs(dx) + Math.Abs(dy);
                    if (distanceFromPlayer < minDist || distanceFromPlayer > maxDist)
                        continue;

                    var x = playerCell.x + dx;
                    var y = playerCell.y + dy;
                    if (!graph.IsWalkable(x, y, movingGhost))
                        continue;

                    var pressure = heatMap.GetGhostPressure(x, y);
                    var distanceFromGhost = graph.GetHeuristic(ghostCell.x, ghostCell.y, x, y);
                    var score = pressure * Settings.SurroundPressureWeight +
                                   distanceFromGhost * Settings.SurroundDistanceWeight;
                    candidates.Add((x, y, score));
                }
            }
            if (candidates.Count == 0)
                return playerPosition;

            var best = candidates[0];
            for (int i = 1; i < candidates.Count; i++)
            {
                if (candidates[i].score < best.score)
                    best = candidates[i];
            }
            return graph.GridToWorld(best.x, best.y);
        }

        public void ClearCache()
        {
            lock (pathCache)
            {
                pathCache.Clear();
            }
        }
    }
}
