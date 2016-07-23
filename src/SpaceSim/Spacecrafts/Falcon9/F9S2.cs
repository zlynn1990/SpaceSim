using System;
using System.Drawing;
using SpaceSim.Engines;
using VectorMath;

namespace SpaceSim.Spacecrafts.Falcon9
{
    sealed class F9S2 : SpaceCraftBase
    {
        public override string CraftName { get { return "F9 S2"; } }

        public override double DryMass { get { return 4000; } }

        public override double Width { get { return 3.706; } }
        public override double Height { get { return 14.0018; } }

        public override bool ExposedToAirFlow { get { return Parent == null; } }

        public override double DragCoefficient { get { return 0.5; } }

        public override Color IconColor { get { return Color.White; } }

        public override string CommandFileName { get { return "F9S2.xml"; } }

        public F9S2(string craftDirectory, DVector2 position, DVector2 velocity, double stageOffset)
            : base(craftDirectory, position, velocity, 108185, "Textures/fh9S2.png")
        {
            StageOffset = new DVector2(0, stageOffset);

            Engines = new IEngine[]
            {
                new Merlin1DVac(this, new DVector2(0, Height * 0.3))
            };
        }
    }
}
