using System;
using System.IO;
using System.Xml.Serialization;
using VectorMath;

namespace SpaceSim.Contracts
{
    [Serializable]
    public class MissionConfig
    {
        public string VehicleType { get; set; }

        public double PayloadMass { get; set; }

        public string ParentPlanet { get; set; }

        public DVector2 PositionOffset { get; set; }

        public DVector2 VelocityOffset { get; set; }

        public DateTime LaunchDate { get; set; }

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
