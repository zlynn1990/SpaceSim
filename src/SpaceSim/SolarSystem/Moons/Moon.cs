using System.Drawing;
using SpaceSim.Kernels;
using SpaceSim.Orbits;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.SolarSystem.Moons
{
    class Moon : MassiveBodyBase
    {
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
            get { return 6.371e5; }
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

        public Moon(DVector2 positionOffset, DVector2 velocityOffset)
             : base(OrbitHelper.GetPosition(3.63295e8, 0.2967059, positionOffset),
                    OrbitHelper.GetVelocity(3.63295e8, 0.2967059, -1076.0, velocityOffset), new MoonKernel())
        {
        }

        public override double GetAtmosphericDensity(double height)
        {
            return 0.001;
        }

        public override string ToString()
        {
            return "Moon";
        }
    }
}
