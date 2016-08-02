using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using SpaceSim.Commands;
using SpaceSim.Controllers;
using SpaceSim.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using SpaceSim.SolarSystem;
using VectorMath;

namespace SpaceSim.Spacecrafts
{
    abstract class SpaceCraftBase : GravitationalBodyBase, IAerodynamicBody, ISpaceCraft, IMapRenderable, IGdiRenderable
    {
        public abstract string CraftName { get; }
        public string CraftDirectory { get; protected set; }

        public ISpaceCraft Parent { get; protected set; }
        public List<ISpaceCraft> Children { get; protected set; }

        public override double Mass
        {
            get
            {
                double childMass = Children.Sum(child => child.Mass);

                return childMass + DryMass + PropellantMass;
            }
        }

        public abstract double Width { get; }
        public abstract double Height { get; }

        public double TotalWidth
        {
            get { return Width + Children.Sum(spaceCraft => spaceCraft.TotalWidth); }
        }

        public double TotalHeight
        {
            get { return Height + Children.Sum(spaceCraft => spaceCraft.TotalHeight); }
        }

        /// <summary>
        /// Throttle is the maximum of all engines on the spacecraft.
        /// </summary>
        public double Throttle
        {
            get
            {
                double throttle = 0;

                foreach (IEngine engine in Engines)
                {
                    if (engine.Throttle > throttle)
                    {
                        throttle = engine.Throttle;
                    }
                }

                foreach (ISpaceCraft child in Children)
                {
                    if (child.Throttle > throttle)
                    {
                        throttle = child.Throttle;
                    }
                }

                return throttle;
            }
        }

        public double Thrust { get; protected set; }
        public double HeatingRate { get; protected set; }

        public abstract double DryMass { get; }
        public double PropellantMass { get; protected set; }

        public IEngine[] Engines { get; protected set; }
        public IController Controller { get; protected set; }

        public abstract string CommandFileName { get; }

        public bool OnGround { get; private set; }

        public abstract AeroDynamicProperties GetAeroDynamicProperties { get; }
        public abstract double FormDragCoefficient { get; }
        public abstract double CrossSectionalArea { get; }
        public abstract double ExposedSurfaceArea { get; }
        public abstract double LiftingSurfaceArea { get; }
        public abstract double LiftCoefficient { get; }

        public virtual double GetBaseCd(double baseCd)
        {
            if (MachNumber > 1.0)
            {
                double exp = Math.Exp(0.3 / MachNumber);
                baseCd *= 1.4 * exp;
            }

            return baseCd;
        }

        public virtual double SkinFrictionCoefficient
        {
            get
            {
                double velocity = GetRelativeVelocity().Length();
                double altitude = GetRelativeAltitude();
                double viscosity = GravitationalParent.GetAtmosphericViscosity(altitude);
                double reynoldsNumber = (velocity * Height) / viscosity;
                return 0.455 / Math.Pow(Math.Log10(reynoldsNumber), 2.58);
            }
        }

        public DVector2 AccelerationD { get; protected set; }
        public DVector2 AccelerationN { get; protected set; }
        public DVector2 AccelerationL { get; protected set; }

        public abstract Color IconColor { get; }

        protected Bitmap Texture;
        protected DVector2 StageOffset;

        protected double MachNumber;
        protected double IspMultiplier;

        protected ReEntryFlame EntryFlame;

        protected string MissionName;

        private double _trailTimer;
        private LaunchTrail _launchTrail;

        protected SpaceCraftBase(string craftDirectory, DVector2 position, DVector2 velocity, double propellantMass, string texturePath, ReEntryFlame entryFlame = null)
            : base(position, velocity, -Math.PI * 0.5)
        {
            CraftDirectory = craftDirectory;
            Children = new List<ISpaceCraft>();

            Texture = new Bitmap(texturePath);
            PropellantMass = propellantMass;

            EntryFlame = entryFlame;

            MissionName = craftDirectory.Substring(craftDirectory.LastIndexOf('\\') + 1);

            _launchTrail = new LaunchTrail();
        }

        public void InitializeController(EventManager eventManager)
        {
            string commandPath = Path.Combine(CraftDirectory, CommandFileName);

            if (File.Exists(commandPath))
            {
                List<CommandBase> commands = CommandManager.Load(commandPath);

                Controller = new CommandController(commands, this, eventManager);
            }
            else
            {
                Controller = new SimpleFlightController(this);
            }
        }

        /// <summary>
        /// Gets the down range distance along the equator of the parent planet.
        /// </summary>
        public double GetDownrangeDistance(DVector2 pointOfReference)
        {
            DVector2 pofOffset = GravitationalParent.Position - pointOfReference;
            pofOffset.Normalize();

            DVector2 spaceCraftOffset = GravitationalParent.Position - Position;
            spaceCraftOffset.Normalize();

            // Find angle between normal vectors
            double angle = Math.Acos(pofOffset.X*spaceCraftOffset.X + pofOffset.Y*spaceCraftOffset.Y);

            // Fixing wrapping around a circle
            if (angle > Math.PI)
            {
                angle -= Math.PI;
            }

            // Take the ratio of the angle to the full circumference
            double arcRatio = angle / (Math.PI * 2);

            return arcRatio * (2.0 * GravitationalParent.SurfaceRadius * Math.PI);
        }

        /// <summary>
        /// Stages the spacecraft according to its mounted angle by applying a normal force.
        /// </summary>
        public void Stage()
        {
            if (Children.Count > 0)
            {
                ISpaceCraft[] children = Children.ToArray();

                foreach (ISpaceCraft child in children)
                {
                    child.Stage();
                }
            }
            else if (Parent != null)
            {
                Parent.RemoveChild(this);
                Parent = null;

                // Simulate simple staging mechanism
                double sAngle = StageOffset.Angle();

                DVector2 stagingVector = DVector2.FromAngle(Pitch + sAngle + Math.PI * 0.5);

                AccelerationN += stagingVector * 1000;
            }
        }

        /// <summary>
        /// Deploys the fairing.
        /// </summary>
        public virtual void DeployFairing() { }

        /// <summary>
        /// Deploys the grid fins.
        /// </summary>
        public virtual void DeployGridFins() { }

        /// <summary>
        /// Deploys the landing legs.
        /// </summary>
        public virtual void DeployLandingLegs() { }

        public void SetThrottle(double throttle, int[] engineIds = null)
        {
            if (Engines.Length > 0)
            {
                // Throttle all engines if ids are specified
                if (engineIds == null)
                {
                    foreach (IEngine engine in Engines)
                    {
                        engine.AdjustThrottle(throttle);
                    }
                }
                else
                {
                    // Throttle only specified engine ids
                    foreach (int engineId in engineIds)
                    {
                        Engines[engineId].AdjustThrottle(throttle);
                    }
                }
            }
        }

        public void OffsetPitch(double offset)
        {
            Pitch += offset;

            foreach (ISpaceCraft child in Children)
            {
                child.OffsetPitch(offset);
            }
        }

        public void SetPitch(double pitch)
        {
            Pitch = pitch;

            foreach (ISpaceCraft child in Children)
            {
                child.SetPitch(pitch);
            }
        }

        public void OffsetRoll(double offset)
        {
            Roll += offset;

            foreach (ISpaceCraft child in Children)
            {
                child.OffsetRoll(offset);
            }
        }

        public void SetRoll(double roll)
        {
            Roll = roll;

            foreach (ISpaceCraft child in Children)
            {
                child.SetRoll(roll);
            }
        }

        public void OffsetYaw(double offset)
        {
            Yaw += offset;

            foreach (ISpaceCraft child in Children)
            {
                child.OffsetRoll(offset);
            }
        }

        public void SetYaw(double yaw)
        {
            Yaw = yaw;

            foreach (ISpaceCraft child in Children)
            {
                child.SetRoll(yaw);
            }
        }

        public void OffsetRelativePitch(double offset)
        {
            Pitch += offset;

            foreach (ISpaceCraft child in Children)
            {
                child.OffsetRelativePitch(offset);
            }
        }

        public void SetRelativePitch(double pitch)
        {
            double altitude = GetRelativeAltitude();
            if (altitude > GravitationalParent.AtmosphereHeight)
            {
                Pitch = GravitationalParent.Pitch + pitch;
            }
            else
            {
                DVector2 difference = GravitationalParent.Position - Position;
                difference.Normalize();
                var surfaceNormal = new DVector2(difference.Y, difference.X);
                double normal = surfaceNormal.Angle();
                if (double.IsNaN(normal))
                    Pitch = GravitationalParent.Pitch + pitch;
                else
                    Pitch = GravitationalParent.Pitch + pitch - normal;
            }

            foreach (ISpaceCraft child in Children)
            {
                child.SetRelativePitch(pitch);
            }
        }

        public void SetParent(ISpaceCraft craft)
        {
            Parent = craft;
        }

        public void AddChild(ISpaceCraft child)
        {
            Children.Add(child);
        }

        public void RemoveChild(ISpaceCraft child)
        {
            Children.Remove(child);
        }

        public virtual void UpdateAnimations(TimeStep timeStep)
        {
            // Only use re-entry flames for separated bodies
            if (EntryFlame != null && Children.Count == 0)
            {
                EntryFlame.Update(timeStep, Position, Velocity, Pitch, HeatingRate);
            }

            foreach (IEngine engine in Engines)
            {
                engine.Update(timeStep, IspMultiplier);
            }
        }

        public virtual void UpdateChildren(DVector2 position, DVector2 velocity)
        {
            Position = position - new DVector2(StageOffset.X*Math.Sin(Pitch) + StageOffset.Y*Math.Cos(Pitch),
                                               -StageOffset.X*Math.Cos(Pitch) + StageOffset.Y*Math.Sin(Pitch));
            Velocity.X = velocity.X;
            Velocity.Y = velocity.Y;

            MachNumber = velocity.Length() * 0.0029411764;

            foreach (ISpaceCraft child in Children)
            {
                child.UpdateChildren(Position, Velocity);
            }
        }

        public override void ResetAccelerations()
        {
            AccelerationD = DVector2.Zero;
            AccelerationL = DVector2.Zero;
            AccelerationN = DVector2.Zero;

            base.ResetAccelerations();
        }

        /// <summary>
        /// Gets the relative altitude of the spacecraft from it's parent's surface.
        /// </summary>
        public override double GetRelativeAltitude()
        {
            DVector2 difference = Position - GravitationalParent.Position;

            double totalDistance = difference.Length();

            return totalDistance - GravitationalParent.SurfaceRadius;
        }

        public override DVector2 GetRelativeAcceleration()
        {
            return AccelerationD + AccelerationL + AccelerationN;
        }

        /// <summary>
        /// Gets the relative velocity of the spacecraft. If the space craft is within the parent's
        /// atmosphere than the rotation of the planet is taken in account. Otherwise its a simple difference of velocities.
        /// </summary>
        public override DVector2 GetRelativeVelocity()
        {
            double altitude = GetRelativeAltitude();

            if (altitude > GravitationalParent.AtmosphereHeight)
            {
                return Velocity - GravitationalParent.Velocity;
            }

            DVector2 difference = GravitationalParent.Position - Position;
            difference.Normalize();

            var surfaceNormal = new DVector2(-difference.Y, difference.X);

            double altitudeFromCenter = altitude + GravitationalParent.SurfaceRadius;

            // Distance of circumference at this altitude ( c= 2r * pi )
            double pathCirumference = 2 * Math.PI * altitudeFromCenter;

            double rotationalSpeed = pathCirumference / GravitationalParent.RotationPeriod;

            return Velocity - (GravitationalParent.Velocity + surfaceNormal * rotationalSpeed);
        }

        public override double GetRelativePitch()
        {
            double altitude = GetRelativeAltitude();
            double relativePitch = Pitch - GravitationalParent.Pitch;
            if (altitude > GravitationalParent.AtmosphereHeight)
            {
                return relativePitch;
            }
            else
            {
                DVector2 difference = GravitationalParent.Position - Position;
                difference.Normalize();
                var surfaceNormal = new DVector2(difference.Y, difference.X);
                double normal = surfaceNormal.Angle();
                if (!double.IsNaN(normal))
                {
                    if (altitude > 0.1)
                        relativePitch += normal;
                    else
                        relativePitch = normal;
                }

                return relativePitch;
            }
        }

        public double GetAlpha()
        {
            double altitude = GetRelativeAltitude();
            if (altitude > GravitationalParent.AtmosphereHeight)
            {
                return Pitch - GravitationalParent.Pitch;
            }

            var alpha = 0.0;
            if (altitude > 0.1)
            {
                DVector2 difference = GravitationalParent.Position - Position;
                difference.Normalize();
                double vAngle = GetRelativeVelocity().Angle();
                alpha = Pitch - vAngle;

                double twoPi = Math.PI * 2;
                while (alpha > Math.PI)
                {
                    alpha -= twoPi;
                }

                while (alpha < -Math.PI)
                {
                    alpha += twoPi;
                }
            }

            return alpha;
        }

        public override void ResolveGravitation(IPhysicsBody other)
        {
            if (Parent != null) return;

            base.ResolveGravitation(other);
        }

        public void ResolveAtmopsherics(IMassiveBody body)
        {
            // Don't resolve drag for children
            if (Parent != null) return;

            DVector2 difference = body.Position - Position;

            double distance = difference.Length();

            difference.Normalize();

            double altitude = distance - body.SurfaceRadius;

            // The spacecraft is in the bodies atmopshere
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
                    OnGround = true;

                    var normal = new DVector2(-difference.X, -difference.Y);

                    Position = body.Position + normal*(body.SurfaceRadius);

                    Velocity = (body.Velocity + surfaceNormal*rotationalSpeed);

                    Pitch = normal.Angle();

                    AccelerationN.X = -AccelerationG.X;
                    AccelerationN.Y = -AccelerationG.Y;
                }
                else
                {
                    OnGround = false;
                }

                double atmosphericDensity = body.GetAtmosphericDensity(altitude);

                DVector2 relativeVelocity = (body.Velocity + surfaceNormal * rotationalSpeed) - Velocity;

                double velocity = relativeVelocity.Length();
                double velocityMagnitude = relativeVelocity.LengthSquared();

                if (velocityMagnitude > 0)
                {
                    double speed = relativeVelocity.Length();

                    // Heating
                    HeatingRate = 1.83e-4 * Math.Pow(speed, 3) * Math.Sqrt(atmosphericDensity / (Width * 0.5));

                    relativeVelocity.Normalize();

                    double formDragTerm = TotalFormDragCoefficient() * TotalFormDragArea();
                    double skinFrictionTerm = TotalSkinFrictionCoefficient() * TotalSkinFrictionArea();

                    double dragTerm = formDragTerm;
                    if (!double.IsNaN(skinFrictionTerm))
                        dragTerm += skinFrictionTerm;

                    double liftTerm = TotalLiftCoefficient() * TotalLiftArea();

                    // Form Drag ( Fd = 0.5pv^2dA )
                    // Skin friction ( Fs = 0.5CfpV^2S )
                    DVector2 drag = relativeVelocity * (0.5 * atmosphericDensity * velocityMagnitude * dragTerm);
                    DVector2 lift = relativeVelocity * (0.5 * atmosphericDensity * velocityMagnitude * liftTerm);

                    AccelerationD = drag / Mass;
                    DVector2 accelerationLift = lift / Mass;

                    double alpha = GetAlpha();
                    double halfPi = Math.PI / 2;
                    bool isRetrograde = alpha > halfPi || alpha < -halfPi;

                    if (isRetrograde)
                    {
                        AccelerationL.X -= accelerationLift.Y;
                        AccelerationL.Y += accelerationLift.X;
                    }
                    else
                    {
                        AccelerationL.X += accelerationLift.Y;
                        AccelerationL.Y -= accelerationLift.X;
                    }
                }
            }
            else
            {
                HeatingRate = 0;
            }
        }

        /// <summary>
        /// Recursively finds the total drag of all surfaces exposed to air in the spacecraft.
        /// </summary>
        private double TotalFormDragCoefficient()
        {
            double totalDragCoefficient = 0;
            AeroDynamicProperties props = GetAeroDynamicProperties;
            if (props.HasFlag(AeroDynamicProperties.ExposedToAirFlow) ||
                props.HasFlag(AeroDynamicProperties.ExtendsFineness))
            {
                totalDragCoefficient = FormDragCoefficient;
            }

            return GetChildDragCoefficient(Children, totalDragCoefficient);
        }

        private double GetChildDragCoefficient(List<ISpaceCraft> children, double totalDragCoefficient)
        {
            foreach (SpaceCraftBase child in children)
            {
                AeroDynamicProperties props = child.GetAeroDynamicProperties;
                if (props.HasFlag(AeroDynamicProperties.ExposedToAirFlow))
                {
                    if (child.FormDragCoefficient > totalDragCoefficient)
                        totalDragCoefficient = child.FormDragCoefficient;
                }
                else if (props.HasFlag(AeroDynamicProperties.ExtendsFineness))
                {
                    totalDragCoefficient *= child.FormDragCoefficient;
                }
                else if (props.HasFlag(AeroDynamicProperties.ExtendsCrossSection))
                {
                    totalDragCoefficient = (totalDragCoefficient + child.FormDragCoefficient) / 2;
                }

                totalDragCoefficient = GetChildDragCoefficient(child.Children, totalDragCoefficient);
            }

            return totalDragCoefficient;
        }

        /// <summary>
        /// Recursively finds the total drag area of all surfaces exposed to air in the spacecraft.
        /// </summary>
        private double TotalFormDragArea()
        {
            double totalFormDragArea = 0;
            AeroDynamicProperties props = GetAeroDynamicProperties;
            if (props.HasFlag(AeroDynamicProperties.ExposedToAirFlow) ||
                props.HasFlag(AeroDynamicProperties.ExtendsFineness))
            {
                totalFormDragArea = CrossSectionalArea;
            }

            return GetChildFormDragArea(Children, totalFormDragArea);
        }

        private double GetChildFormDragArea(List<ISpaceCraft> children, double totalFormDragArea)
        {
            foreach (SpaceCraftBase child in children)
            {
                AeroDynamicProperties props = child.GetAeroDynamicProperties;
                if (props.HasFlag(AeroDynamicProperties.ExposedToAirFlow))
                {
                    if (child.CrossSectionalArea > totalFormDragArea)
                        totalFormDragArea = child.CrossSectionalArea;
                }
                else if (props.HasFlag(AeroDynamicProperties.ExtendsCrossSection))
                {
                    totalFormDragArea += child.CrossSectionalArea;
                }

                totalFormDragArea = GetChildFormDragArea(child.Children, totalFormDragArea);
            }

            return totalFormDragArea;
        }

        private double HeatingRadius()
        {
            double radius = 0;
            AeroDynamicProperties props = GetAeroDynamicProperties;
            if (!props.Equals(AeroDynamicProperties.None))
                radius = Width / 2;

            return ChildHeatingRadius(Children, radius);
        }

        private double ChildHeatingRadius(List<ISpaceCraft> children, double radius)
        {
            foreach (SpaceCraftBase child in children)
            {
                AeroDynamicProperties props = child.GetAeroDynamicProperties;
                if (!props.Equals(AeroDynamicProperties.None))
                {
                    if (child.Width / 2 > radius)
                        radius = child.Width / 2;
                }

                radius = ChildHeatingRadius(child.Children, radius);
            }

            return radius;
        }

        /// <summary>
        /// Recursively finds the total skin friction due to all surfaces exposed to air in the spacecraft.
        /// </summary>
        private double TotalSkinFrictionCoefficient()
        {
            double totalSkinFrictionCoefficient = 0;

            AeroDynamicProperties props = GetAeroDynamicProperties;
            if (props.HasFlag(AeroDynamicProperties.ExposedToAirFlow)
                || props.HasFlag(AeroDynamicProperties.ExtendsFineness)
                || props.HasFlag(AeroDynamicProperties.ExtendsCrossSection))
            {
                totalSkinFrictionCoefficient = SkinFrictionCoefficient;
            }

            foreach (SpaceCraftBase child in Children)
            {
                totalSkinFrictionCoefficient += child.TotalSkinFrictionCoefficient();
            }

            return totalSkinFrictionCoefficient;
        }

        /// <summary>
        /// Recursively finds the total drag area of all surfaces exposed to air in the spacecraft.
        /// </summary>
        private double TotalSkinFrictionArea()
        {
            double totalSkinFrictionArea = 0;
            AeroDynamicProperties props = GetAeroDynamicProperties;
            if (props.HasFlag(AeroDynamicProperties.ExposedToAirFlow)
                || props.HasFlag(AeroDynamicProperties.ExtendsFineness)
                || props.HasFlag(AeroDynamicProperties.ExtendsCrossSection))
            {
                totalSkinFrictionArea += ExposedSurfaceArea;
            }

            foreach (SpaceCraftBase child in Children)
            {
                totalSkinFrictionArea += child.TotalSkinFrictionArea();
            }

            return totalSkinFrictionArea;
        }

        /// <summary>
        /// Recursively finds the total lift of all surfaces exposed to air in the spacecraft.
        /// </summary>
        private double TotalLiftCoefficient()
        {
            double totalLiftCoefficient = 0;
            AeroDynamicProperties props = GetAeroDynamicProperties;
            if (props.HasFlag(AeroDynamicProperties.ExposedToAirFlow)
                || props.HasFlag(AeroDynamicProperties.ExtendsFineness)
                || props.HasFlag(AeroDynamicProperties.ExtendsCrossSection))
            {
                totalLiftCoefficient = LiftCoefficient;
            }

            return GetMaxChildLiftCoefficient(Children, totalLiftCoefficient);
        }

        private double GetMaxChildLiftCoefficient(List<ISpaceCraft> children, double totalLiftCoefficient)
        {
            foreach (SpaceCraftBase child in children)
            {
                AeroDynamicProperties props = child.GetAeroDynamicProperties;
                if (props.HasFlag(AeroDynamicProperties.ExposedToAirFlow))
                {
                    if (Math.Abs(child.LiftCoefficient) > Math.Abs(totalLiftCoefficient))
                        totalLiftCoefficient = child.LiftCoefficient;
                }
                else if (props.HasFlag(AeroDynamicProperties.ExtendsFineness)
                         || props.HasFlag(AeroDynamicProperties.ExtendsCrossSection))
                {
                    totalLiftCoefficient += child.LiftCoefficient;
                }

                totalLiftCoefficient = GetMaxChildLiftCoefficient(child.Children, totalLiftCoefficient);
            }

            return totalLiftCoefficient;
        }

        /// <summary>
        /// Recursively finds the total lift area of all surfaces exposed to air in the spacecraft.
        /// </summary>
        private double TotalLiftArea()
        {
            double totalLiftArea = 0;
            AeroDynamicProperties props = GetAeroDynamicProperties;
            if (props.HasFlag(AeroDynamicProperties.ExposedToAirFlow)
                || props.HasFlag(AeroDynamicProperties.ExtendsFineness)
                || props.HasFlag(AeroDynamicProperties.ExtendsCrossSection))
            {
                totalLiftArea = LiftingSurfaceArea;
            }

            return GetChildLiftArea(Children, totalLiftArea);
        }

        private double GetChildLiftArea(List<ISpaceCraft> children, double totalLiftArea)
        {
            foreach (SpaceCraftBase child in children)
            {
                AeroDynamicProperties props = child.GetAeroDynamicProperties;
                if (props.HasFlag(AeroDynamicProperties.ExposedToAirFlow))
                {
                    if (child.LiftingSurfaceArea > totalLiftArea)
                        totalLiftArea = child.LiftingSurfaceArea;
                }
                else if (props.HasFlag(AeroDynamicProperties.ExtendsFineness)
                         || props.HasFlag(AeroDynamicProperties.ExtendsCrossSection))
                {
                    totalLiftArea += child.LiftingSurfaceArea;
                }

                totalLiftArea = GetChildLiftArea(child.Children, totalLiftArea);
            }

            return totalLiftArea;
        }

        /// <summary>
        /// Recursively updates the engines in the spacecraft and updates propellant mass.
        /// </summary>
        private void UpdateEngines(double dt)
        {
            Thrust = 0;

            if (Engines.Length > 0 && PropellantMass > 0)
            {
                foreach (IEngine engine in Engines)
                {
                    if (!engine.IsActive) continue;

                    Thrust += engine.Thrust(IspMultiplier);

                    PropellantMass = Math.Max(0, PropellantMass - engine.MassFlowRate() * dt);
                }
            }

            foreach (SpaceCraftBase child in Children)
            {
                child.UpdateEngines(dt);

                Thrust += child.Thrust;
            }
        }

        /// <summary>
        /// Updates the spacecraft and it's children.
        /// </summary>
        public override void Update(double dt)
        {
            Controller.Update(dt);

            double altitude = GetRelativeAltitude();

            IspMultiplier = GravitationalParent.GetIspMultiplier(altitude);

            if (Parent == null)
            {
                UpdateEngines(dt);

                if (Thrust > 0)
                {
                    var thrustVector = new DVector2(Math.Cos(Pitch), Math.Sin(Pitch));

                    AccelerationN += (thrustVector * Thrust) / Mass;
                }

                // Integrate acceleration
                Velocity += (AccelerationG * dt);
                Velocity += (AccelerationD * dt);
                Velocity += (AccelerationN * dt);
                Velocity += (AccelerationL * dt);

                // Re-normalize FTL scenarios
                if (Velocity.LengthSquared() > FlightGlobals.SPEED_LIGHT_SQUARED)
                {
                    Velocity.Normalize();

                    Velocity *= FlightGlobals.SPEED_OF_LIGHT;
                }

                // Integrate velocity
                Position += (Velocity * dt);

                MachNumber = GetRelativeVelocity().Length() * 0.0029411764;

                foreach (ISpaceCraft child in Children)
                {
                    child.UpdateChildren(Position, Velocity);
                }

                _trailTimer += dt;

                if (_trailTimer > 1 && !OnGround)
                {
                    _launchTrail.AddPoint(Position, GravitationalParent, Throttle > 0);
                    _trailTimer = 0;
                }
            }
        }

        // Return an inflated bounding box for better drawing results
        public virtual RectangleD ComputeBoundingBox()
        {
            if (Width > Height)
            {
                return new RectangleD(Position.X - Width, Position.Y - Width, Width * 2, Width * 2);
            }

            return new RectangleD(Position.X - Height, Position.Y - Height, Height * 2, Height * 2);
        }

        public double Visibility(RectangleD cameraBounds)
        {
            double distanceRatio = TotalHeight / cameraBounds.Width;

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

        /// <summary>
        /// Renders the space craft at it's correct scale and rotation according to the camera.
        /// The engines are rendered first and then the space craft body.
        /// </summary>
        public void RenderGdi(Graphics graphics, RectangleD cameraBounds)
        {
            RectangleF screenBounds = RenderUtils.ComputeBoundingBox(Position, cameraBounds, Width, Height);

            // Saftey
            if (screenBounds.Width > RenderUtils.ScreenWidth * 500) return;

            RenderBelow(graphics, cameraBounds);

            RenderShip(graphics, cameraBounds, screenBounds);

            RenderAbove(graphics, cameraBounds);
        }

        public void RenderLaunchTrail(Graphics graphics, RectangleD cameraBounds)
        {
            _launchTrail.Draw(graphics, cameraBounds, GravitationalParent);
        }

        protected virtual void RenderBelow(Graphics graphics, RectangleD cameraBounds)
        {
            foreach (IEngine engine in Engines)
            {
                engine.Draw(graphics, cameraBounds);
            }
        }

        protected virtual void RenderShip(Graphics graphics, RectangleD cameraBounds, RectangleF screenBounds)
        {
            double drawingRotation = Pitch + Math.PI * 0.5;

            var offset = new PointF(screenBounds.X + screenBounds.Width * 0.5f,
                                    screenBounds.Y + screenBounds.Height * 0.5f);

            graphics.TranslateTransform(offset.X, offset.Y);

            graphics.RotateTransform((float)(drawingRotation * 180 / Math.PI));

            graphics.TranslateTransform(-offset.X, -offset.Y);

            graphics.DrawImage(Texture, screenBounds.X, screenBounds.Y, screenBounds.Width, screenBounds.Height);

            graphics.ResetTransform();
        }

        protected virtual void RenderAbove(Graphics graphics, RectangleD cameraBounds)
        {
            if (EntryFlame != null)
            {
                EntryFlame.Draw(graphics, cameraBounds);
            }
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}]", CraftName, MissionName);
        }
    }
}
