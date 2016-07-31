using System;
using System.Drawing;
using Cloo;
using OpenCLWrapper;
using SpaceSim.Drawing;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.SolarSystem
{
    abstract class MassiveBodyBase : GravitationalBodyBase, IMassiveBody, IGpuRenderable
    {
        public IMassiveKernel Kernel { get; private set; }

        public abstract double SurfaceRadius { get; }
        public abstract double AtmosphereHeight { get; } 

        public abstract double RotationRate { get; }
        public abstract double RotationPeriod { get; }

        public abstract Color IconColor { get; }
        public abstract Color IconAtmopshereColor { get; }

        private ComputeKernel _computeKernel;

        protected MassiveBodyBase(DVector2 position, DVector2 velocity, IMassiveKernel kernel)
            : base(position, velocity, 0)
        {
            Kernel = kernel;
        }

        public override void Update(double dt)
        {
            // Integrate for position
            Velocity += (AccelerationG * dt);
            Position += (Velocity * dt);

            // Rotate the planet
            Pitch += (RotationRate * dt);
        }

        public double GetSurfaceGravity()
        {
            double r2 = SurfaceRadius * SurfaceRadius;

            double massDistanceRatio = Mass / r2;

            // Gravitation ( aG = G m1 / r^2 )
            return FlightGlobals.GRAVITATION_CONSTANT*massDistanceRatio;
        }

        public double GetIspMultiplier(double altitude)
        {
            double heightRatio = Math.Max(altitude / AtmosphereHeight, 0);

            return 1.0 - Math.Exp(-21.3921 * heightRatio);
        }

        public virtual double GetAtmosphericDensity(double altitude)
        {
            double heightRatio = Math.Max(altitude / AtmosphereHeight, 0);

            return Math.Exp(-21.3921 * heightRatio);
        }

        public virtual double GetAtmosphericViscosity(double altitude)
        {
            return 1.48e-5;
        }

        public virtual double BoundingRadius
        {
            get { return SurfaceRadius + AtmosphereHeight; }
        }

        public virtual RectangleD ComputeBoundingBox()
        {
            double totalRadius = BoundingRadius;

            return new RectangleD(Position.X - totalRadius, Position.Y - totalRadius, totalRadius * 2, totalRadius * 2);
        }

        public virtual double Visibility(RectangleD cameraBounds)
        {
            double distanceRatio = BoundingRadius / cameraBounds.Width;

            if (distanceRatio > 0.0025)
            {
                return 1;
            }

            if (distanceRatio > 0.002)
            {
                return (distanceRatio - 0.002) * 2000;
            }

            return 0;
        }

        public void Load(OpenCLProxy clProxy)
        {
            if (Kernel != null)
            {
                _computeKernel = clProxy.CreateKernel(Kernel);
            }
        }

        public void RenderCl(OpenCLProxy clProxy, RectangleD cameraBounds, IPhysicsBody sun)
        {
            RectangleD bounds = ComputeBoundingBox();

            // Not in range easy return
            if (!cameraBounds.IntersectsWith(bounds))
            {
                return;
            }

            DVector2 sunNormal = DVector2.Zero;

            if (sun != null)
            {
                sunNormal = sun.Position - Position;

                sunNormal.Normalize();
            }

            var normalizedPosition = new DVector2(cameraBounds.X - Position.X, cameraBounds.Y - Position.Y);

            if (clProxy.HardwareAccelerationEnabled)
            {
                clProxy.UpdateDoubleArgument("cameraLeft", normalizedPosition.X);
                clProxy.UpdateDoubleArgument("cameraTop", normalizedPosition.Y);

                clProxy.UpdateDoubleArgument("cameraWidth", cameraBounds.Width);
                clProxy.UpdateDoubleArgument("cameraHeight", cameraBounds.Height);

                clProxy.UpdateDoubleArgument("sunNormalX", sunNormal.X);
                clProxy.UpdateDoubleArgument("sunNormalY", sunNormal.Y);

                clProxy.UpdateDoubleArgument("rotation", Pitch);

                clProxy.RunKernel(_computeKernel, RenderUtils.ScreenArea);
            }
            else
            {
                int totalSize = RenderUtils.ScreenArea;

                for (int i = 0; i < totalSize; i++)
                {
                    Kernel.Run(clProxy.ReadIntBuffer("image", totalSize), RenderUtils.ScreenWidth, RenderUtils.ScreenHeight,
                                                    normalizedPosition.X, normalizedPosition.Y, cameraBounds.Width, cameraBounds.Height,
                                                    sunNormal.X, sunNormal.Y, Pitch);
                }

                Kernel.Finish();
            }
        }

        public void RenderGdiFallback(Graphics graphics, RectangleD cameraBounds, IPhysicsBody sun)
        {
            RectangleD bounds = ComputeBoundingBox();

            // Not in range easy return
            if (!cameraBounds.IntersectsWith(bounds))
            {
                return;
            }

            if (AtmosphereHeight > 0)
            {
                RectangleF atmosphereBounds = RenderUtils.ComputeEllipseSize(Position, cameraBounds, BoundingRadius);

                // Saftey
                if (atmosphereBounds.Width > RenderUtils.ScreenWidth * 5000) return;

                graphics.FillEllipse(new SolidBrush(IconAtmopshereColor), atmosphereBounds);
            }

            RectangleF surfaceBounds = RenderUtils.ComputeEllipseSize(Position, cameraBounds, SurfaceRadius);

            // Saftey
            if (surfaceBounds.Width > RenderUtils.ScreenWidth * 5000) return;

            graphics.FillEllipse(new SolidBrush(IconColor), surfaceBounds);
        }
    }
}
