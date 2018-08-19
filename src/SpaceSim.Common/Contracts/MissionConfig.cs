using System;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;
using VectorMath;

namespace SpaceSim.Common.Contracts
{
    [Serializable]
    public class MissionConfig
    {
        public string VehicleType { get; set; }
        public double PayloadMass { get; set; }

        public string ParentPlanet { get; set; }

        public DVector2 PositionOffset { get; set; }
        public DVector2 VelocityOffset { get; set; }

        public string LaunchDate { get; set; }

        public int ClockDelay { get; set; }
        public int TimeSkew { get; set; }

        public DateTime GetLaunchDate()
        {
            DateTime date;

            if (DateTime.TryParse(LaunchDate, new CultureInfo("en-us"), DateTimeStyles.None, out date))
            {
                return date;
            }

            // Default to epoch if no date is found
            return Constants.Epoch;
        }

        /// <summary>
        /// Loads a mission from a profile path.
        /// </summary>
        public static MissionConfig Load(string path)
        {
            var configSerializer = new XmlSerializer(typeof(MissionConfig));

            using (var stream = new FileStream(Path.Combine(path, "MissionConfig.xml"), FileMode.Open))
            {
                var config = (MissionConfig)configSerializer.Deserialize(stream);

                if (config.PositionOffset == null)
                {
                    config.PositionOffset = DVector2.Zero;
                }

                if (config.VelocityOffset == null)
                {
                    config.VelocityOffset = DVector2.Zero;
                }

                return config;
            }
        }
    }
}
