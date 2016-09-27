using SpaceSim.SolarSystem;
using VectorMath;

namespace SpaceSim.Physics
{
    interface IGravitationalBody : IPhysicsBody
    {
        IMassiveBody GravitationalParent { get; }

        DVector2 AccelerationG { get; }

        double Apogee { get; }
        double Perigee { get; }

        bool InOrbit { get; }

        void ResetAccelerations();
        void ResetOrientation();

        void ResolveGravitation(IPhysicsBody other);

        double GetRelativePitch();
        double GetRelativeAltitude();

        DVector2 GetRelativeVelocity();
        DVector2 GetRelativeAcceleration();

        void SetGravitationalParent(IMassiveBody parent);

        void FixedUpdate(TimeStep timeStep);
    }
}
