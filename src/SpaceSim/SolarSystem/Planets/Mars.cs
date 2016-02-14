using System.Drawing;
using SpaceSim.Kernels;
using SpaceSim.Orbits;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.SolarSystem.Planets
{
    class Mars : MassiveBodyBase
    {
        public override double Mass
        {
            get { return 0.64174e24; }
        }

        public override double SurfaceRadius
        {
            get { return SymbolKernel.MARS_RADIUS; }
        }

        public override double AtmosphereHeight
        {
            get { return SymbolKernel.MARS_ATMOSPHERE; }
        }

        public override double RotationRate
        {
            get { return -7.077658089896e-5; }
        }

        public override double RotationPeriod
        {
            get { return 88774.92; }
        }

        public override Color IconColor { get { return Color.Orange; } }
        public override Color IconAtmopshereColor { get { return Color.LightSalmon; } }

        public Mars()
            : base(OrbitHelper.GetPosition(2.067e11, 5.865019079167, DVector2.Zero),
                   OrbitHelper.GetVelocity(2.067e11, 5.865019079167, -2.65e4, DVector2.Zero), new MarsKernel())
        {
        }

        public override double GetAtmosphericDensity(double height)
        {
            return base.GetAtmosphericDensity(height) * 0.02;
        }

        public override string ToString()
        {
            return "Mars";
        }
    }
}
