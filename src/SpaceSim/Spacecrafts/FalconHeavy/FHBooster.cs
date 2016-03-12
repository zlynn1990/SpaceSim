using System;
using System.Drawing;
using SpaceSim.Engines;
using VectorMath;

namespace SpaceSim.Spacecrafts.FalconHeavy
{
    sealed class FHBooster : SpaceCraftBase
    {
        public int Id { get; private set; }

        public override double DryMass { get { return 25600; } }

        public override double Width { get { return 4.11; } }
        public override double Height { get { return 44.6; } }

        public override bool ExposedToAirFlow { get { return true; } }

        public override double DragCoefficient
        {
            get { return 1.6; }
        }

        public override double CrossSectionalArea { get { return Math.PI * 1.83 * 1.83; } }

        public override Color IconColor { get { return Color.White; } }

        public FHBooster(int id, DVector2 position, DVector2 velocity)
            : base(position, velocity, 398887, "Textures/fhBooster" + id + ".png")
        {
            Id = id;

            if (Id == 1)
            {
                StageOffset = new DVector2(-4, 1.5);   
            }
            else
            {
                StageOffset = new DVector2(4, 1.5);
            }

            Engines = new IEngine[9];

            for (int i = 0; i < 9; i++)
            {
                double engineOffsetX = (i - 4.0) / 4.0;

                var offset = new DVector2(engineOffsetX * Width * 0.3, Height * 0.45);

                Engines[i] = new Merlin1D(i, this, offset);
            }
        }

        public override string CommandFileName
        {
            get { return Id == 1 ? "FHLeftBooster.xml" : "FHRightBooster.xml"; }
        }

        public override string ToString()
        {
            return Id == 1 ? "Falcon Heavy Left Booster" : "Falcon Heavy Right Booster";
        }
    }
}
