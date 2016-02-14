using System;
using System.Drawing;
using SpaceSim.Kernels;
using SpaceSim.Orbits;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.SolarSystem.Planets
{
    class Earth : MassiveBodyBase
    {
        public override double Mass
        {
            get { return 5.97219e24; }
        }

        public override double SurfaceRadius
        {
            get { return SymbolKernel.EARTH_RADIUS; }
        }

        public override double AtmosphereHeight
        {
            get { return SymbolKernel.EARTH_ATMOSPHERE; }
        }

        public override double RotationRate
        {
            get { return -7.2722052166e-5; }
        }

        public override double RotationPeriod
        {
            get { return 86400; }
        }

        public override Color IconColor { get { return Color.Green; } }
        public override Color IconAtmopshereColor { get { return Color.LightSkyBlue; } }

        public Earth()
            : base(OrbitHelper.GetPosition(1.47095e11, 1.7967674211, DVector2.Zero),
                   OrbitHelper.GetVelocity(1.47095e11, 1.7967674211, -3.029e4, DVector2.Zero), new EarthKernel())
        {
        }

        // Realistic density model based off https://www.grc.nasa.gov/www/k-12/rocket/atmos.html
        public override double GetAtmosphericDensity(double height)
        {
            if (height > AtmosphereHeight)
            {
                return 0;
            }

            double tempurature;
            double pressure;

            if (height > 25098.756)
            {
                tempurature = -205.05 + 0.0053805776 * height;

                pressure = 51.97 * Math.Pow((tempurature + 459.7) / 389.98, -11.388);
            }
            else if (height > 11019.13)
            {
                tempurature = -70;

                pressure = 473.1 * Math.Exp(1.73 - 0.00015748032 * height);
            }
            else
            {
                tempurature = 59 - 0.0116797904 * height;

                pressure = 2116 * Math.Pow((tempurature + 459.7) / 518.6, 5.256);   
            }

            double density = pressure / (1718 * (tempurature + 459.7));

            return density * 515.379;
        }

        public override string ToString()
        {
            return "Earth";
        }
    }
}
