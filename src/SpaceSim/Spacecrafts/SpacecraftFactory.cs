using System;
using System.Collections.Generic;
using SpaceSim.Contracts;
using SpaceSim.SolarSystem;
using SpaceSim.Spacecrafts.DragonV1;
using SpaceSim.Spacecrafts.DragonV2;
using SpaceSim.Spacecrafts.Falcon9;
using SpaceSim.Spacecrafts.Falcon9SSTO;
using SpaceSim.Spacecrafts.FalconCommon;
using SpaceSim.Spacecrafts.FalconHeavy;
using SpaceSim.Spacecrafts.ITS;
using VectorMath;

namespace SpaceSim.Spacecrafts
{
    static class SpacecraftFactory
    {
        public static List<ISpaceCraft> BuildSpaceCraft(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            if (string.IsNullOrEmpty(config.VehicleType))
            {
                throw new Exception("Must specify a vehicle type in the MissionConfig.xml!");
            }

            if (string.IsNullOrEmpty(config.ParentPlanet))
            {
                throw new Exception("Must specify a parent planet for the launch vehicle!");
            }

            switch (config.VehicleType)
            {
                case "GenericF9":
                    return BuildGenericF9(planet, config, craftDirectory);
                case "DragonF9":
                    return BuildF9Dragon(planet, config, craftDirectory);
                case "F9SSTO":
                    return BuildF9SSTO(planet, craftDirectory);
                case "DragonAbort":
                    return BuildDragonV2Abort(planet, config, craftDirectory);
                case "DragonEntry":
                    return BuildDragonV2Entry(planet, config, craftDirectory);
                case "RedDragonFH":
                    return BuildRedDragonFH(planet, config, craftDirectory);
                case "GreyDragonFH":
                    return BuildGreyDragonFH(planet, config, craftDirectory);
                case "GenericFH":
                    return BuildFalconHeavy(planet, config, craftDirectory);
                case "AutoLandingTest":
                    return BuildAutoLandingTest(planet, config, craftDirectory);
                case "ITS Crew Launch":
                    return BuildITSCrew(planet, config, craftDirectory);
                case "ITS Tanker SSTO":
                    return BuildITSTanker(planet, config, craftDirectory);
                case "ItsEDL":
                    return BuildItsEDL(planet, config, craftDirectory);
                default:
                    throw new Exception("Unknown vehicle type: " + config.VehicleType);
            }
        }

        private static List<ISpaceCraft> BuildGenericF9(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var demoSat = new DemoSat(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius) + config.PositionOffset, planet.Velocity, config.PayloadMass);

            var fairingLeft = new Fairing(craftDirectory, demoSat.Position, DVector2.Zero, true);
            var fairingRight = new Fairing(craftDirectory, demoSat.Position, DVector2.Zero, false);

            demoSat.AttachFairings(fairingLeft, fairingRight);

            var f9S1 = new F9S1(craftDirectory, DVector2.Zero, DVector2.Zero);
            var f9S2 = new F9S2(craftDirectory, DVector2.Zero, DVector2.Zero, 11.2);
            
            demoSat.AddChild(f9S2);
            f9S2.SetParent(demoSat);
            f9S2.AddChild(f9S1);
            f9S1.SetParent(f9S2);

            return new List<ISpaceCraft>
            {
                demoSat, f9S2, f9S1, fairingLeft, fairingRight
            };
        }

        private static List<ISpaceCraft> BuildF9Dragon(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var dragon = new Dragon(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius) + config.PositionOffset, planet.Velocity, config.PayloadMass);
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
            var f9SSTO = new F9SSTO(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius), planet.Velocity);

            return new List<ISpaceCraft> { f9SSTO };
        }

        private static List<ISpaceCraft> BuildDragonV2Abort(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var dragon = new DragonV2.DragonV2(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius), planet.Velocity, config.PayloadMass, 446);
            var dragonTrunk = new DragonV2Trunk(craftDirectory, DVector2.Zero, DVector2.Zero);

            dragon.AddChild(dragonTrunk);
            dragonTrunk.SetParent(dragon);

            return new List<ISpaceCraft> { dragon, dragonTrunk };
        }

        private static List<ISpaceCraft> BuildDragonV2Entry(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var dragon = new GreyDragon.GreyDragon(craftDirectory, planet.Position + new DVector2(0, planet.SurfaceRadius + 200000.0),
                                               planet.Velocity + new DVector2(11000, -1650), config.PayloadMass, 446);

            var dragonTrunk = new DragonV2Trunk(craftDirectory, DVector2.Zero, DVector2.Zero);

            dragon.AddChild(dragonTrunk);
            dragonTrunk.SetParent(dragon);

            dragon.SetPitch(Math.PI * 1.24);

            return new List<ISpaceCraft> { dragon, dragonTrunk };
        }

        private static List<ISpaceCraft> BuildFalconHeavy(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var demoSat = new DemoSat(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius) + config.PositionOffset,
                                      planet.Velocity + new DVector2(-400, 0) + config.VelocityOffset, config.PayloadMass);

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

        private static List<ISpaceCraft> BuildRedDragonFH(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var redDragon = new RedDragon.RedDragon(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius),
                                      planet.Velocity + new DVector2(-400, 0), config.PayloadMass);

            var dragonTrunk = new DragonV2Trunk(craftDirectory, DVector2.Zero, DVector2.Zero);

            var fhS1 = new FHS1(craftDirectory, DVector2.Zero, DVector2.Zero);
            var fhS2 = new FHS2(craftDirectory, DVector2.Zero, DVector2.Zero);

            var fhLeftBooster = new FHBooster(craftDirectory, 1, DVector2.Zero, DVector2.Zero);
            var fhRightBooster = new FHBooster(craftDirectory, 2, DVector2.Zero, DVector2.Zero);

            redDragon.AddChild(dragonTrunk);
            dragonTrunk.SetParent(redDragon);
            dragonTrunk.AddChild(fhS2);
            fhS2.SetParent(dragonTrunk);
            fhS2.AddChild(fhS1);
            fhS1.SetParent(fhS2);
            fhS1.AddChild(fhLeftBooster);
            fhS1.AddChild(fhRightBooster);
            fhLeftBooster.SetParent(fhS1);
            fhRightBooster.SetParent(fhS1);

            return new List<ISpaceCraft>
            {
                redDragon, dragonTrunk, fhS2, fhS1, fhLeftBooster, fhRightBooster
            };
        }

        private static List<ISpaceCraft> BuildGreyDragonFH(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var dragon = new GreyDragon.GreyDragon(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius), planet.Velocity, config.PayloadMass, 446);

            var dragonTrunk = new DragonV2Trunk(craftDirectory, DVector2.Zero, DVector2.Zero);

            var fhS1 = new FHS1(craftDirectory, DVector2.Zero, DVector2.Zero);
            var fhS2 = new FHS2(craftDirectory, DVector2.Zero, DVector2.Zero);

            var fhLeftBooster = new FHBooster(craftDirectory, 1, DVector2.Zero, DVector2.Zero);
            var fhRightBooster = new FHBooster(craftDirectory, 2, DVector2.Zero, DVector2.Zero);

            dragon.AddChild(dragonTrunk);
            dragonTrunk.SetParent(dragon);
            dragonTrunk.AddChild(fhS2);
            fhS2.SetParent(dragonTrunk);
            fhS2.AddChild(fhS1);
            fhS1.SetParent(fhS2);
            fhS1.AddChild(fhLeftBooster);
            fhS1.AddChild(fhRightBooster);
            fhLeftBooster.SetParent(fhS1);
            fhRightBooster.SetParent(fhS1);

            return new List<ISpaceCraft>
            {
                dragon, dragonTrunk, fhS2, fhS1, fhLeftBooster, fhRightBooster
            };
        }

        public static List<ISpaceCraft> BuildAutoLandingTest(IMassiveBody planet, MissionConfig payload, string craftDirectory)
        {
            var f9 = new F9S1(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius - 7000),
                planet.Velocity + new DVector2(-400, 400), 3500);

            return new List<ISpaceCraft>
            {
                f9,
            };
        }

        private static List<ISpaceCraft> BuildITSCrew(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new ITSShip(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius),
                planet.Velocity + new DVector2(-400, 0), config.PayloadMass, 0);

            var booster = new ITSBooster(craftDirectory, DVector2.Zero, DVector2.Zero);

            ship.AddChild(booster);
            booster.SetParent(ship);

            return new List<ISpaceCraft>
            {
                ship, booster
            };
        }

        private static List<ISpaceCraft> BuildITSTanker(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var tanker = new ITSTanker(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius),
                planet.Velocity + new DVector2(-400, 0), config.PayloadMass, 0);

            return new List<ISpaceCraft>
            {
                tanker
            };
        }

        private static List<ISpaceCraft> BuildItsEDL(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new ITSShip(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius - 150000),
                                  planet.Velocity + new DVector2(-7400, 700), config.PayloadMass, 0);

            return new List<ISpaceCraft>
            {
                ship
            };
        }
    }
}
