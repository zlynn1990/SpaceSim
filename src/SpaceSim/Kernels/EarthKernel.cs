using SpaceSim.SolarSystem;

namespace SpaceSim.Kernels
{
    class EarthKernel : BaseKernel, IMassiveKernel
    {
        // -------- ------ Image ----- --------
        // 01111111 00000000 00000000 00000000 alpha >> 24
        // 00000000 11111111 00000000 00000000 red >> 16
        // 00000000 00000000 11111111 00000000 green >> 8
        // 00000000 00000000 00000000 11111111 blue
        public void Run(int[] image, int resX, int resY, double cX, double cY, double cWidth, double cHeight, double cRot, double sunNormalX, double sunNormalY, double bodyX, double bodyY, double bodyRot)
        {
            int index = get_global_id(0);

            double diffX, diffY;
            ComputeCameraRotation(index, resX, resY, cX, cY, cWidth, cHeight, cRot, bodyX, bodyY, out diffX, out diffY);

            double distance = sqrt(diffX * diffX + diffY * diffY);

            if (distance < EARTH_RADIUS + EARTH_ATMOSPHERE)
            {
                double worldNormalX, worldNormalY, sunDotProduct;
                ComputeSunShading(diffX, diffY, distance, sunNormalX, sunNormalY, out worldNormalX, out worldNormalY, out sunDotProduct);

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
                    
                    int red = (int)((95 - ratio * 95) * sunDotProduct);
                    int green = (int)((188 - ratio * 188) * sunDotProduct);
                    int blue = (int)((255 - ratio * 255) * sunDotProduct);

                    image[index] = ALPHA | blue | (green << 8) | (red << 16);
                }
            }
        }
    }
}
