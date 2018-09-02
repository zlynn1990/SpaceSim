using System.Drawing;
using SpaceSim.Kernels;
using SpaceSim.Orbits;
using VectorMath;

namespace SpaceSim.SolarSystem.Moons
{
    class Io : MassiveBodyBase
    {
        public override double Mass
        {
            get { return 8.931938e22; }
        }

        public override double SurfaceRadius
        {
            get { return BaseKernel.IO_RADIUS; }
        }

        public override double AtmosphereHeight
        {
            get { return 0.0; }
        }

        public override double RotationRate
        {
            get { return -4.1105923995997146089500714672169e-5; }
        }

        public override double RotationPeriod
        {
            get { return 152853.5232; }
        }

        public override Color IconColor { get { return Color.DarkKhaki; } }
        public override Color IconAtmopshereColor { get { return Color.White; } }

        public Io(DVector2 parentPositon, DVector2 parentVelocity)
            : base(OrbitHelper.FromJplEphemeris(-4.187887882620349E+05, -3.532782596227979E+04) + parentPositon,
                OrbitHelper.FromJplEphemeris(1.513295262903566E+00, -1.731988564333744E+01) + parentVelocity, new IoKernel())
        {
        }

        public override double GetAtmosphericDensity(double height)
        {
            return 0.0;
        }

        public override string ToString()
        {
            return "Io";
        }
    }
}
