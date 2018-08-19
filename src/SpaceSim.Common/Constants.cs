using System;

namespace SpaceSim.Common
{
    public static class Constants
    {
        public static DateTime Epoch = new DateTime(2016, 9, 9, 6, 53, 0, DateTimeKind.Utc);

        public const double GravitationConstant = 6.67384e-11;

        public const double SpeedOfLight = 2.998e8;

        public const double SpeedLightSquared = 8.988004e16;

        public const double TwoPi = 6.283185307179586476925286766559;
        public const double PiOverTwo = 1.5707963267948966192313216916398;
    }
}
