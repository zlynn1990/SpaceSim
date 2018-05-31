using System.Drawing;
using SpaceSim.Kernels;
using SpaceSim.Orbits;

namespace SpaceSim.SolarSystem.Planets
{
    class Saturn : MassiveBodyBase
    {
        public override double Mass
        {
            get { return 5.6836e26; }
        }

        public override double SurfaceRadius
        {
            get { return SymbolKernel.SATURN_RADIUS; }
        }

        public override double AtmosphereHeight
        {
            get { return SymbolKernel.SATURN_ATMOSPHERE; }
        }

        public override double RotationRate
        {
            get { return -1.70553347100423085692868e-4; }
        }

        public override double RotationPeriod
        {
            get { return 36840; }
        }

        public override Color IconColor { get { return Color.Gold; } }
        public override Color IconAtmopshereColor { get { return Color.Goldenrod; } }

        public override double BoundingRadius
        {
            get { return SurfaceRadius + SymbolKernel.SATURN_RING_END; }
        }

        public Saturn()
            : base(OrbitHelper.FromJplEphemeris(-3.667006922888632E+08, -1.455132410758398E+09),
                   OrbitHelper.FromJplEphemeris(8.836135826121589E+00, -2.389892378353466E+00), new SaturnKernel())
        {
        }

        public override string ToString()
        {
            return "Saturn";
        }
    }
}
