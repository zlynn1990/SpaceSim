using SpaceSim.SolarSystem;

namespace SpaceSim.Kernels
{
    class SunKernel : BaseKernel, IMassiveKernel
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
