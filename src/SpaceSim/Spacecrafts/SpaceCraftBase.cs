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
    abstract class SpaceCraftBase : GravitationalBodyBase, IAreodynamicBody, ISpaceCraft, IGdiRenderable
    {
        public virtual string ShortName { get { return ToString(); } }

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

        public abstract bool ExposedToAirFlow { get; }
        public abstract double DragCoefficient { get; }

        public virtual double CrossSectionalArea { get { return Math.PI * (Width * 0.5) * (Width * 0.5); } }
        public virtual double SurfaceArea { get { return Math.PI * Width * Height; } }

        public DVector2 AccelerationD { get; protected set; }
        public DVector2 AccelerationN { get; protected set; }

        public abstract Color IconColor { get; }

        protected Bitmap Texture;
        protected DVector2 StageOffset;

        protected double MachNumber;
        protected double IspMultiplier;

        protected ReEntryFlame EntryFlame;

        protected SpaceCraftBase(DVector2 position, DVector2 velocity, double propellantMass, string texturePath, ReEntryFlame entryFlame = null)
            : base(position, velocity, -Math.PI * 0.5)
        {
            Children = new List<ISpaceCraft>();

            Texture = new Bitmap(texturePath);
            PropellantMass = propellantMass;

            EntryFlame = entryFlame;
        }

        public void InitializeController(string craftDirectory, EventManager eventManager)
        {
            string commandPath = Path.Combine(craftDirectory, CommandFileName);

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

                DVector2 stagingVector = DVector2.FromAngle(Rotation + sAngle + Math.PI * 0.5);

                AccelerationN += stagingVector * 1000;
            }
        }

        /// <summary>
        /// Deploys the fairing.
        /// </summary>
        public virtual void DeployFairing() { }

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

        public void OffsetRotation(double offset)
        {
            Rotation += offset;

            foreach (ISpaceCraft child in Children)
            {
                child.OffsetRotation(offset);
            }
        }

        public void SetRotation(double rotation)
        {
            Rotation = rotation;

            foreach (ISpaceCraft child in Children)
            {
                child.SetRotation(rotation);
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
                EntryFlame.Update(timeStep, Position, Velocity, Rotation, HeatingRate);
            }

            foreach (IEngine engine in Engines)
            {
                engine.Update(timeStep, IspMultiplier);
            }
        }

        public virtual void UpdateChildren(DVector2 position, DVector2 velocity)
        {
            Position = position - new DVector2(StageOffset.X*Math.Sin(Rotation) + StageOffset.Y*Math.Cos(Rotation),
                                               -StageOffset.X*Math.Cos(Rotation) + StageOffset.Y*Math.Sin(Rotation));
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
            return AccelerationD + AccelerationN;
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
                    var normal = new DVector2(-difference.X, -difference.Y);

                    Position = body.Position + normal * (body.SurfaceRadius);

                    Velocity = (body.Velocity + surfaceNormal * rotationalSpeed);

                    Rotation = normal.Angle();

                    AccelerationN.X = -AccelerationG.X;
                    AccelerationN.Y = -AccelerationG.Y;
                }

                double atmosphericDensity = body.GetAtmosphericDensity(altitude);

                DVector2 relativeVelocity = (body.Velocity + surfaceNormal * rotationalSpeed) - Velocity;

                double velocity = relativeVelocity.Length();
                double velocitySquared = relativeVelocity.LengthSquared();

                if (velocitySquared > 0)
                {
                    double speed = relativeVelocity.Length();

                    // Heating
                    HeatingRate = 1.83e-4 * Math.Pow(speed, 3) * Math.Sqrt(atmosphericDensity / (Width * 0.5));

                    relativeVelocity.Normalize();

                    double dragTerm = TotalDragCoefficient() * TotalDragArea();

                    // Drag ( Fd = 0.5pv^2dA )
                    DVector2 dragForce = relativeVelocity * (0.5 * atmosphericDensity * velocitySquared * dragTerm);

                    AccelerationD += dragForce / Mass;

                    double reynoldsNumber = (velocity * Height) / body.GetAtmosphericViscosity(altitude);

                    double frictionCoefficient = 0.455 / Math.Pow(Math.Log10(reynoldsNumber), 2.58);

                    double frictionTerm = frictionCoefficient*TotalSurfaceArea();

                    // Skin friction ( Fs = 0.5CfpV^2S )
                    DVector2 skinFriction = relativeVelocity * (0.5 * atmosphericDensity * velocitySquared * frictionTerm);

                    AccelerationD += skinFriction / Mass;
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
        private double TotalDragCoefficient()
        {
            double totalDragCoefficient = 0;

            if (ExposedToAirFlow)
            {
                totalDragCoefficient += DragCoefficient;
            }

            foreach (SpaceCraftBase child in Children)
            {
                totalDragCoefficient += child.TotalDragCoefficient();
            }

            return totalDragCoefficient;
        }

        /// <summary>
        /// Recursively finds the total drag area of all surfaces exposed to air in the spacecraft.
        /// </summary>
        private double TotalDragArea()
        {
            double totalDragArea = 0;

            if (ExposedToAirFlow)
            {
                totalDragArea += CrossSectionalArea;
            }

            foreach (SpaceCraftBase child in Children)
            {
                totalDragArea += child.TotalDragArea();
            }

            return totalDragArea;
        }

        /// <summary>
        /// Recursively finds the total surface area of the spacecraft.
        /// </summary>
        private double TotalSurfaceArea()
        {
            double totalSuraceArea = SurfaceArea;

            foreach (SpaceCraftBase child in Children)
            {
                totalSuraceArea += child.TotalSurfaceArea();
            }

            return totalSuraceArea;
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
                    var thrustVector = new DVector2(Math.Cos(Rotation), Math.Sin(Rotation));

                    AccelerationN += (thrustVector * Thrust) / Mass;
                }

                // Integrate acceleration
                Velocity += (AccelerationG * dt);
                Velocity += (AccelerationD * dt);
                Velocity += (AccelerationN * dt);

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
            }
        }

        public virtual RectangleD ComputeBoundingBox()
        {
            if (Width > Height)
            {
                return new RectangleD(Position.X - Width * 0.5, Position.Y - Width * 0.5, Width, Width);
            }

            return new RectangleD(Position.X - Height * 0.5, Position.Y - Height * 0.5, Height, Height);
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
        public virtual void RenderGdi(Graphics graphics, RectangleD cameraBounds)
        {
            RectangleF screenBounds = RenderUtils.ComputeBoundingBox(Position, cameraBounds, Width, Height);

            // Saftey
            if (screenBounds.Width > RenderUtils.ScreenWidth * 500) return;

            RenderAnimations(graphics, cameraBounds);

            RenderShip(graphics, cameraBounds, screenBounds);
        }

        protected virtual void RenderAnimations(Graphics graphics, RectangleD cameraBounds)
        {
            foreach (IEngine engine in Engines)
            {
                engine.Draw(graphics, cameraBounds);
            }

            if (EntryFlame != null)
            {
                EntryFlame.Draw(graphics, cameraBounds);   
            }
        }

        protected virtual void RenderShip(Graphics graphics, RectangleD cameraBounds, RectangleF screenBounds)
        {
            double drawingRotation = Rotation + Math.PI * 0.5;

            var offset = new PointF(screenBounds.X + screenBounds.Width * 0.5f,
                                    screenBounds.Y + screenBounds.Height * 0.5f);

            graphics.TranslateTransform(offset.X, offset.Y);

            graphics.RotateTransform((float)(drawingRotation * 180 / Math.PI));

            graphics.TranslateTransform(-offset.X, -offset.Y);

            graphics.DrawImage(Texture, screenBounds.X, screenBounds.Y, screenBounds.Width, screenBounds.Height);

            graphics.ResetTransform();
        }
    }
}
