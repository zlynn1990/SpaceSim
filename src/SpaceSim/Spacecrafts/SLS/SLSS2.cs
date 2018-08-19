using System;
using System.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Spacecrafts.FalconHeavy
{
    sealed class SLSS2 : SpaceCraftBase
    {
        public override string CraftName { get { return "SLS ICPS "; } }
        public override string CommandFileName { get { return "SLSS2.xml"; } }

        public override double DryMass { get { return 3490; } }

        public override double Width { get { return 5.1; } }
        public override double Height { get { return 13.7; } }

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

        public SLSS2(string craftDirectory, DVector2 position, DVector2 velocity, double zOffset = 9)
            : base(craftDirectory, position, velocity, 0, 27220, "DIVH/S2.png")
        {
            StageOffset = new DVector2(0, zOffset);

            Engines = new IEngine[]
            {
                new RL10(0, this, new DVector2(0, Height * 0.33))
            };
        }
    }
}

