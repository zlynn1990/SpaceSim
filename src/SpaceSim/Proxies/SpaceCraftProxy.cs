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

        public bool OnGround { get; private set; }

        public override double Mass { get { return _proxy.DryMass + PropellantMass; } }

        public double Width { get { return _proxy.Width; }}
        public double Height {get { return _proxy.Height; }}

        public double HeatingRate { get { return 0; } }

        public double FormDragCoefficient
        {
            get
            {
                return 1.2;
            }
        }
        public double FrontalArea { get { return Math.PI * Math.Pow(Width / 2, 2); } }

        public double SkinFrictionCoefficient
        {
            get
            {
                double velocity = RelativeVelocity.Length();
                double viscosity = GravitationalParent.GetAtmosphericViscosity(Altitude);
                double reynoldsNumber = (velocity * Height) / viscosity;

                return 0.455 / Math.Pow(Math.Log10(reynoldsNumber), 2.58);
            }
        }

        public double ExposedSurfaceArea
        {
            get
            {
                // A = 2πrh + πr2
                return 2 * Math.PI * (Width / 2) * Height + FrontalArea;
            }
        }

        public double LiftCoefficient { get { return 0.3; } }
        public double LiftingSurfaceArea { get { return Width * Height; } }

        public double PropellantMass { get; private set; }

        public IEngine[] Engines { get; private set; }

        public DVector2 AccelerationD { get; private set; }
        public DVector2 AccelerationL { get; private set; }
        public DVector2 AccelerationN { get; private set; }

        public override Color IconColor { get { return Color.White; } }

        private double _thrust;

        private SpaceCraftBase _proxy;

        public SpaceCraftProxy(DVector2 position, DVector2 velocity, SpaceCraftBase spaceCraft)
            : base(position, velocity, spaceCraft.Pitch)
        {
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

            double distance = difference.Length() - Height * 0.5;

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

                    Position = body.Position + normal*(body.SurfaceRadius);

                    Pitch = normal.Angle();

                    AccelerationN.X = -AccelerationG.X;
                    AccelerationN.Y = -AccelerationG.Y;

                    OnGround = true;
                }
                else
                {
                    OnGround = false;
                }

                double atmosphericDensity = body.GetAtmosphericDensity(Altitude);

                RelativeVelocity = (body.Velocity + surfaceNormal * rotationalSpeed) - Velocity;

                double velocityMagnitude = RelativeVelocity.LengthSquared();

                if (velocityMagnitude > 0)
                {
                    DVector2 normalizedRelativeVelocity = RelativeVelocity.Clone();
                    normalizedRelativeVelocity.Normalize();

                    double formDragTerm = FormDragCoefficient * FrontalArea;
                    double skinFrictionTerm = SkinFrictionCoefficient * ExposedSurfaceArea;
                    double dragTerm = formDragTerm + skinFrictionTerm;

                    // Drag ( Fd = 0.5pv^2dA )
                    DVector2 dragForce = normalizedRelativeVelocity * (0.5 * atmosphericDensity * velocityMagnitude * dragTerm);

                    AccelerationD = dragForce / Mass;
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
                double ispMultiplier = GravitationalParent.GetIspMultiplier(Altitude);

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
