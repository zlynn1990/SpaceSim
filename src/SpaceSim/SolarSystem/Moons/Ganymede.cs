using System.Drawing;
using SpaceSim.Kernels;
using SpaceSim.Orbits;
using VectorMath;

namespace SpaceSim.SolarSystem.Moons
{
    class Ganymede : MassiveBodyBase
    {
        public override double Mass
        {
            get { return 1.4819e23; }
        }

        public override double SurfaceRadius
        {
            get { return BaseKernel.GANYMEDE_RADIUS; }
        }

        public override double AtmosphereHeight
        {
            get { return 0.0; }
        }

        public override double RotationRate
        {
            get { return -1.0164443839668306187470707863048e-5; }
        }

        public override double RotationPeriod
        {
            get { return 618153.3792; }
        }

        public override Color IconColor { get { return Color.Gray; } }
        public override Color IconAtmopshereColor { get { return Color.White; } }

        public Ganymede(DVector2 parentPositon, DVector2 parentVelocity)
            : base(OrbitHelper.FromJplEphemeris(-9.194475286771294E+05, -5.471806593872152E+05) + parentPositon,
                OrbitHelper.FromJplEphemeris(5.585068938118295E+00, -9.333406535151921E+00) + parentVelocity, new GanymedeKernel())
        {
        }

        public override double GetAtmosphericDensity(double height)
        {
            return 0.0;
        }

        public override string ToString()
        {
            return "Ganymede";
        }
    }
}
