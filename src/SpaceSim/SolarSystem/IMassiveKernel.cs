namespace SpaceSim.SolarSystem
{
    interface IMassiveKernel
    {
        void Run(int[] image, int resX, int resY, double cX, double cY, double cWidth, double cHeight, double cRot, double sunNormalX, double sunNormalY, double bodyX, double bodyY, double bodyRot);

        void Finish();
    }
}
