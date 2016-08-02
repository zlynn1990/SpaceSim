using SpaceSim.SolarSystem;
using VectorMath;

namespace SpaceSim.Physics
{
    interface IGravitationalBody : IPhysicsBody
    {
        IMassiveBody GravitationalParent { get; }

        DVector2 AccelerationG { get; }

        void ResetAccelerations();

        void ResolveGravitation(IPhysicsBody other);

        double GetRelativePitch();
        double GetRelativeAltitude();

        DVector2 GetRelativeVelocity();
        DVector2 GetRelativeAcceleration();

        void SetGravitationalParent(IMassiveBody parent);
    }
}
