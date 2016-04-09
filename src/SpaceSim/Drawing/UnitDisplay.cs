namespace SpaceSim.Drawing
{
    /// <summary>
    /// Creates display friendly representations of units.
    /// </summary>
    static class UnitDisplay
    {
        public static string Speed(double speed)
        {
            if (speed > 10000)
            {
                speed *= 0.001;

                return speed.ToString("#,##0.0") + " km/s";
            }

            return speed.ToString("#,##0") + " m/s";
        }

        public static string Acceleration(double acceleration)
        {
            double accelerationInGs = acceleration / 9.81;

            if (accelerationInGs < 0.0001)
            {
                return "0.000 g";
            }

            return accelerationInGs.ToString("0.000") + " g";
        }

        public static string Distance(double distance)
        {
            if (distance > 10000000000)
            {
                distance *= 6.684492e-12;

                return distance.ToString("0.00") + " AU";
            }

            if (distance > 1000000)
            {
                distance *= 0.001;

                return distance.ToString("#,##0") + " km";
            }

            if (distance > 10000)
            {
                distance *= 0.001;

                return distance.ToString("#,##0.0") + " km";
            }

            if (distance < 0)
            {
                distance = 0;
            }

            return distance.ToString("#,##0.0") + " m";
        }

        public static string Force(double force)
        {
            if (force > 1000)
            {
                force *= 0.001;

                return force.ToString("#,##0.0") + " kN";
            }

            return force.ToString("#,##0.0") + " N";
        }

        public static string Mass(double mass)
        {
            if (mass > 10000000)
            {
                return mass.ToString("0.#e0") + " kg";
            }

            if (mass > 1000)
            {
                return mass.ToString("#,##0") + " kg";
            }

            return mass.ToString("#,##0.0") + " kg";
        }

        public static string Density(double density)
        {
            return density.ToString("0.##0") + " kg/m^3";
        }

        public static string Pressure(double pressure)
        {
            if (pressure > 1000)
            {
                pressure *= 0.001;

                return pressure.ToString("#,##0.0") + " kPa";
            }

            return pressure.ToString("#,##0.0") + " Pa";
        }
    }
}
