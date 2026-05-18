using System;

namespace BoyAfraidOfGhosts.Helpers
{
    public static class GeometryHelper
    {
        public static double Dot(Vector2D a, Vector2D b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        public static double Clamp(double value, double min, double max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        public static double ToDegrees(double radians)
        {
            return radians * 180 / Math.PI;
        }

        public static double AngleBetweenDegrees(Vector2D a, Vector2D b)
        {
            var first = a.Normalize();
            var second = b.Normalize();
            var dot = Dot(first, second);
            dot = Clamp(dot, -1, 1);
            return ToDegrees(Math.Acos(dot));
        }

        public static bool IsPointInsideRadius(
            Vector2D origin,
            Vector2D point,
            double radius)
        {
            return origin.DistanceTo(point) <= radius;
        }

        public static bool IsPointInCone(
            Vector2D origin,
            Vector2D direction,
            double halfAngleDegrees,
            Vector2D point,
            double maxRadius)
        {
            var toPoint = point.Subtract(origin);
            var distance = toPoint.Length();

            if (distance > maxRadius)
                return false;

            if (distance < 0.01)
                return true;

            var angleDegrees = AngleBetweenDegrees(direction, toPoint);
            return angleDegrees <= halfAngleDegrees;
        }
    }
}