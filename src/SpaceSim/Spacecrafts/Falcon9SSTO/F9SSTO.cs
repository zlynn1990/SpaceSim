using System;
using System.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Spacecrafts.Falcon9SSTO
{
    sealed class F9SSTO : SpaceCraftBase
    {
        public override string CraftName { get { return "F9 S1"; } }

        public override double DryMass { get { return 21600; } }

        public override double Width { get { return 4.11; } }
        public override double Height { get { return 47.812188; } }

        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExtendsFineness; } }

        public override double FormDragCoefficient
        {
            get
            {
                double baseCd = GetBaseCd(0.15);
                double alpha = GetAlpha();
                double cosAlpha = Math.Cos(alpha);
                double Cd = Math.Abs(baseCd * cosAlpha);

                return Cd;
            }
        }

        public override double LiftCoefficient
        {
            get
            {
                double baseCd = GetBaseCd(0.6);
                double alpha = GetAlpha();
                double sinAlpha = Math.Sin(alpha * 2);
                return baseCd * sinAlpha;
            }
        }

        public override double CrossSectionalArea { get { return Math.PI * Math.Pow(Width / 2, 2); } }

        public override double ExposedSurfaceArea
        {
            get
            {
                // A = 2πrh + πr2
                return 2 * Math.PI * (Width / 2) * Height + CrossSectionalArea;
            }
        }

        public override double LiftingSurfaceArea { get { return Width * Height; } }

        public override Color IconColor { get { return Color.White; } }

        public override string CommandFileName { get { return "F9SSTO.xml"; } }

        public F9SSTO(string craftDirectory, DVector2 position, DVector2 velocity)
            : base(craftDirectory, position, velocity, 409500, "Textures/f9ssto.png")
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
