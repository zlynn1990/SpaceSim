using System;
using System.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Spacecrafts.Falcon9
{
    sealed class F9S2 : SpaceCraftBase
    {
        public override string CraftName { get { return "F9 S2"; } }

        public override double DryMass { get { return 4000; } }

        public override double Width { get { return 3.706; } }
        public override double Height { get { return 14.0018; } }

        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExtendsFineness; } }

        public override double FormDragCoefficient
        {
            get
            {
                double baseCd = GetBaseCd(0.5);
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
                double sinAlpha = Math.Sin(alpha * 2.0);
                return baseCd * sinAlpha;
            }
        }

        public override double CrossSectionalArea { get { return Math.PI * Math.Pow(Width / 2, 2); } }

        public override double ExposedSurfaceArea
        {
            get
            {
                // A = 2πrh + πr2
                return 2 * Math.PI * (Width / 2.0) * Height + CrossSectionalArea;
            }
        }

        public override double LiftingSurfaceArea { get { return Width * Height; } }

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
