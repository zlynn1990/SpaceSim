using System;
using System.Drawing;
using Cloo;
using OpenCLWrapper;
using SpaceSim.Drawing;
using SpaceSim.Orbits;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.SolarSystem
{
    abstract class MassiveBodyBase : GravitationalBodyBase, IMassiveBody, IGpuRenderable
    {
        public IMassiveKernel Kernel { get; }

        public virtual string ApoapsisName { get { return "Apoapsis"; } }
        public virtual string PeriapsisName { get { return "Periapsis"; } }

        public abstract double SurfaceRadius { get; }
        public abstract double AtmosphereHeight { get; } 

        public abstract double RotationRate { get; }
        public abstract double RotationPeriod { get; }

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
            return Constants.GravitationConstant*massDistanceRatio;
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

        public double GetSurfaceAngle(DateTime localTime, IMassiveBody sun)
        {
            DVector2 sunDifference = sun.Position - Position;

            double noonAngle = sunDifference.Angle();

            TimeSpan noonOffset = new TimeSpan(localTime.Hour, localTime.Minute, localTime.Second) - new TimeSpan(12, 0, 0);

            return noonAngle + noonOffset.TotalSeconds * RotationRate;
        }

        public virtual double BoundingRadius
        {
            get { return SurfaceRadius + AtmosphereHeight; }
        }

        public override RectangleD ComputeBoundingBox()
        {
            double totalRadius = BoundingRadius;

            return new RectangleD(Position.X - totalRadius, Position.Y - totalRadius, totalRadius * 2, totalRadius * 2);
        }

        public override double Visibility(RectangleD cameraBounds)
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

        public override void FixedUpdate(TimeStep timeStep)
        {
            OrbitHelper.TraceMassiveBody(this, OrbitTrace);
        }

        public void RenderCl(OpenCLProxy clProxy, Camera camera, IPhysicsBody sun)
        {
            RectangleD bounds = ComputeBoundingBox();

            // Not in range easy return
            if (!camera.Intersects(bounds))
            {
                return;
            }

            DVector2 sunNormal = DVector2.Zero;

            if (sun != null)
            {
                sunNormal = sun.Position - Position;

                sunNormal.Normalize();
            }

            if (clProxy.HardwareAccelerationEnabled)
            {
                clProxy.UpdateDoubleArgument("cX", camera.Bounds.X);
                clProxy.UpdateDoubleArgument("cY", camera.Bounds.Y);

                clProxy.UpdateDoubleArgument("cWidth", camera.Bounds.Width);
                clProxy.UpdateDoubleArgument("cHeight", camera.Bounds.Height);

                clProxy.UpdateDoubleArgument("cRot", -camera.Rotation);

                clProxy.UpdateDoubleArgument("sunNormalX", sunNormal.X);
                clProxy.UpdateDoubleArgument("sunNormalY", sunNormal.Y);

                clProxy.UpdateDoubleArgument("bodyX", Position.X);
                clProxy.UpdateDoubleArgument("bodyY", Position.Y);
                clProxy.UpdateDoubleArgument("bodyRot", Pitch);

                clProxy.RunKernel(_computeKernel, RenderUtils.ScreenArea);
            }
            else
            {
                int totalSize = RenderUtils.ScreenArea;

                for (int i = 0; i < totalSize; i++)
                {
                    Kernel.Run(clProxy.ReadIntBuffer("image", totalSize), RenderUtils.ScreenWidth, RenderUtils.ScreenHeight,
                               camera.Bounds.X, camera.Bounds.Y, camera.Bounds.Width, camera.Bounds.Height, camera.Rotation,
                               sunNormal.X, sunNormal.Y, Position.X, Position.Y, Pitch);
                }

                Kernel.Finish();
            }
        }

        public void RenderGdiFallback(Graphics graphics, Camera camera, IPhysicsBody sun)
        {
            RectangleD bounds = ComputeBoundingBox();

            // Not in range easy return
            if (!camera.Intersects(bounds))
            {
                return;
            }

            if (AtmosphereHeight > 0)
            {
                RectangleF atmosphereBounds = RenderUtils.ComputeEllipseSize(Position, camera.Bounds, BoundingRadius);

                // Saftey
                if (atmosphereBounds.Width > RenderUtils.ScreenWidth * 5000)
                {
                    // Just render the atmosphere color to simulate being in the air
                    graphics.Clear(IconAtmopshereColor);

                    return;
                }

                graphics.FillEllipse(new SolidBrush(IconAtmopshereColor), atmosphereBounds);
            }

            RectangleF surfaceBounds = RenderUtils.ComputeEllipseSize(Position, camera.Bounds, SurfaceRadius);

            // Saftey
            if (surfaceBounds.Width > RenderUtils.ScreenWidth * 5000) return;

            graphics.FillEllipse(new SolidBrush(IconColor), surfaceBounds);
        }
    }
}