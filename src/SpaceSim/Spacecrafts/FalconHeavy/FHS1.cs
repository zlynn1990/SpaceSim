using System;
using System.Drawing;
using SpaceSim.Engines;
using VectorMath;

namespace SpaceSim.Spacecrafts.FalconHeavy
{
    sealed class FHS1 : SpaceCraftBase
    {
        public override double DryMass { get { return 22200; } }

        public override double Width { get { return 4.11; } }
        public override double Height { get { return 47.812188; } }

        public override bool ExposedToAirFlow { get { return Parent == null; } }

        public override double DragCoefficient
        {
            get
            {
                if (MachNumber < 0.65 || MachNumber > 2.8)
                {
                    return 0.4;
                }

                double normalizedMach;

                if (MachNumber < 1.5)
                {
                    normalizedMach = (MachNumber - 0.65) * 1.17;
                }
                else
                {
                    normalizedMach = (2.8 - MachNumber) * 0.769;
                }

                return 0.4 + normalizedMach * 0.31;
            }
        }

        public override double CrossSectionalArea { get { return 5 * Math.PI * 1.83 * 1.83; } }

        public override Color IconColor { get { return Color.White; } }

        public FHS1(DVector2 position, DVector2 velocity)
            : base(position, velocity, 409500, "Textures/fh9S1.png")
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

        public override string CommandFileName { get { return "FHCore.xml"; } }

        public override string ToString()
        {
            return "Falcon Heavy First Stage";
        }
    }
}
