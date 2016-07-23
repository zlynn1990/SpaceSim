using System.Drawing;
using SpaceSim.Engines;
using VectorMath;

namespace SpaceSim.Spacecrafts.DragonV2
{
    class DragonV2Trunk : SpaceCraftBase
    {
        public override string CraftName { get { return "DragonV2 Trunk"; } }

        public override double Width { get { return 4.72; } }
        public override double Height { get { return 4.1; } }

        public override double DryMass { get { return 1000; } }

        public override bool ExposedToAirFlow { get { return Parent == null; } }

        public override double DragCoefficient { get { return 0.3; } }

        // Cylinder - 2 * pi * r * h
        public override double CrossSectionalArea { get { return 27.6579; } }

        public override Color IconColor { get { return Color.White; } }

        public override string CommandFileName { get { return "dragonTrunk.xml"; } }

        public DragonV2Trunk(string craftDirectory, DVector2 position, DVector2 velocity)
            : base(craftDirectory, position, velocity, 0, "Textures/dragonTrunkV2.png")
        {
            StageOffset = new DVector2(0, 4);

            Engines = new IEngine[0];
        }
    }
}
