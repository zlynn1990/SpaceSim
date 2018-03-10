using SpaceSim.SolarSystem;

namespace SpaceSim.Kernels
{
    class EarthKernel : SymbolKernel, IMassiveKernel
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

            if (distance < EARTH_RADIUS + EARTH_ATMOSPHERE)
            {
                double worldNormalX = -diffX / distance;
                double worldNormalY = -diffY / distance;

                double sunDotProduct = sunNormalX * worldNormalX + sunNormalY * worldNormalY;

                if (sunDotProduct < 0)
                {
                    sunDotProduct = 0.1f;
                }
                else
                {
                    sunDotProduct = 0.1f + sunDotProduct * 0.9f;
                }

                if (distance < EARTH_RADIUS)
                {
                    double ratio = distance / EARTH_RADIUS;

                    double worldAngle = atan2(worldNormalX, worldNormalY) + atan2(ratio, 1.1) + bodyRot;

                    double textureFactor = 0.25 * sin(worldAngle * 500) + 0.75;

                    int red = (int)(sunDotProduct * 127 * ratio * textureFactor);
                    int green = (int)(sunDotProduct * 225 * ratio * textureFactor);
                    int blue = (int)(sunDotProduct * 63 * ratio * textureFactor);

                    image[index] = ALPHA | (green << 8) | (red << 16);
                }
                else
                {
                    double ratio = (distance - EARTH_RADIUS) / EARTH_ATMOSPHERE;
                    
                    int red = (int)((125 - ratio * 125) * sunDotProduct);
                    int green = (int)((188 - ratio * 188) * sunDotProduct);
                    int blue = (int)((255 - ratio * 255) * sunDotProduct);

                    image[index] = ALPHA | blue | (green << 8) | (red << 16);
                }
            }
        }
    }
}
