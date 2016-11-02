using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using SpaceSim.Contracts;
using SpaceSim.SolarSystem;
using SpaceSim.Spacecrafts.DragonV1;
using SpaceSim.Spacecrafts.DragonV2;
using SpaceSim.Spacecrafts.Falcon9;
using SpaceSim.Spacecrafts.Falcon9SSTO;
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

        public static List<ISpaceCraft> BuildSpaceCraft(IMassiveBody planet, string craftDirectory, float offset = 0)
        {
            Payload payload = GetPayload(craftDirectory);

            if (string.IsNullOrEmpty(payload.CraftType))
            {
                throw new Exception("Please specify a craftType in the payload.xml!");
            }

            switch (payload.CraftType)
            {
                case "GenericF9":
                    return BuildF9(planet, payload, craftDirectory, offset);
                case "DragonF9":
                    return BuildF9Dragon(planet, craftDirectory, offset);
                case "F9SSTO":
                    return BuildF9SSTO(planet, craftDirectory);
                case "DragonAbort":
                    return BuildDragonV2Abort(planet, payload, craftDirectory);
                case "DragonEntry":
                    return BuildDragonV2Entry(planet, payload, craftDirectory);
                case "GenericFH":
                    return BuildFalconHeavy(planet, payload, craftDirectory, offset);
                    default:
                    throw new Exception("Unkown craftType: " + payload.CraftType);
            }
        }

        private static List<ISpaceCraft> BuildF9(IMassiveBody planet, Payload payload, string craftDirectory, float offset = 0)
        {
            var demoSat = new DemoSat(craftDirectory, planet.Position + new DVector2(offset, -planet.SurfaceRadius),
                                      planet.Velocity + new DVector2(-400, 0), payload.DryMass, payload.PropellantMass);

            var f9S1 = new F9S1(craftDirectory, DVector2.Zero, DVector2.Zero);
            var f9S2 = new F9S2(craftDirectory, DVector2.Zero, DVector2.Zero, 13.3);

            demoSat.AddChild(f9S2);
            f9S2.SetParent(demoSat);
            f9S2.AddChild(f9S1);
            f9S1.SetParent(f9S2);

            return new List<ISpaceCraft>
            {
                demoSat, f9S2, f9S1
            };
        }

        private static List<ISpaceCraft> BuildF9Dragon(IMassiveBody planet, string craftDirectory, float offset = 0)
        {
            var dragon = new Dragon(craftDirectory, planet.Position + new DVector2(offset, -planet.SurfaceRadius), planet.Velocity);
            var dragonTrunk = new DragonTrunk(craftDirectory, DVector2.Zero, DVector2.Zero);

            var f9S1 = new F9S1(craftDirectory, DVector2.Zero, DVector2.Zero);
            var f9S2 = new F9S2(craftDirectory, DVector2.Zero, DVector2.Zero, 8.3);

            dragon.AddChild(dragonTrunk);
            dragonTrunk.SetParent(dragon);
            dragonTrunk.AddChild(f9S2);
            f9S2.SetParent(dragonTrunk);
            f9S2.AddChild(f9S1);
            f9S1.SetParent(f9S2);

            return new List<ISpaceCraft>
            {
                dragon, dragonTrunk, f9S2, f9S1
            };
        }

        private static List<ISpaceCraft> BuildF9SSTO(IMassiveBody planet, string craftDirectory)
        {
            var f9SSTO = new F9SSTO(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius),
                                planet.Velocity + new DVector2(-400, 0));

            return new List<ISpaceCraft> { f9SSTO };
        }

        private static List<ISpaceCraft> BuildDragonV2Abort(IMassiveBody planet, Payload payload, string craftDirectory)
        {
            var dragon = new DragonV2.DragonV2(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius),
                                                planet.Velocity, payload.DryMass, payload.PropellantMass);

            var dragonTrunk = new DragonV2Trunk(craftDirectory, DVector2.Zero, DVector2.Zero);

            dragon.AddChild(dragonTrunk);
            dragonTrunk.SetParent(dragon);

            return new List<ISpaceCraft> { dragon, dragonTrunk };
        }

        private static List<ISpaceCraft> BuildDragonV2Entry(IMassiveBody planet, Payload payload, string craftDirectory)
        {
            var dragon = new DragonV2.DragonV2(craftDirectory, planet.Position + new DVector2(planet.SurfaceRadius*0.75, planet.SurfaceRadius*-0.75),
                                               planet.Velocity + new DVector2(-6000, -5100), payload.DryMass, payload.PropellantMass);

            var dragonTrunk = new DragonV2Trunk(craftDirectory, DVector2.Zero, DVector2.Zero);

            dragon.AddChild(dragonTrunk);
            dragonTrunk.SetParent(dragon);

            dragon.SetPitch(Math.PI * 1.24);

            return new List<ISpaceCraft> { dragon, dragonTrunk };
        }

        private static List<ISpaceCraft> BuildFalconHeavy(IMassiveBody planet, Payload payload, string craftDirectory, float offset = 0)
        {
            var demoSat = new DemoSat(craftDirectory, planet.Position + new DVector2(offset, -planet.SurfaceRadius),
                                      planet.Velocity + new DVector2(-400, 0), payload.DryMass, payload.PropellantMass);

            var fhS1 = new FHS1(craftDirectory, DVector2.Zero, DVector2.Zero);
            var fhS2 = new FHS2(craftDirectory, DVector2.Zero, DVector2.Zero);

            var fhLeftBooster = new FHBooster(craftDirectory, 1, DVector2.Zero, DVector2.Zero);
            var fhRightBooster = new FHBooster(craftDirectory, 2, DVector2.Zero, DVector2.Zero);

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
