using System.Drawing;
using SpaceSim.Kernels;
using SpaceSim.Orbits;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.SolarSystem.Planets
{
    class Jupiter : MassiveBodyBase
    {
        public override double Mass
        {
            get { return 1.8986e27; }
        }

        public override double SurfaceRadius
        {
            get { return SymbolKernel.JUPITER_RADIUS; }
        }

        public override double AtmosphereHeight
        {
            get { return SymbolKernel.JUPITER_ATMOSPHERE; }
        }

        public override double RotationRate
        {
            get { return -1.7809482163207444662486640494782e-4; }
        }

        public override double RotationPeriod
        {
            get { return 35280; }
        }

        public override Color IconColor { get { return Color.DarkRed; } }
        public override Color IconAtmopshereColor { get { return Color.IndianRed; } }

        public Jupiter()
            : base(OrbitHelper.GetPosition(7.405736e11, 0.25750325984, DVector2.Zero),
                   OrbitHelper.GetVelocity(7.405736e11, 0.25750325984, -1.372e4, DVector2.Zero), new JupiterKernel())
        {
        }

        public override double GetAtmosphericDensity(double height)
        {
            return base.GetAtmosphericDensity(height) * 1000;
        }

        public override string ToString()
        {
            return "Jupiter";
        }
    }
}
