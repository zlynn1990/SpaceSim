using System;
using System.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Spacecrafts.Falcon9
{
    sealed class F9S2B5 : SpaceCraftBase
    {
        public override string CraftName { get { return "F9 S2"; } }
        public override string CommandFileName { get { return "F9S2B5.xml"; } }

        public override double DryMass { get { return 5000; } }

        public override double Width { get { return 3.706; } }
        public override double Height { get { return 14.0018; } }

        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExtendsFineness; } }

        public override double FormDragCoefficient
        {
            get
            {
                double baseCd = GetBaseCd(0.5);
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
                return 2 * Math.PI * (Width / 2.0) * Height + FrontalArea;
            }
        }

        public override double LiftingSurfaceArea { get { return Width * Height; } }

        public override Color IconColor { get { return Color.White; } }

        public F9S2B5(string craftDirectory, DVector2 position, DVector2 velocity, double stageOffset, double propellantMass = 103500)
            : base(craftDirectory, position, velocity, 0, propellantMass, "Falcon/9/S2.png")
        {
            StageOffset = new DVector2(0, stageOffset);

            Engines = new IEngine[]
            {
                new Merlin1DVac(this, new DVector2(0, Height * 0.3))
            };
        }
    }
}

