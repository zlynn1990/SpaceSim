using SpaceSim.SolarSystem;

namespace SpaceSim.Kernels
{
    class JupiterKernel : SymbolKernel, IMassiveKernel
    {
        // -------- ------ Image ----- --------
        // 01111111 00000000 00000000 00000000 alpha >> 24
        // 00000000 11111111 00000000 00000000 red >> 16
        // 00000000 00000000 11111111 00000000 green >> 8
        // 00000000 00000000 00000000 11111111 blue
        public void Run(int[] image, int resX, int resY, double cameraLeft, double cameraTop, double cameraWidth, double cameraHeight, double sunNormalX, double sunNormalY, double rotation)
        {
            int index = get_global_id(0);

            // screen-space coords
            int y = index / resX;
            int x = index - (y * resX);

            // screen-space uvmap
            float u = (float)x / resX;
            float v = (float)y / resY;

            // world-space pixel location
            double worldX = cameraLeft + cameraWidth * u;
            double worldY = cameraTop + cameraHeight * v;
            double distance = sqrt(worldX * worldX + worldY * worldY);

            if (distance < JUPITER_RADIUS + JUPITER_ATMOSPHERE)
            {
                double worldNormalX = worldX / distance;
                double worldNormalY = worldY / distance;

                double sunDotProduct = sunNormalX * worldNormalX + sunNormalY * worldNormalY;

                if (sunDotProduct < 0)
                {
                    sunDotProduct = 0.05f;
                }
                else
                {
                    sunDotProduct = 0.05f + sunDotProduct * 0.95f;
                }

                if (distance < JUPITER_RADIUS)
                {
                    double ratio = distance / JUPITER_RADIUS;

                    double worldAngle = atan2(worldNormalX, worldNormalY) + atan2(ratio, 0.9) + rotation;

                    double textureFactor = 0.25 * sin(worldAngle * 60) + 0.75;

                    int redComponent = (int)(sunDotProduct * 255 * ratio * textureFactor);

                    image[index] = ALPHA | (redComponent << 16);
                }
                else
                {
                    double ratio = (distance - JUPITER_RADIUS) / JUPITER_ATMOSPHERE;

                    int redComponent = (int)((255 - ratio * 255) * sunDotProduct);

                    image[index] = ALPHA | (redComponent << 16);
                }
            }
        }
    }
}
