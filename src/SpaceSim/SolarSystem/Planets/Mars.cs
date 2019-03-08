using System;
using System.Drawing;
using SpaceSim.Kernels;
using SpaceSim.Orbits;

namespace SpaceSim.SolarSystem.Planets
{
    class Mars : MassiveBodyBase
    {
        public override string ApoapsisName { get { return "Apoareion"; } }
        public override string PeriapsisName { get { return "Periareion"; } }

        public override double Mass
        {
            get { return 0.64174e24; }
        }

        public override double SurfaceRadius
        {
            get { return BaseKernel.MARS_RADIUS; }
        }

        public override double AtmosphereHeight
        {
            get { return BaseKernel.MARS_ATMOSPHERE; }
        }

        public override double RotationRate
        {
            get { return -7.077658089896e-5; }
        }

        public override double RotationPeriod
        {
            get { return 88774.92; }
        }

        public override Color IconColor { get { return Color.Orange; } }
        public override Color IconAtmopshereColor { get { return Color.LightSalmon; } }

        public Mars()
            : base(OrbitHelper.FromJplEphemeris(1.186183757284019E+08, -1.722428244606592E+08),
                   OrbitHelper.FromJplEphemeris(2.091501229864822E+01, 1.576687031430872E+01), new MarsKernel())
        {
        }

        // Realistic density model based off https://www.grc.nasa.gov/www/k-12/rocket/atmos.html
        public override double GetAtmosphericDensity(double altitude)
        {
            if (altitude > AtmosphereHeight) return 0;

            double temperature;
            if (altitude > 7000)
            {
                temperature = -23.4 - 0.00222 * altitude;
            }
            else
            {
                temperature = -31 - 0.000998 * altitude;
            }

            double pressure = 0.699 * Math.Exp(-0.00009 * temperature);
            return pressure / (0.1921 * (temperature + 273.1));
        }

        public override double GetSpeedOfSound(double altitude)
        {
            double speed = 244.2;
            if (altitude > 0 && altitude < 30000)
                speed -= altitude * 0.00104;
            else if (altitude > 30000)
                speed = 212.9;

            return speed;
        }

        public override string ToString()
        {
            return "Mars";
        }
    }
}
