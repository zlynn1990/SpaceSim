using System;
using System.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Spacecrafts.DeltaIV
{
    sealed class Star48PAM : SpaceCraftBase
    {
        public override string CraftName { get { return "Star 48"; } }
        public override string CommandFileName { get { return "Star48.xml"; } }

        public override double DryMass { get { return 132; } }

        public override double Width { get { return 1.245; } }
        public override double Height { get { return 2.235; } }

        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExtendsFineness; } }

        public override double FormDragCoefficient
        {
            get
            {
                double baseCd = GetBaseCd(0.4);
                double alpha = GetAlpha();

                return Math.Abs(baseCd * Math.Cos(alpha));
            }
        }

        public override double LiftCoefficient
        {
            get
            {
                double baseCd = GetBaseCd(0.6);
                double alpha = GetAlpha();

                return baseCd * Math.Sin(alpha * 2);
            }
        }

        public override double FrontalArea { get { return Math.PI * Math.Pow(Width / 2, 2); } }
        public override double ExposedSurfaceArea
        {
            get
            {
                // A = 2πrh + πr2
                return 2 * Math.PI * (Width / 2) * Height + FrontalArea;
            }
        }

        public override double LiftingSurfaceArea { get { return Width * Height; } }

        public override Color IconColor { get { return Color.White; } }

        public Star48PAM(string craftDirectory, DVector2 position, DVector2 velocity, double zOffset = 2)
            : base(craftDirectory, position, velocity, 0, 2011, "DeltaIV/star48.png")
        {
            StageOffset = new DVector2(0, zOffset);

            Engines = new IEngine[]
            {
                new Star48(0, this, new DVector2(0, Height * 0.33))
            };
        }
    }
}
