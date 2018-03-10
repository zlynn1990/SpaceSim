using SpaceSim.SolarSystem;

namespace SpaceSim.Kernels
{
    class ClearKernel : SymbolKernel, IMassiveKernel
    {
        // -------- ------ Image ----- --------
        // 01111111 00000000 00000000 00000000 alpha >> 24
        // 00000000 11111111 00000000 00000000 red >> 16
        // 00000000 00000000 11111111 00000000 green >> 8
        // 00000000 00000000 00000000 11111111 blue
        public void Run(int[] image, int resX, int resY, double cX, double cY, double cWidth, double cHeight, double cRot, double sunNormalX, double sunNormalY, double bodyX, double bodyY, double bodyRot)
        {
            int index = get_global_id(0);

            image[index] = ALPHA;
        }
    }
}
