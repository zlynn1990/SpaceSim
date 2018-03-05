using System.Drawing;
using SpaceSim.Kernels;
using SpaceSim.Orbits;
using VectorMath;

namespace SpaceSim.SolarSystem.Stars
{
    class Sun : MassiveBodyBase
    {
        public override double Mass
        {
            get { return 1.9891e30; }
        }

        public override double SurfaceRadius
        {
            get { return SymbolKernel.SUN_RADIUS; }
        }

        public override double AtmosphereHeight
        {
            get { return SymbolKernel.SUN_ATMOSPHERE; }
        }

        public override double RotationRate
        {
            get { return -2.9030759347876406801791263614248e-6; }
        }

        public override double RotationPeriod
        {
            get { return 2160000; }
        }

        public override Color IconColor { get { return Color.Yellow; } }
        public override Color IconAtmopshereColor { get { return Color.Yellow; } }

        public Sun()
            : base(OrbitHelper.FromJplEphemeris(5.432947986595253E+05, 4.730052562792657E+05),
                   OrbitHelper.FromJplEphemeris(-2.899962878525558E-03, 1.198996655704255E-02), new SunKernel())
        {
        }

        public override double Visibility(RectangleD cameraBounds)
        {
            return 1;
        }

        public override string ToString()
        {
            return "Sun";
        }
    }
}
