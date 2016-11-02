using System.Drawing;
using SpaceSim.Drawing;
using SpaceSim.Kernels;
using SpaceSim.Orbits;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.SolarSystem.Planets
{
    class Venus : MassiveBodyBase
    {
        public override double Mass
        {
            get { return 4.8675e24; }
        }

        public override double SurfaceRadius
        {
            get { return SymbolKernel.VENUS_RADIUS; }
        }

        public override double AtmosphereHeight
        {
            get { return SymbolKernel.VENUS_ATMOSPHERE; }
        }

        public override double RotationRate
        {
            get { return -6.228869564576e-7; }
        }

        public override double RotationPeriod
        {
            get { return 10087200; }
        }

        public override Color IconColor { get { return Color.Yellow; } }
        public override Color IconAtmopshereColor { get { return Color.LightYellow; } }

        public Venus()
            : base(OrbitHelper.GetPosition(1.07477e11, 2.29568357, DVector2.Zero),
                   OrbitHelper.GetVelocity(1.07477e11, 2.29568357, -3.526e4, DVector2.Zero), new VenusKernel())
        {
        }

        public override double GetAtmosphericDensity(double height)
        {
            return base.GetAtmosphericDensity(height) * 67.0;
        }

        public override string ToString()
        {
            return "Venus";
        }
    }
}
