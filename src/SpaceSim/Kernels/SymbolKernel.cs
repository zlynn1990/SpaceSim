using OpenCLWrapper;

namespace SpaceSim.Kernels
{
    abstract class SymbolKernel : CLSourceBase
    {
        #region SYMBOL START

        internal const int ALPHA = 2130706432;
        internal const int GREEN = 16711680;
        internal const int RED = 65280;
        internal const int BLACK = 255;

        public const double SUN_RADIUS = 6.96342e8;
        public const double SUN_ATMOSPHERE = 6.96342e8;

        public const double MERCURY_RADIUS = 6.0518e6;

        public const double VENUS_RADIUS = 6.059e6;
        public const double VENUS_ATMOSPHERE = 1.5e5;

        public const double EARTH_RADIUS = 6.371e6;
        public const double EARTH_ATMOSPHERE = 1.5e5;

        public const double MOON_RADIUS = 1.738e6;

        public const double MARS_RADIUS = 3.3962e6;
        public const double MARS_ATMOSPHERE = 1e5;

        public const double JUPITER_RADIUS = 7.1492e7;
        public const double JUPITER_ATMOSPHERE = 3.970588e5;

        public const double EUROPA_RADIUS = 1.5608e6;

        public const double SATURN_RADIUS = 6.0268e7;
        public const double SATURN_ATMOSPHERE = 5.95e5;

        public const double SATURN_RING_START = 6.63e6;
        public const double SATURN_RING_END = 1.207e8;

        #endregion SYMBOL END
    }
}
