using System;
using System.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Spacecrafts.FalconHeavy
{
    sealed class FHS2 : SpaceCraftBase
    {
        public override string CraftName { get { return "FH S2 "; } }
        public override string CommandFileName { get { return "FHStage2.xml"; } }

        public override double DryMass { get { return 3950; } }

        public override double Width { get { return 3.706; } }
        public override double Height { get { return 14.0018; } }

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

        public FHS2(string craftDirectory, DVector2 position, DVector2 velocity)
            : base(craftDirectory, position, velocity, 0, 108185, "Textures/fh9S2.png")
        {
            StageOffset = new DVector2(0, 9);

            Engines = new IEngine[]
            {
                new Merlin1DVac(this, new DVector2(0, Height * 0.3))
            };
        }
    }
}
