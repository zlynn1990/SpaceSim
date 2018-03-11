using System.Drawing;
using OpenCLWrapper;
using SpaceSim.Physics;
using SpaceSim.SolarSystem;

namespace SpaceSim.Drawing
{
    interface IGpuRenderable : IMapRenderable
    {
        #region OpenCL

        IMassiveKernel Kernel { get; }

        void Load(OpenCLProxy clProxy);

        void RenderCl(OpenCLProxy clProxy, Camera camera, IPhysicsBody sun);

        #endregion

        #region GdiPlus Fallback

        void RenderGdiFallback(Graphics graphics, Camera camera, IPhysicsBody sun);

        #endregion
    }
}
