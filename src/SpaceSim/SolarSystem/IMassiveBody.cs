using System.Drawing;
using SpaceSim.Physics;

namespace SpaceSim.SolarSystem
{
    interface IMassiveBody : IGravitationalBody
    {
        Color IconAtmopshereColor { get; }

        double SurfaceRadius { get; }
        double AtmosphereHeight { get; }

        double RotationRate { get; }
        double RotationPeriod { get; }

        double GetSurfaceGravity();

        double GetIspMultiplier(double height);
        double GetAtmosphericDensity(double height);
    }
}
