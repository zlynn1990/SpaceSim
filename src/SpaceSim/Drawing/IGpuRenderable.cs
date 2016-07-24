using System.Drawing;
using OpenCLWrapper;
using SpaceSim.Physics;
using SpaceSim.SolarSystem;
using VectorMath;

namespace SpaceSim.Drawing
{
    interface IGpuRenderable : IMapRenderable
    {
        #region OpenCL

        IMassiveKernel Kernel { get; }

        void Load(OpenCLProxy clProxy);

        void RenderCl(OpenCLProxy clProxy, RectangleD cameraBounds, IPhysicsBody sun);

        #endregion

        #region GdiPlus Fallback

        void RenderGdiFallback(Graphics graphics, RectangleD cameraBounds, IPhysicsBody sun);

        #endregion
    }
}
