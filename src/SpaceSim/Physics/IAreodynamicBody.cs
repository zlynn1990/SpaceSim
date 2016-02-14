using SpaceSim.SolarSystem;
using VectorMath;

namespace SpaceSim.Physics
{
    interface IAreodynamicBody : IPhysicsBody
    {
        bool ExposedToAirFlow { get; }

        double DragCoefficient { get; }
        double CrossSectionalArea { get; }

        DVector2 AccelerationD { get; }
        DVector2 AccelerationN { get; }

        void ResolveAtmopsherics(IMassiveBody body);
    }
}
