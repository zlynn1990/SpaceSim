using SpaceSim.SolarSystem;
using VectorMath;

namespace SpaceSim.Physics
{
    abstract class GravitationalBodyBase : IGravitationalBody
    {
        public IMassiveBody GravitationalParent { get; protected set; }

        public DVector2 Position { get; protected set; }
        public DVector2 Velocity { get; protected set; }

        public abstract double Mass { get; }
        public double Rotation { get; protected set; }

        public DVector2 AccelerationG { get; protected set; }

        protected GravitationalBodyBase(DVector2 position, DVector2 velocity, double rotation)
        {
            Position = position;
            Velocity = velocity;

            Rotation = rotation;
        }

        public virtual void ResetAccelerations()
        {
            AccelerationG = DVector2.Zero;
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
            AccelerationG += difference * FlightGlobals.GRAVITATION_CONSTANT * massDistanceRatio;
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

        public abstract void Update(double dt);

        public void SetGravitationalParent(IMassiveBody parent)
        {
            GravitationalParent = parent;
        }
    }
}
