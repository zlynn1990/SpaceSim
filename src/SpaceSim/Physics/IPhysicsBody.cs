using VectorMath;

namespace SpaceSim.Physics
{
    interface IPhysicsBody
    {
        DVector2 Position { get; }
        DVector2 Velocity { get; }

        double Mass { get; }
        double Pitch { get; }

        void Update(double dt);
    }
}
