using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using SpaceSim.Contracts;
using SpaceSim.SolarSystem;
using SpaceSim.Spacecrafts.Falcon9;
using SpaceSim.Spacecrafts.FalconHeavy;
using VectorMath;

namespace SpaceSim.Spacecrafts
{
    static class SpacecraftFactory
    {
        private static XmlSerializer PayloadSerializer;

        static SpacecraftFactory()
        {
            PayloadSerializer = new XmlSerializer(typeof(Payload));
        }

        public static List<ISpaceCraft> BuildF9SSTO(IMassiveBody planet, string path)
        {
            var f9S1 = new F9S1(planet.Position + new DVector2(0, -planet.SurfaceRadius),
                                planet.Velocity + new DVector2(-400, 0));

            return new List<ISpaceCraft> { f9S1 };
        }

        public static List<ISpaceCraft> BuildFalconHeavy(IMassiveBody planet, string path)
        {
            Payload payload = GetPayload(path);

            var demoSat = new DemoSat(planet.Position + new DVector2(0, -planet.SurfaceRadius),
                                      planet.Velocity + new DVector2(-400, 0), payload.DryMass, payload.PropellantMass);

            var fhS1 = new FHS1(DVector2.Zero, DVector2.Zero);
            var fhS2 = new FHS2(DVector2.Zero, DVector2.Zero);

            var fhLeftBooster = new FHBooster(1, DVector2.Zero, DVector2.Zero);
            var fhRightBooster = new FHBooster(2, DVector2.Zero, DVector2.Zero);

            demoSat.AddChild(fhS2);
            fhS2.SetParent(demoSat);
            fhS2.AddChild(fhS1);
            fhS1.SetParent(fhS2);
            fhS1.AddChild(fhLeftBooster);
            fhS1.AddChild(fhRightBooster);
            fhLeftBooster.SetParent(fhS1);
            fhRightBooster.SetParent(fhS1);

            return new List<ISpaceCraft>
            {
                demoSat, fhS2, fhS1, fhLeftBooster, fhRightBooster
            };
        }

        private static Payload GetPayload(string path)
        {
            using (var stream = new FileStream(Path.Combine(path, "payload.xml"), FileMode.Open))
            {
                return (Payload)PayloadSerializer.Deserialize(stream);
            }
        }
    }
}
