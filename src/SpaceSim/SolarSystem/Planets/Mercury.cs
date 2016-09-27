using System.Drawing;
using SpaceSim.Kernels;
using SpaceSim.Orbits;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.SolarSystem.Planets
{
    class Mercury : MassiveBodyBase
    {
        public override double Mass
        {
            get { return 4.8676e24; }
        }

        public override double SurfaceRadius
        {
            get { return SymbolKernel.MERCURY_RADIUS; }
        }

        public override double AtmosphereHeight
        {
            get { return 6.371e6; }
        }

        public override double RotationRate
        {
            get { return -1.2325771553632271023472392432829e-6; }
        }

        public override double RotationPeriod
        {
            get { return 5097600; }
        }

        public override Color IconColor { get { return Color.Gray; } }
        public override Color IconAtmopshereColor { get { return Color.White; } }

        public Mercury()
            : base(OrbitHelper.FromJplEphemeris(5.246226302537809E+07, -2.527033713925028E+07),
                   OrbitHelper.FromJplEphemeris(1.211166887870701E+01, 4.586947862338018E+01), new MercuryKernel())
        {
        }

        public override double GetAtmosphericDensity(double height)
        {
            return 0.001;
        }

        public override string ToString()
        {
            return "Mercury";
        }
    }
}
