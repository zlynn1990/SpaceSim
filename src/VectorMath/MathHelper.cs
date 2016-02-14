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
