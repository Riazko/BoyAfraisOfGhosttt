using System;
using BoyAfraidOfGhosts.Helpers;

namespace BoyAfraidOfGhosts.Models
{
    public class WorldMap
    {
        public int WidthInCells { get; private set; }
        public int HeightInCells { get; private set; }
        public int CellSize { get; private set; }
        private readonly WorldTileType[,] tiles;
        private readonly Random random;

        public WorldMap(int widthInCells, int heightInCells, int cellSize)
        {
            WidthInCells = widthInCells;
            HeightInCells = heightInCells;
            CellSize = cellSize;
            tiles = new WorldTileType[WidthInCells, HeightInCells];
            random = new Random();
            Generate();
        }

        private void Generate()
        {
            FillFloor();
            GenerateWallClusters();
            ClearStartArea();
        }

        private void FillFloor()
        {
            for (var x = 0; x < WidthInCells; x++)
            {
                for (var y = 0; y < HeightInCells; y++)
                    tiles[x, y] = WorldTileType.Floor;
            }
        }

        private void GenerateWallClusters()
        {
            for (var i = 0; i < Settings.WallClusterCount; i++)
            {
                var startX = random.Next(2, WidthInCells - 2);
                var startY = random.Next(2, HeightInCells - 2);
                var length = random.Next(Settings.WallMinLength, Settings.WallMaxLength + 1);
                var horizontal = random.Next(2) == 0;
                for (var j = 0; j < length; j++)
                {
                    var x = horizontal ? startX + j : startX;
                    var y = horizontal ? startY : startY + j;
                    if (x < 0 || x >= WidthInCells || y < 0 || y >= HeightInCells)
                        continue;

                    if (IsNearStartArea(x, y))
                        continue;
                    tiles[x, y] = WorldTileType.Wall;
                }
            }
        }

        private void ClearStartArea()
        {
            var centerX = WidthInCells / 2;
            var centerY = HeightInCells / 2;
            for (var x = centerX - Settings.StartSafeZoneCells; x <= centerX + Settings.StartSafeZoneCells; x++)
            {
                for (var y = centerY - Settings.StartSafeZoneCells; y <= centerY + Settings.StartSafeZoneCells; y++)
                {
                    if (x >= 0 && x < WidthInCells && y >= 0 && y < HeightInCells)
                        tiles[x, y] = WorldTileType.Floor;
                }
            }
        }

        private bool IsNearStartArea(int x, int y)
        {
            var centerX = WidthInCells / 2;
            var centerY = HeightInCells / 2;
            return Math.Abs(x - centerX) <= Settings.StartSafeZoneCells &&
                   Math.Abs(y - centerY) <= Settings.StartSafeZoneCells;
        }

        public bool IsInsideCellBounds(int x, int y)
        {
            return x >= 0 && x < WidthInCells && y >= 0 && y < HeightInCells;
        }

        public bool IsInsideWorld(Vector2D worldPosition)
        {
            var halfWidth = Settings.WorldWidth / 2;
            var halfHeight = Settings.WorldHeight / 2;
            return worldPosition.X >= -halfWidth && worldPosition.X <= halfWidth &&
                   worldPosition.Y >= -halfHeight && worldPosition.Y <= halfHeight;
        }

        public WorldTileType GetTile(int x, int y)
        {
            if (!IsInsideCellBounds(x, y))
                return WorldTileType.Wall;
            return tiles[x, y];
        }

        public bool IsWall(int x, int y)
        {
            return GetTile(x, y) == WorldTileType.Wall;
        }

        public Vector2D CellToWorldCenter(int x, int y)
        {
            var worldX = x * CellSize - Settings.WorldWidth / 2 + CellSize / 2;
            var worldY = y * CellSize - Settings.WorldHeight / 2 + CellSize / 2;
            return new Vector2D(worldX, worldY);
        }

        public (int x, int y) WorldToCell(Vector2D worldPosition)
        {
            var x = (int)((worldPosition.X + Settings.WorldWidth / 2) / CellSize);
            var y = (int)((worldPosition.Y + Settings.WorldHeight / 2) / CellSize);
            x = Math.Max(0, Math.Min(WidthInCells - 1, x));
            y = Math.Max(0, Math.Min(HeightInCells - 1, y));
            return (x, y);
        }

        public bool IsWallAtWorldPosition(Vector2D worldPosition)
        {
            var cell = WorldToCell(worldPosition);
            return IsWall(cell.x, cell.y);
        }

        public bool IsCircleBlocked(Vector2D center, double radius)
        {
            if (!Settings.CyclicWorld && !IsCircleInsideWorld(center, radius))
                return true;

            var topLeft = WorldToCell(new Vector2D(center.X - radius, center.Y - radius));
            var bottomRight = WorldToCell(new Vector2D(center.X + radius, center.Y + radius));
            for (var x = topLeft.x; x <= bottomRight.x; x++)
            {
                for (var y = topLeft.y; y <= bottomRight.y; y++)
                {
                    if (IsWall(x, y))
                        return true;
                }
            }
            return false;
        }

        private bool IsCircleInsideWorld(Vector2D center, double radius)
        {
            var halfWidth = Settings.WorldWidth / 2;
            var halfHeight = Settings.WorldHeight / 2;
            return center.X - radius >= -halfWidth && center.X + radius <= halfWidth &&
                   center.Y - radius >= -halfHeight && center.Y + radius <= halfHeight;
        }
    }
}
