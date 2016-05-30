using System;
using SpaceSim.Physics;
using SpaceSim.SolarSystem;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Proxies
{
    /// <summary>
    /// Spacecraft proxy used for doing orbital approximations and traces.
    /// </summary>
    class SpaceCraftProxy : GravitationalBodyBase, IAreodynamicBody
    {
        public override double Mass { get { return _proxy.Mass; } }

        public bool ExposedToAirFlow { get { return _proxy.ExposedToAirFlow; } }
        public double HeatingRate { get { return _proxy.HeatingRate; } }

        public double DragCoefficient { get { return _proxy.DragCoefficient; } }
        public double CrossSectionalArea { get { return _proxy.CrossSectionalArea; } }
        public double SurfaceArea { get { return _proxy.SurfaceArea; } }

        public DVector2 AccelerationD { get; private set; }
        public DVector2 AccelerationL { get; private set; }
        public DVector2 AccelerationN { get; private set; }

        private SpaceCraftBase _proxy;

        public SpaceCraftProxy(DVector2 position, DVector2 velocity, SpaceCraftBase spaceCraft)
            : base(position, velocity, 0)
        {
            _proxy = spaceCraft;
        }

        public override void ResetAccelerations()
        {
            AccelerationD = DVector2.Zero;
            AccelerationL = DVector2.Zero;
            AccelerationN = DVector2.Zero;

            base.ResetAccelerations();
        }

        public void ApplyFrameOffset(DVector2 offset)
        {
            Position -= offset;
        }

        public void ResolveAtmopsherics(IMassiveBody body)
        {
            DVector2 difference = body.Position - Position;

            double distance = difference.Length();

            difference.Normalize();

            double altitude = distance - body.SurfaceRadius;

            // The object is in the atmosphere of body B
            if (altitude < body.AtmosphereHeight)
            {
                var surfaceNormal = new DVector2(-difference.Y, difference.X);

                double altitudeFromCenter = altitude + body.SurfaceRadius;

                // Distance of circumference at this altitude ( c= 2r * pi )
                double pathCirumference = 2 * Math.PI * altitudeFromCenter;

                double rotationalSpeed = pathCirumference / body.RotationPeriod;

                // Simple collision detection
                if (altitude <= 0)
                {
                    var normal = new DVector2(-difference.X, -difference.Y);

                    Position = body.Position + normal * (body.SurfaceRadius);

                    Velocity = (body.Velocity + surfaceNormal * rotationalSpeed);

                    Rotation = normal.Angle();

                    AccelerationN.X = -AccelerationG.X;
                    AccelerationN.Y = -AccelerationG.Y;
                }

                double atmosphericDensity = body.GetAtmosphericDensity(altitude);

                DVector2 relativeVelocity = (body.Velocity + surfaceNormal * rotationalSpeed) - Velocity;

                double velocityMagnitude = relativeVelocity.LengthSquared();

                if (velocityMagnitude > 0)
                {
                    relativeVelocity.Normalize();

                    // Drag ( Fd = 0.5pv^2dA )
                    DVector2 dragForce = relativeVelocity * (0.5 * atmosphericDensity * velocityMagnitude * DragCoefficient * CrossSectionalArea);

                    AccelerationD += dragForce / Mass;
                }
            }
        }

        public override void Update(double dt)
        {
            Velocity += (AccelerationG * dt);
            Velocity += (AccelerationD * dt);
            Velocity += (AccelerationN * dt);

            Position += (Velocity * dt);
        }
    }
}
