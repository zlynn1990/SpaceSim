using System.Drawing;
using SpaceSim.Kernels;
using SpaceSim.Orbits;
using VectorMath;

namespace SpaceSim.SolarSystem.Moons
{
    class Callisto : MassiveBodyBase
    {
        public override double Mass
        {
            get { return 1.075938e23; }
        }

        public override double SurfaceRadius
        {
            get { return BaseKernel.CALLISTO_RADIUS; }
        }

        public override double AtmosphereHeight
        {
            get { return 0.0; }
        }

        public override double RotationRate
        {
            get { return -4.3574794015100468486813972729667e-6; }
        }

        public override double RotationPeriod
        {
            get { return 1441931.1552; }
        }

        public override Color IconColor { get { return Color.RosyBrown; } }
        public override Color IconAtmopshereColor { get { return Color.White; } }

        public Callisto(DVector2 parentPositon, DVector2 parentVelocity)
            : base(OrbitHelper.FromJplEphemeris(-1.838974425381978E+06, 4.612334321076249E+05) + parentPositon,
                OrbitHelper.FromJplEphemeris(-1.996468410126901E+00, -7.894863713236139E+00) + parentVelocity, new CallistoKernel())
        {
        }

        public override double GetAtmosphericDensity(double height)
        {
            return 0.0;
        }

        public override string ToString()
        {
            return "Callisto";
        }
    }
}
