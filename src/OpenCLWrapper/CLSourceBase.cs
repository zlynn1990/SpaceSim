using System;

namespace OpenCLWrapper
{
    public abstract class CLSourceBase : ICLSource
    {
        private static int CurrentId;

        public int get_global_id(int val)
        {
            return CurrentId++;
        }

        public void Finish()
        {
            CurrentId = 0;
        }

        public int min(int val1, int val2)
        {
            return Math.Min(val1, val2);
        }

        public float min(float val1, float val2)
        {
            return Math.Min(val1, val2);
        }

        public int max(int val1, int val2)
        {
            return Math.Max(val1, val2);
        }

        public float max(float val1, float val2)
        {
            return Math.Max(val1, val2);
        }

        public float sqrt(float val)
        {
            return (float)Math.Sqrt(val);
        }

        public double sqrt(double val)
        {
            return Math.Sqrt(val);
        }

        public float atan2(float x, float y)
        {
            return (float)Math.Atan2(y, x);
        }

        public double atan2(double x, double y)
        {
            return Math.Atan2(y, x);
        }

        public float sin(float val)
        {
            return (float)Math.Sin(val);
        }

        public double sin(double val)
        {
            return Math.Sin(val);
        }

        public double cos(double val)
        {
            return Math.Cos(val);
        }

        public double tan(double val)
        {
            return Math.Tan(val);
        }
    }
}
