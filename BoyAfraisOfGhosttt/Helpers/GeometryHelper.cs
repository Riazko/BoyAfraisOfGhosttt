using System;

namespace BoyAfraidOfGhosts.Helpers
{
    public static class GeometryHelper
    {
        // Проверяет, находится ли точка внутри конуса (для вспышки)
        public static bool IsPointInCone(Vector2D origin, Vector2D direction, 
            double halfAngleDegrees, Vector2D point, double maxRadius)
        {
            var toPoint = point.Subtract(origin);
            var distance = toPoint.Length();

            var norm = toPoint.Normalize();
            var dotProduct = direction.X * norm.X + direction.Y * norm.Y;
            dotProduct = Math.Max(-1, Math.Min(1, dotProduct));

            var angleDegrees = Math.Acos(dotProduct) * 180 / Math.PI;
            return angleDegrees <= halfAngleDegrees;
        }
    }
}