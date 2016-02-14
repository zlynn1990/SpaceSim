using SpaceSim.SolarSystem;

namespace SpaceSim.Kernels
{
    class SaturnKernel : SymbolKernel, IMassiveKernel
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

            if (distance < SATURN_RADIUS + SATURN_RING_END)
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

                // In the ring
                if (distance > SATURN_RADIUS + SATURN_RING_START)
                {
                    double ringRatio = (distance - SATURN_RING_START) / SATURN_RING_END;

                    int textureComponent = (int)(sunDotProduct * cos(ringRatio * 200) * sin(ringRatio * 20) * 150);

                    image[index] = ALPHA | (textureComponent << 8) | (textureComponent << 16);
                }
                else if (distance < SATURN_RADIUS + SATURN_ATMOSPHERE)
                {
                    if (distance < SATURN_RADIUS)
                    {
                        double ratio = distance / SATURN_RADIUS;

                        double worldAngle = atan2(worldNormalX, worldNormalY) + atan2(ratio, 1.1) + rotation;

                        double textureFactor = 0.25 * sin(worldAngle * 35) + 0.75;

                        int rgComponents = (int)(sunDotProduct * 255 * ratio);

                        int textureComponent = (int)(sunDotProduct * 255 * textureFactor * ratio);

                        image[index] = ALPHA | textureComponent | (rgComponents << 8) | (rgComponents << 16);
                    }
                    else
                    {
                        double ratio = (distance - SATURN_RADIUS) / SATURN_ATMOSPHERE;

                        int rgComponents = (int)((255 - ratio * 255) * sunDotProduct);

                        image[index] = ALPHA | (rgComponents << 8) | (rgComponents << 16);
                    }
                }
            }
        }
    }
}
