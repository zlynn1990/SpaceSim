using System;

namespace VectorMath
{
    [Serializable]
    public class DVector2
    {
        public static DVector2 Zero { get { return new DVector2(0, 0); } }

        public double X;
        public double Y;

        public DVector2() { }

        public DVector2(double x, double y)
        {
            X = x;
            Y = y;
        }

        public void Reset()
        {
            X = 0;
            Y = 0;
        }

        public void Normalize()
        {
            double length = Length();

            X /= length;
            Y /= length;
        }

        public DVector2 Normalized()
        {
            return Clone() / Length(); 
        }

        public void Negate()
        {
            X = -X;
            Y = -Y;
        }

        public DVector2 Clone()
        {
            return new DVector2(X, Y);
        }

        public static DVector2 operator +(DVector2 a, DVector2 b)
        {
            return new DVector2(a.X + b.X, a.Y + b.Y);
        }

        public static DVector2 operator -(DVector2 a, DVector2 b)
        {
            return new DVector2(a.X - b.X, a.Y - b.Y);
        }

        public static DVector2 operator *(DVector2 a, double b)
        {
            return new DVector2(a.X * b, a.Y * b);
        }

        public static DVector2 operator /(DVector2 a, DVector2 b)
        {
            return new DVector2(a.X / b.X, a.Y / b.Y);
        }

        public static DVector2 operator /(DVector2 a, double b)
        {
            return new DVector2(a.X / b, a.Y / b);
        }

        public void Accumulate(DVector2 other)
        {
            X += other.X;
            Y += other.Y;
        }

        public DVector2 Divide(float scalar)
        {
            return new DVector2(X / scalar, Y / scalar);
        }

        public DVector2 Divide(double scalar)
        {
            return new DVector2(X / scalar, Y / scalar);
        }

        public double Dot(DVector2 v)
        {
            return X * v.X + Y * v.Y;
        }

        public double Cross(DVector2 v)
        {
            return X * v.Y - Y * v.X;
        }

        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        public double LengthSquared()
        {
            return X * X + Y * Y;
        }

        public double Angle()
        {
            return Math.Atan2(Y, X);
        }

        public Vector2 ToVector2()
        {
            return new Vector2((float)X, (float)Y);
        }

        public static DVector2 Lerp(DVector2 from, DVector2 to, double t)
        {
            return new DVector2(from.X + t * (to.X - from.X),
                               from.Y + t * (to.Y - from.Y));
        }

        public static DVector2 FromAngle(double angle)
        {
            return new DVector2(Math.Cos(angle), Math.Sin(angle));
        }

        public static double Distance(DVector2 v1, DVector2 v2)
        {
            return (v2 - v1).Length();
        }

        public override string ToString()
        {
            return "{" + Math.Round(X, 5) + "," + Math.Round(Y, 5) + "}";
        }
    }
}
