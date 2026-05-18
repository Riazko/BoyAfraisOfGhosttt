using System.Collections.Generic;
using BoyAfraidOfGhosts.Helpers;

namespace BoyAfraidOfGhosts.Models
{
    public class PathfindingGraph
    {
        public const int GridWidth = Settings.MapGridWidth;
        public const int GridHeight = Settings.MapGridHeight;
        public const double CellSize = Settings.TileSize;
        private readonly GameModel gameModel;

        public PathfindingGraph(GameModel gameModel)
        {
            this.gameModel = gameModel;
        }

        public (int x, int y) WorldToGrid(Vector2D worldPos)
        {
            var x = (int)((worldPos.X + Settings.WorldWidth / 2) / CellSize);
            var y = (int)((worldPos.Y + Settings.WorldHeight / 2) / CellSize);
            x = System.Math.Max(0, System.Math.Min(GridWidth - 1, x));
            y = System.Math.Max(0, System.Math.Min(GridHeight - 1, y));
            return (x, y);
        }

        public Vector2D GridToWorld(int x, int y)
        {
            var worldX = x * CellSize - Settings.WorldWidth / 2 + CellSize / 2;
            var worldY = y * CellSize - Settings.WorldHeight / 2 + CellSize / 2;
            return new Vector2D(worldX, worldY);
        }

        public bool IsInsideGrid(int x, int y)
        {
            return x >= 0 && x < GridWidth && y >= 0 && y < GridHeight;
        }

        public bool IsWallCell(int x, int y)
        {
            if (!IsInsideGrid(x, y))
                return true;
            return gameModel.WorldMap.IsWall(x, y);
        }

        public bool IsWalkable(int x, int y, GhostModel currentGhost)
        {
            return IsInsideGrid(x, y) && !IsWallCell(x, y);
        }

        public IEnumerable<(int x, int y)> GetNeighbors(int x, int y)
        {
            if (y > 0) yield return (x, y - 1);
            if (y < GridHeight - 1) yield return (x, y + 1);
            if (x > 0) yield return (x - 1, y);
            if (x < GridWidth - 1) yield return (x + 1, y);
        }

        public List<(int x, int y)> GetWalkableNeighbors(int x, int y, GhostModel currentGhost)
        {
            var result = new List<(int x, int y)>();
            foreach (var neighbor in GetNeighbors(x, y))
            {
                if (IsWalkable(neighbor.x, neighbor.y, currentGhost))
                    result.Add(neighbor);
            }
            return result;
        }

        public double GetHeuristic(int x1, int y1, int x2, int y2)
        {
            return System.Math.Abs(x1 - x2) + System.Math.Abs(y1 - y2);
        }
    }
}
