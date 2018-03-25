using System.Drawing;
using SpaceSim.Kernels;
using SpaceSim.Orbits;

namespace SpaceSim.SolarSystem.Planets
{
    class Jupiter : MassiveBodyBase
    {
        public override string ApoapsisName { get { return "Apozene"; } }
        public override string PeriapsisName { get { return "Perizene"; } }

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
            : base(OrbitHelper.FromJplEphemeris(-8.140496172394816E+08, -2.921133413936896E+07),
                   OrbitHelper.FromJplEphemeris(3.165828961330318E-01, -1.243819908039714E+01), new JupiterKernel())
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
