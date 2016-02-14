using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using SpaceSim.Contracts;
using SpaceSim.Contracts.Commands;
using SpaceSim.SolarSystem;
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

        public static List<ISpaceCraft> BuildFalconHeavy(IMassiveBody planet, string path)
        {
            var spacecraft = new List<ISpaceCraft>();

            Payload payload = GetPayload(path);

            var demoSat = new DemoSat(planet.Position + new DVector2(0, -planet.SurfaceRadius),
                                      planet.Velocity + new DVector2(-400, 0), payload.DryMass, payload.PropellantMass);

            var falcon9S1 = new FH9S1(DVector2.Zero, DVector2.Zero);
            var falcon9S2 = new FH9S2(DVector2.Zero, DVector2.Zero);

            var fhLeftBooster = new FHBooster(1, DVector2.Zero, DVector2.Zero);
            var fhRightBooster = new FHBooster(2, DVector2.Zero, DVector2.Zero);

            demoSat.AddChild(falcon9S2);
            falcon9S2.SetParent(demoSat);
            falcon9S2.AddChild(falcon9S1);
            falcon9S1.SetParent(falcon9S2);
            falcon9S1.AddChild(fhLeftBooster);
            falcon9S1.AddChild(fhRightBooster);
            fhLeftBooster.SetParent(falcon9S1);
            fhRightBooster.SetParent(falcon9S1);

            spacecraft.Add(demoSat);
            spacecraft.Add(falcon9S2);
            spacecraft.Add(falcon9S1);
            spacecraft.Add(fhLeftBooster);
            spacecraft.Add(fhRightBooster);

            foreach (ISpaceCraft spaceCraft in spacecraft)
            {
                spaceCraft.InitializeController(path);
            }

            return spacecraft;
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
