using System;
using System.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using SpaceSim.SolarSystem;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Proxies
{
    /// <summary>
    /// Spacecraft proxy used for doing orbital approximations and traces.
    /// </summary>
    class SpaceCraftProxy : GravitationalBodyBase, IAerodynamicBody
    {
        public double Yaw { get { return 0; } }
        public double Roll { get { return 0; } }

        public AeroDynamicProperties GetAeroDynamicProperties { get { return _proxy.GetAeroDynamicProperties; } }

        public double Altitude { get; private set; }
        public DVector2 RelativeVelocity { get; private set; }

        public override double Mass { get { return _dryMass + PropellantMass; } }

        public double Height {get { return _proxy.Height; }}

        public double HeatingRate { get { return _proxy.HeatingRate; } }

        public double FormDragCoefficient { get { return _proxy.FormDragCoefficient; } }
        public double CrossSectionalArea { get { return _proxy.CrossSectionalArea; } }
        public double SkinFrictionCoefficient { get { return _proxy.SkinFrictionCoefficient; } }
        public double ExposedSurfaceArea { get { return _proxy.ExposedSurfaceArea; } }
        public double LiftCoefficient { get { return _proxy.LiftCoefficient; } }
        public double LiftingSurfaceArea { get { return _proxy.LiftingSurfaceArea; } }

        public double PropellantMass { get; private set; }

        public IEngine[] Engines { get; private set; }

        public DVector2 AccelerationD { get; private set; }
        public DVector2 AccelerationL { get; private set; }
        public DVector2 AccelerationN { get; private set; }

        public override Color IconColor { get { return Color.White; } }

        private double _dryMass;
        private double _thrust;

        private SpaceCraftBase _proxy;

        public SpaceCraftProxy(DVector2 position, DVector2 velocity, SpaceCraftBase spaceCraft)
            : base(position, velocity, spaceCraft.Pitch)
        {
            _dryMass = spaceCraft.DryMass;
            PropellantMass = spaceCraft.PropellantMass;

            Engines = new IEngine[spaceCraft.Engines.Length];

            for (int i = 0; i < spaceCraft.Engines.Length; i++)
            {
                Engines[i] = spaceCraft.Engines[i].Clone();
            }

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

            Altitude = distance - body.SurfaceRadius;

            // The object is in the atmosphere of body B
            if (Altitude < body.AtmosphereHeight)
            {
                var surfaceNormal = new DVector2(-difference.Y, difference.X);

                double altitudeFromCenter = Altitude + body.SurfaceRadius;

                // Distance of circumference at this altitude ( c= 2r * pi )
                double pathCirumference = 2 * Math.PI * altitudeFromCenter;

                double rotationalSpeed = pathCirumference / body.RotationPeriod;

                // Simple collision detection
                if (Altitude <= 0)
                {
                    var normal = new DVector2(-difference.X, -difference.Y);

                    Position = body.Position + normal * (body.SurfaceRadius);

                    Pitch = normal.Angle();

                    AccelerationN.X = -AccelerationG.X;
                    AccelerationN.Y = -AccelerationG.Y;
                }

                double atmosphericDensity = body.GetAtmosphericDensity(Altitude);

                RelativeVelocity = (body.Velocity + surfaceNormal * rotationalSpeed) - Velocity;

                double velocityMagnitude = RelativeVelocity.LengthSquared();

                if (velocityMagnitude > 0)
                {
                    DVector2 normalizedRelativeVelocity = RelativeVelocity.Clone();
                    normalizedRelativeVelocity.Normalize();

                    double formDragTerm = FormDragCoefficient * CrossSectionalArea;
                    double skinFrictionTerm = SkinFrictionCoefficient * ExposedSurfaceArea;
                    double dragTerm = formDragTerm + skinFrictionTerm;

                    double liftTerm = LiftCoefficient * LiftingSurfaceArea;

                    // Drag ( Fd = 0.5pv^2dA )
                    DVector2 dragForce = normalizedRelativeVelocity * (0.5 * atmosphericDensity * velocityMagnitude * dragTerm);
                    DVector2 liftForce = normalizedRelativeVelocity * (0.5 * atmosphericDensity * velocityMagnitude * liftTerm);

                    AccelerationD += dragForce / Mass;
                    AccelerationN += liftForce / Mass;
                }
            }
        }

        public override void Update(double dt)
        {
            UpdateEngines(dt);

            if (_thrust > 0)
            {
                var thrustVector = new DVector2(Math.Cos(Pitch), Math.Sin(Pitch));

                AccelerationN += (thrustVector * _thrust) / Mass;
            }

            Velocity += (AccelerationG * dt);
            Velocity += (AccelerationD * dt);
            Velocity += (AccelerationN * dt);

            Position += (Velocity * dt);
        }

        public override double Visibility(RectangleD cameraBounds)
        {
            throw new NotImplementedException();
        }

        public override RectangleD ComputeBoundingBox()
        {
            throw new NotImplementedException();
        }

        public override void FixedUpdate(TimeStep timeStep)
        {
            throw new NotImplementedException();
        }

        private void UpdateEngines(double dt)
        {
            _thrust = 0;

            if (Engines.Length > 0 && PropellantMass > 0)
            {
                double altitude = GetRelativeAltitude();

                double ispMultiplier = GravitationalParent.GetIspMultiplier(altitude);

                foreach (IEngine engine in Engines)
                {
                    if (!engine.IsActive) continue;

                    _thrust += engine.Thrust(ispMultiplier);

                    PropellantMass = Math.Max(0, PropellantMass - engine.MassFlowRate() * dt);
                }
            }
        }
    }
}
