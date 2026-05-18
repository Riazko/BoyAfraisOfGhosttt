using BoyAfraidOfGhosts.Helpers;

namespace BoyAfraidOfGhosts.Models
{
    public static class WorldGenerator
    {
        public static Vector2D WrapPosition(Vector2D position)
        {
            var halfWidth = Settings.WorldWidth / 2;
            var halfHeight = Settings.WorldHeight / 2;

            while (position.X < -halfWidth)
                position.X += Settings.WorldWidth;

            while (position.X > halfWidth)
                position.X -= Settings.WorldWidth;

            while (position.Y < -halfHeight)
                position.Y += Settings.WorldHeight;

            while (position.Y > halfHeight)
                position.Y -= Settings.WorldHeight;
            return position;
        }
    }
}
