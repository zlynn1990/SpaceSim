using SpaceSim.SolarSystem;

namespace SpaceSim.Kernels
{
    class MercuryKernel : SymbolKernel, IMassiveKernel
    {
        // -------- ------ Image ----- --------
        // 01111111 00000000 00000000 00000000 alpha >> 24
        // 00000000 11111111 00000000 00000000 red >> 16
        // 00000000 00000000 11111111 00000000 green >> 8
        // 00000000 00000000 00000000 11111111 blue
        public void Run(int[] image, int resX, int resY, double cX, double cY, double cWidth, double cHeight, double cRot, double sunNormalX, double sunNormalY, double bodyX, double bodyY, double bodyRot)
        {
            int index = get_global_id(0);

            // screen-space coords
            int y = index / resX;
            int x = index - (y * resX);

            // screen-space uvmap
            float u = (float)x / resX;
            float v = (float)y / resY;

            // world-space pixel location
            double worldX = cX + cWidth * u;
            double worldY = cY + cHeight * v;
            double distance = sqrt(worldX * worldX + worldY * worldY);

            if (distance < MERCURY_RADIUS)
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

                double ratio = distance / MERCURY_RADIUS;

                double worldAngle = atan2(worldNormalX, worldNormalY) + atan2(ratio, 0.9) + bodyRot;

                double textureFactor = 0.25 * sin(worldAngle * 15) + 0.75;

                int rgbComponent = (int)(sunDotProduct * 255 * ratio * textureFactor);

                int color = ALPHA | (rgbComponent << 16);
                color = color | (rgbComponent << 8);
                image[index] = color | rgbComponent;
            }
        }
    }
}
