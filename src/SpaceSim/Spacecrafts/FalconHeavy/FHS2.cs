using System.Drawing;
using SpaceSim.Engines;
using VectorMath;

namespace SpaceSim.Spacecrafts.FalconHeavy
{
    sealed class FHS2 : SpaceCraftBase
    {
        public override string ShortName { get { return "FH S2 "; } }

        public override double DryMass { get { return 4000; } }

        public override double Width { get { return 3.706; } }
        public override double Height { get { return 14.0018; } }

        public override bool ExposedToAirFlow { get { return Parent == null; } }

        public override double DragCoefficient { get { return 0.5; } }

        public override Color IconColor { get { return Color.White; } }

        public FHS2(DVector2 position, DVector2 velocity)
            : base(position, velocity, 108185, "Textures/fh9S2.png")
        {
            StageOffset = new DVector2(0, 13.3);

            Engines = new IEngine[]
            {
                new Merlin1DVac(this, new DVector2(0, Height * 0.3))
            };
        }

        public override string CommandFileName { get { return "FHStage2.xml"; } }

        public override string ToString()
        {
            return "Falcon Heavy Second Stage";
        }
    }
}
