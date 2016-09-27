using VectorMath;

namespace SpaceSim.Particles
{
    class Particle
    {
        public bool IsActive;

        public DVector2 Position;

        public DVector2 Velocity;

        public double Age;
        public double MaxAge;
    }
}
