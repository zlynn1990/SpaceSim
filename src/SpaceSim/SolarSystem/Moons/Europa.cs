using System.Drawing;
using SpaceSim.Kernels;
using SpaceSim.Orbits;
using VectorMath;

namespace SpaceSim.SolarSystem.Moons
{
    class Europa : MassiveBodyBase
    {
        public override double Mass
        {
            get { return 4.799844e22; }
        }

        public override double SurfaceRadius
        {
            get { return SymbolKernel.EUROPA_RADIUS; }
        }

        public override double AtmosphereHeight
        {
            get { return 6.371e5; }
        }

        public override double RotationRate
        {
            get { return 2.0777729190408685439567747243912e-5; }
        }

        public override double RotationPeriod
        {
            get { return 302400; }
        }

        public override Color IconColor { get { return Color.LightBlue; } }
        public override Color IconAtmopshereColor { get { return Color.AliceBlue; } }

        public Europa(DVector2 parentPositon, DVector2 parentVelocity)
            : base(OrbitHelper.FromJplEphemeris(6.156481591252791E+05, 2.501017249378511E+05) + parentPositon,
                   OrbitHelper.FromJplEphemeris(-5.263255140302287E+00, 1.282255171501272E+01) + parentVelocity, new EuropaKernel())
        {
        }

        public override double GetAtmosphericDensity(double height)
        {
            return 0.001;
        }

        public override string ToString()
        {
            return "Europa";
        }
    }
}
