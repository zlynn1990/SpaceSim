using System;
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

        double GetIspMultiplier(double altitude);
        double GetAtmosphericDensity(double altitude);
        double GetAtmosphericViscosity(double altitude);

        double GetSurfaceAngle(DateTime localTime, IMassiveBody sun);
    }
}
