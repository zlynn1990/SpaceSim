using System.Drawing;
using SpaceSim.Engines;
using VectorMath;

namespace SpaceSim.Spacecrafts.DragonV1
{
    class DragonTrunk : SpaceCraftBase
    {
        public override double Width { get { return 3.8754; } }
        public override double Height { get { return 3.253; } }

        public override double DryMass { get { return 1000; } }

        public override bool ExposedToAirFlow { get { return Parent == null; } }

        public override double DragCoefficient { get { return 0.3; } }

        // Cylinder - 2 * pi * r * h
        public override double CrossSectionalArea { get { return 27.6579; } }

        public override Color IconColor { get { return Color.White; } }

        public DragonTrunk(DVector2 position, DVector2 velocity)
            : base(position, velocity, 0, "Textures/dragonTrunk.png")
        {
            StageOffset = new DVector2(0, 2.8);

            Engines = new IEngine[0];
        }

        public override string CommandFileName { get { return "dragonTrunk.xml"; } }

        public override string ToString()
        {
            return "Dragon Trunk";
        }
    }
}
