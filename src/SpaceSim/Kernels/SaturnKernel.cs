using SpaceSim.SolarSystem;

namespace SpaceSim.Kernels
{
    class SaturnKernel : BaseKernel, IMassiveKernel
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

            if (distance < SATURN_RADIUS + SATURN_RING_END)
            {
                double worldNormalX, worldNormalY, sunDotProduct;
                ComputeSunShading(diffX, diffY, distance, sunNormalX, sunNormalY, out worldNormalX, out worldNormalY, out sunDotProduct);

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

                        double worldAngle = atan2(worldNormalX, worldNormalY) + atan2(ratio, 1.1) + bodyRot;

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
