namespace SpaceSim.SolarSystem
{
    interface IMassiveKernel
    {
        void Run(int[] image, int resX, int resY, double cameraLeft, double cameraTop, double cameraWidth, double cameraHeight, double sunNormalX, double sunNormalY, double rotation);

        void Finish();
    }
}
