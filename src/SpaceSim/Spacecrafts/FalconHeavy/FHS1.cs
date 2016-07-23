using System;
using System.Drawing;
using SpaceSim.Engines;
using VectorMath;

namespace SpaceSim.Spacecrafts.FalconHeavy
{
    sealed class FHS1 : SpaceCraftBase
    {
        public override string CraftName { get { return "FH Core "; } }

        public override double DryMass { get { return 25600; } }

        public override double Width { get { return 4.11; } }
        public override double Height { get { return 47.812188; } }

        public override bool ExposedToAirFlow { get { return Parent == null; } }

        public override double DragCoefficient
        {
            get { return 1.6; }
        }

        public override double CrossSectionalArea { get { return Math.PI * 1.83 * 1.83; } }

        public override Color IconColor { get { return Color.White; } }

        public override string CommandFileName { get { return "FHCore.xml"; } }

        public FHS1(string craftDirectory, DVector2 position, DVector2 velocity)
            : base(craftDirectory, position, velocity, 398887, "Textures/fh9S1.png")
        {
            StageOffset = new DVector2(0, 25.5);

            Engines = new IEngine[9];

            for (int i=0; i < 9; i++)
            {
                double engineOffsetX = (i - 4.0) / 4.0;

                var offset = new DVector2(engineOffsetX * Width * 0.3, Height * 0.45);

                Engines[i] = new Merlin1D(i, this, offset);
            }
        }

        public void DeployFins() { }
        public void DeployLegs() { }
    }
}
