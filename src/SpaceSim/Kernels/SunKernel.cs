using SpaceSim.SolarSystem;

namespace SpaceSim.Kernels
{
    class SunKernel : SymbolKernel, IMassiveKernel
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

            if (distance < SUN_RADIUS + SUN_ATMOSPHERE)
            {
                if (distance < SUN_RADIUS)
                {
                    image[index] = ALPHA | RED | GREEN;
                }
                else
                {
                    double ratio = (distance - SUN_RADIUS) / SUN_ATMOSPHERE;

                    int rgComponent = (int)(255 - ratio * 255);

                    int color = ALPHA | (rgComponent << 16);
                    image[index] = color | (rgComponent << 8);
                }
            }
        }
    }
}
