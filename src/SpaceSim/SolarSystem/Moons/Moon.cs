using System.Drawing;
using SpaceSim.Kernels;
using SpaceSim.Orbits;
using VectorMath;

namespace SpaceSim.SolarSystem.Moons
{
    class Moon : MassiveBodyBase
    {
        public override string ApoapsisName { get { return "Aposelene"; } }
        public override string PeriapsisName { get { return "Periselene"; } }

        public override double Mass
        {
            get { return 7.3477e22; }
        }

        public override double SurfaceRadius
        {
            get { return SymbolKernel.MOON_RADIUS; }
        }

        public override double AtmosphereHeight
        {
            get { return 0.0; }
        }

        public override double RotationRate
        {
            get { return -2.6616665019555815474155301717917e-6; }
        }

        public override double RotationPeriod
        {
            get { return 2332800; }
        }

        public override Color IconColor { get { return Color.LightGray; } }
        public override Color IconAtmopshereColor { get { return Color.White; } }

        public Moon(DVector2 parentPositon, DVector2 parentVelocity)
            : base(OrbitHelper.FromJplEphemeris(-1.275488485528684E+05, -3.756609087364946E+05) + parentPositon,
                   OrbitHelper.FromJplEphemeris(9.291844409239249E-01, -2.857101216046916E-01) + parentVelocity, new MoonKernel())
        {
        }

        public override double GetAtmosphericDensity(double height)
        {
            return 0.0;
        }

        public override string ToString()
        {
            return "Moon";
        }
    }
}
