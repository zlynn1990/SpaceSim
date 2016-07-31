using SpaceSim.SolarSystem;
using VectorMath;

namespace SpaceSim.Physics
{
    public enum AeroDynamicProperties
    {
        None,
        ExposedToAirFlow,
        ExtendsFineness,
        ExtendsCrossSection
    }

    interface IAerodynamicBody : IPhysicsBody
    {
        AeroDynamicProperties GetAeroDynamicProperties { get; }

        double HeatingRate { get; }

        double FormDragCoefficient { get; }
        double CrossSectionalArea { get; }
        double SkinFrictionCoefficient { get; }
        double ExposedSurfaceArea { get; }
        double LiftCoefficient { get; }
        double LiftingSurfaceArea { get; }

        DVector2 AccelerationD { get; }
        DVector2 AccelerationN { get; }

        void ResolveAtmopsherics(IMassiveBody body);
    }
}
