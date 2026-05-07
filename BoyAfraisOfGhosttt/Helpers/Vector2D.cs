using System;

namespace BoyAfraidOfGhosts.Helpers
{
    public struct Vector2D
    {
        public double X;
        public double Y;

        public Vector2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Vector2D Add(Vector2D other)
        {
            return new Vector2D(X + other.X, Y + other.Y);
        }

        public Vector2D Subtract(Vector2D other)
        {
            return new Vector2D(X - other.X, Y - other.Y);
        }

        public Vector2D Multiply(double scalar)
        {
            return new Vector2D(X * scalar, Y * scalar);
        }

        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        public Vector2D Normalize()
        {
            var len = Length();
            if (len == 0) 
                return new Vector2D(0, 0);
            return new Vector2D(X / len, Y / len);
        }

        public double DistanceTo(Vector2D other)
        {
            var dx = X - other.X;
            var dy = Y - other.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static Vector2D operator +(Vector2D a, Vector2D b)
        {
            return new Vector2D(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2D operator -(Vector2D a, Vector2D b)
        {
            return new Vector2D(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2D operator *(Vector2D a, double scalar)
        {
            return new Vector2D(a.X * scalar, a.Y * scalar);
        }

        public static Vector2D operator *(double scalar, Vector2D a)
        {
            return new Vector2D(a.X * scalar, a.Y * scalar);
        }

        public override string ToString()
        {
            return $"({X:F2}, {Y:F2})";
        }
    }
}