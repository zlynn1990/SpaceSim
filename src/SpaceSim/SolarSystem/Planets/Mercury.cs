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
            : base(OrbitHelper.GetPosition(4.6001200e10, 1.35187, DVector2.Zero),
                   OrbitHelper.GetVelocity(4.6001200e10, 1.35187, -5.898e4, DVector2.Zero), new MercuryKernel())
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
