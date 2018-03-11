﻿using SpaceSim.SolarSystem;

namespace SpaceSim.Kernels
{
    class SunKernel : SymbolKernel, IMassiveKernel
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
            double diffX = bodyX - worldX;
            double diffY = bodyY - worldY;

            double distance = sqrt(diffX * diffX + diffY * diffY);

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
