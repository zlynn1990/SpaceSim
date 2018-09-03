using OpenCLWrapper;

namespace SpaceSim.Kernels
{
    abstract class BaseKernel : CLSourceBase
    {
        #region CONSTANTS START

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

        public const double CALLISTO_RADIUS = 2.4103e6;
        public const double EUROPA_RADIUS = 1.5608e6;
        public const double GANYMEDE_RADIUS = 2.6341e6;
        public const double IO_RADIUS = 1.8213e6;

        public const double SATURN_RADIUS = 6.0268e7;
        public const double SATURN_ATMOSPHERE = 5.95e5;

        public const double SATURN_RING_START = 6.63e6;
        public const double SATURN_RING_END = 1.207e8;

        #endregion CONSTANTS END

        #region FUNCTIONS START

        // Computes the camera rotation based on the camera position and world position
        protected void ComputeCameraRotation(int index, int resX, int resY, double cX, double cY, double cWidth, double cHeight, double cRot, double bodyX, double bodyY, out double diffX, out double diffY)
        {
            // screen-space coords
            int y = index / resX;
            int x = index - (y * resX);

            // screen-space uvmap
            float u = (float)x / resX;
            float v = (float)y / resY;

            // non-rotated world-space camera center
            double camCenterX = cX + cWidth * 0.5;
            double camCenterY = cY + cHeight * 0.5;

            // non-rotated world-space camera position
            double worldX = cX + cWidth * u;
            double worldY = cY + cHeight * v;

            // rotate the camera about the point
            double pivotX = worldX - camCenterX;
            double pivotY = worldY - camCenterY;

            double rotatedX = pivotX * cos(cRot) - pivotY * sin(cRot);
            double rotatedY = pivotX * sin(cRot) + pivotY * cos(cRot);

            worldX = rotatedX + camCenterX;
            worldY = rotatedY + camCenterY;

            // find the distance between the rotated camera position and the given body position
            diffX = bodyX - worldX;
            diffY = bodyY - worldY;
        }
        
        // Computes the sun shading factor based on sun lighting angle
        protected void ComputeSunShading(double diffX, double diffY, double distance, double sunNormalX, double sunNormalY, out double worldNormalX, out double worldNormalY, out double sunDotProduct)
        {
            worldNormalX = -diffX / distance;
            worldNormalY = -diffY / distance;

            sunDotProduct = sunNormalX * worldNormalX + sunNormalY * worldNormalY;

            if (sunDotProduct < 0)
            {
                sunDotProduct = 0.1f;
            }
            else
            {
                sunDotProduct = 0.1f + sunDotProduct * 0.9f;
            }
        }

        #endregion FUNCTIONS END
    }
}
