using Cloo;
using OpenCLWrapper;
using SpaceSim.Kernels;
using SpaceSim.SolarSystem;

namespace SpaceSim.Drawing
{
    class GpuClear
    {
        private IMassiveKernel _kernel;
        private ComputeKernel _computeKernel;

        public void Load(OpenCLProxy clProxy)
        {
            _kernel = new ClearKernel();

            _computeKernel = clProxy.CreateKernel(_kernel);
        }

        public void RenderCl(OpenCLProxy clProxy)
        {
            if (clProxy.HardwareAccelerationEnabled)
            {
                clProxy.RunKernel(_computeKernel, RenderUtils.ScreenArea);
            }
            else
            {
                int totalSize = RenderUtils.ScreenArea;

                for (int i = 0; i < totalSize; i++)
                {
                    _kernel.Run(clProxy.ReadIntBuffer("image", totalSize), 0, 0, 0, 0, 0, 0, 0, 0, 0);
                }

                _kernel.Finish();
            }
        }
    }
}
