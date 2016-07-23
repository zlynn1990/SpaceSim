using System.Drawing;
using SpaceSim.Engines;
using VectorMath;

namespace SpaceSim.Spacecrafts.FalconHeavy
{
    sealed class FHS2 : SpaceCraftBase
    {
        public override string CraftName { get { return "FH S2 "; } }

        public override double DryMass { get { return 4000; } }

        public override double Width { get { return 3.706; } }
        public override double Height { get { return 14.0018; } }

        public override bool ExposedToAirFlow { get { return Parent == null; } }

        public override double DragCoefficient { get { return 0.5; } }

        public override Color IconColor { get { return Color.White; } }

        public override string CommandFileName { get { return "FHStage2.xml"; } }

        public FHS2(string craftDirectory, DVector2 position, DVector2 velocity)
            : base(craftDirectory, position, velocity, 108185, "Textures/fh9S2.png")
        {
            StageOffset = new DVector2(0, 13.3);

            Engines = new IEngine[]
            {
                new Merlin1DVac(this, new DVector2(0, Height * 0.3))
            };
        }
    }
}
