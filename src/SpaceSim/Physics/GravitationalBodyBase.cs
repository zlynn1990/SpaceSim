using System.Drawing;
using SpaceSim.Common;
using SpaceSim.Drawing;
using SpaceSim.Orbits;
using SpaceSim.SolarSystem;
using VectorMath;

namespace SpaceSim.Physics
{
    abstract class GravitationalBodyBase : IGravitationalBody, IGdiRenderable, IMapRenderable
    {
        public IMassiveBody GravitationalParent { get; protected set; }

        public DVector2 Position { get; protected set; }
        public DVector2 Velocity { get; protected set; }

        public abstract double Mass { get; }
        public double Pitch { get; protected set; }

        public DVector2 AccelerationG { get; protected set; }

        public double Apoapsis { get { return OrbitTrace.Apogee; } }
        public double Periapsis { get { return OrbitTrace.Perigee; } }

        public bool InOrbit
        {
            get { return Apoapsis > GravitationalParent.AtmosphereHeight * 0.5 && Periapsis > GravitationalParent.AtmosphereHeight * 0.5 && GetRelativeVelocity().Length() > 1; }
        }

        public abstract Color IconColor { get; }

        protected OrbitTrace OrbitTrace;

        protected GravitationalBodyBase(DVector2 position, DVector2 velocity, double pitch)
        {
            Position = position;
            Velocity = velocity;

            Pitch = pitch;

            OrbitTrace = new OrbitTrace();
        }

        public virtual void ResetAccelerations()
        {
            AccelerationG = DVector2.Zero;
        }

        public void ResetOrientation()
        {
            Pitch = 0;
        }

        public virtual void ResolveGravitation(IPhysicsBody other)
        {
            DVector2 difference = other.Position - Position;

            double r2 = difference.LengthSquared();

            double massDistanceRatio = other.Mass / r2;

            // Ignore the force, the planet is too far away to matter
            if (massDistanceRatio < 2500)
            {
                return;
            }

            difference.Normalize();

            // Gravitation ( aG = G m1 / r^2 )
            AccelerationG += difference * Constants.GravitationConstant * massDistanceRatio;
        }

        public virtual double GetRelativeAltitude()
        {
            DVector2 difference = Position - GravitationalParent.Position;

            return difference.Length();
        }

        public virtual DVector2 GetRelativeAcceleration()
        {
            return DVector2.Zero;
        }

        public virtual DVector2 GetRelativeVelocity()
        {
            return Velocity - GravitationalParent.Velocity;
        }

        public virtual double GetRelativePitch()
        {
            return Pitch - GravitationalParent.Pitch;
        }

        public abstract double Visibility(RectangleD cameraBounds);
        public abstract RectangleD ComputeBoundingBox();

        public abstract void Update(double dt);

        public void SetGravitationalParent(IMassiveBody parent)
        {
            GravitationalParent = parent;
        }

        public abstract void FixedUpdate(TimeStep timeStep);

        public virtual void RenderGdi(Graphics graphics, Camera camera)
        {
            OrbitTrace.Draw(graphics, camera, this);
        }
    }
}