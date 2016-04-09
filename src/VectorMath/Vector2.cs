using System;

namespace VectorMath
{
    public class Vector2
    {
        public static Vector2 Zero { get { return new Vector2(0, 0); } }

        public float X { get; set; }
        public float Y { get; set; }

        public Vector2() { }

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float Dot(Vector2 other)
        {
            return X * other.X + Y * other.Y;
        }

        public float Cross(Vector2 v)
        {
            return X * v.Y - Y * v.X;
        }

        public float Distance(Vector2 other)
        {
            float diffX = other.X - X;
            float diffY = other.Y - Y;

            return (float)Math.Sqrt(diffX * diffX + diffY * diffY);
        }

        public float Length()
        {
            return (float)Math.Sqrt(X * X + Y * Y);
        }

        public float LengthSquared()
        {
            return X * X + Y * Y;
        }

        public void Mirror()
        {
            X = -X;
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2 operator *(Vector2 a, float b)
        {
            return new Vector2(a.X * b, a.Y * b);
        }

        public static Vector2 Lerp(Vector2 from, Vector2 to, float t)
        {
            return new Vector2(from.X + t * (to.X - from.X),
                               from.Y + t * (to.Y - from.Y));
        }

        public override string ToString()
        {
            return "[" + X + ", " + Y + "]";
        }
    }
}
