using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using SpaceSim.Contracts;
using SpaceSim.SolarSystem;

namespace SpaceSim.Structures
{
    static class StructureFactory
    {
        public static List<StructureBase> Load(IMassiveBody planet, double launchAngle, string missionProfile)
        {
            var structures = new List<StructureBase>();

            string structurePath = Path.Combine(missionProfile, "Structures.xml");

            // No structures are okay
            if (!File.Exists(structurePath)) return structures;

            var configSerializer = new XmlSerializer(typeof(List<StructureConfig>));

            using (var stream = new FileStream(structurePath, FileMode.Open))
            {
                var structureConfigs = (List<StructureConfig>)configSerializer.Deserialize(stream);

                foreach (StructureConfig structureConfig in structureConfigs)
                {
                    double surfaceAngle = GetDownrangeAngle(planet, structureConfig.DownrangeDistance) + launchAngle;

                    switch (structureConfig.Type)
                    {
                        case "ASDS":
                            structures.Add(new ASDS(surfaceAngle, structureConfig.HeightOffset, planet));
                            break;
                        case "Edwards":
                            structures.Add(new Edwards(surfaceAngle, structureConfig.HeightOffset, planet));
                            break;
                        case "ITSMount":
                            structures.Add(new ITSMount(surfaceAngle, structureConfig.HeightOffset, planet));
                            break;
                        case "LandingPad":
                            structures.Add(new LandingPad(surfaceAngle, structureConfig.HeightOffset, planet));
                            break;
                        case "ServiceTower":
                            structures.Add(new ServiceTower(surfaceAngle, structureConfig.HeightOffset, planet));
                            break;
                        case "Strongback":
                            structures.Add(new Strongback(surfaceAngle, structureConfig.HeightOffset, planet));
                            break;
                    }
                }
            }

            return structures;
        }

        private static double GetDownrangeAngle(IMassiveBody planet, double downrangeDistance)
        {
            double circumference = 2 * Math.PI * planet.SurfaceRadius;

            double downrangeRatio = -downrangeDistance / circumference;

            return downrangeRatio * Math.PI * 2;
        }
    }
}
