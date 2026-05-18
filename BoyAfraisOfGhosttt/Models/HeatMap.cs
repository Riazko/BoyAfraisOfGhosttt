using System;
using System.Collections.Generic;
using BoyAfraidOfGhosts.Helpers;

namespace BoyAfraidOfGhosts.Models
{
    public class HeatMap
    {
        private readonly PathfindingGraph graph;
        private readonly double[,] playerDistance;
        private readonly double[,] ghostPressure;
        public int Width => PathfindingGraph.GridWidth;
        public int Height => PathfindingGraph.GridHeight;
        private const double Infinity = 999999;
        private int lastGhostCount = -1;
        private Vector2D lastPlayerPosition;
        private bool needsFullUpdate = true;

        public HeatMap(PathfindingGraph graph)
        {
            this.graph = graph;
            playerDistance = new double[Width, Height];
            ghostPressure = new double[Width, Height];
            lastPlayerPosition = new Vector2D(-9999, -9999);
        }

        public void Update(Vector2D playerPosition, IEnumerable<GhostModel> ghosts)
        {
            if (needsFullUpdate || playerPosition.DistanceTo(lastPlayerPosition) > 50)
            {
                BuildPlayerDistanceMap(playerPosition);
                lastPlayerPosition = playerPosition;
            }
            var currentGhostCount = 0;
            foreach (var ghost in ghosts)
            {
                if (ghost.IsAlive)
                    currentGhostCount++;
            }
            if (needsFullUpdate || currentGhostCount != lastGhostCount)
            {
                BuildGhostPressureMap(ghosts);
                lastGhostCount = currentGhostCount;
            }
            needsFullUpdate = false;
        }

        private void BuildPlayerDistanceMap(Vector2D playerPosition)
        {
            var playerCell = graph.WorldToGrid(playerPosition);
            var queue = new Queue<(int x, int y)>();
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                    playerDistance[x, y] = Infinity;
            }

            playerDistance[playerCell.x, playerCell.y] = 0;
            queue.Enqueue((playerCell.x, playerCell.y));
            while (queue.Count > 0)
            {
                var (cx, cy) = queue.Dequeue();
                var currentDist = playerDistance[cx, cy];
                foreach (var (nx, ny) in graph.GetNeighbors(cx, cy))
                {
                    if (graph.IsWallCell(nx, ny))
                        continue;

                    if (playerDistance[nx, ny] > currentDist + 1)
                    {
                        playerDistance[nx, ny] = currentDist + 1;
                        queue.Enqueue((nx, ny));
                    }
                }
            }
        }

        private void BuildGhostPressureMap(IEnumerable<GhostModel> ghosts)
        {
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                    ghostPressure[x, y] = 0;
            }
            var radius = Settings.HeatMapGhostInfluenceRadius;
            foreach (var ghost in ghosts)
            {
                if (!ghost.IsAlive)
                    continue;
                var ghostCell = graph.WorldToGrid(ghost.Position);
                var minX = Math.Max(0, ghostCell.x - radius);
                var maxX = Math.Min(Width - 1, ghostCell.x + radius);
                var minY = Math.Max(0, ghostCell.y - radius);
                var maxY = Math.Min(Height - 1, ghostCell.y + radius);
                for (var x = minX; x <= maxX; x++)
                {
                    for (var y = minY; y <= maxY; y++)
                    {
                        if (graph.IsWallCell(x, y))
                            continue;
                        var distance = Math.Abs(x - ghostCell.x) + Math.Abs(y - ghostCell.y);
                        if (distance <= radius)
                        {
                            var pressure = Settings.HeatMapGhostPenalty / (distance + 1);
                            ghostPressure[x, y] += pressure;
                        }
                    }
                }
            }
        }

        public double GetGhostPressure(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return Infinity;
            return ghostPressure[x, y];
        }

        public double GetPlayerDistance(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return Infinity;
            return playerDistance[x, y];
        }

        public void Invalidate()
        {
            needsFullUpdate = true;
        }
    }
}
