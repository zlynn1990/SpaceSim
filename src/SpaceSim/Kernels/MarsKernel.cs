using SpaceSim.SolarSystem;

namespace SpaceSim.Kernels
{
    class MarsKernel : BaseKernel, IMassiveKernel
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

            if (distance < MARS_RADIUS + MARS_ATMOSPHERE)
            {
                double worldNormalX, worldNormalY, sunDotProduct;
                ComputeSunShading(diffX, diffY, distance, sunNormalX, sunNormalY, out worldNormalX, out worldNormalY, out sunDotProduct);

                if (distance < MARS_RADIUS)
                {
                    double ratio = distance / MARS_RADIUS;

                    double worldAngle = atan2(worldNormalX, worldNormalY) + atan2(ratio, 1.1) + bodyRot;

                    double textureFactor = 0.25 * sin(worldAngle * 30) + 0.75;

                    int red = (int)(sunDotProduct * 213 * ratio * textureFactor);
                    int green = (int)(sunDotProduct * 158 * ratio * textureFactor);
                    int blue = (int)(sunDotProduct * 104 * ratio * textureFactor);

                    image[index] = ALPHA | (green << 8) | (red << 16);
                }
                else
                {
                    double ratio = (distance - MARS_RADIUS) / MARS_ATMOSPHERE;

                    int red = (int)((233 - ratio * 233) * sunDotProduct);
                    int green = (int)((208 - ratio * 208) * sunDotProduct);
                    int blue = (int)((178 - ratio * 178) * sunDotProduct);

                    image[index] = ALPHA | blue | (green << 8) | (red << 16);
                }
            }
        }
    }
}
