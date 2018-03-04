using System;

namespace VectorMath
{
    public static class MathHelper
    {
        public const double DegreesToRadians = 0.0174533;
        public const double RadiansToDegrees = 57.2958;

        public static float Lerp(float from, float to, float t)
        {
            return from + t * (to - from);
        }

        public static double Lerp(double from, double to, double t)
        {
            return from + t * (to - from);
        }

        public static double LerpAngle(double from, double to, double t)
        {
            double difference = Math.Abs(to - from);

            if (difference > Math.PI)
            {
                if (to > from)
                {
                    from += Math.PI * 2;
                }
                else
                {
                    to += Math.PI * 2;
                }
            }

            return from + (to - from) * t;
        }

        public static int Clamp(int val, int min, int max)
        {
            if (val < min) return min;

            if (val > max) return max;

            return val;
        }

        public static float Clamp(float val, float min, float max)
        {
            if (val < min) return min;

            if (val > max) return max;

            return val;
        }

        public static double Clamp(double val, double min, double max)
        {
            if (val < min) return min;

            if (val > max) return max;

            return val;
        }
    }
}
