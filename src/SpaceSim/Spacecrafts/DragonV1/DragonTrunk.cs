using System;
using System.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Spacecrafts.DragonV1
{
    class DragonTrunk : SpaceCraftBase
    {
        public override string CraftName { get { return "Dragon Trunk"; } }
        public override string CommandFileName { get { return "dragonTrunk.xml"; } }

        public override double Width { get { return 3.8754; } }
        public override double Height { get { return 3.253; } }

        public override double DryMass { get { return 1000; } }

        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExtendsFineness; } }

        public override double FormDragCoefficient
        {
            get
            {
                double baseCd = GetBaseCd(0.3);
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

        // Cylinder - 2 * pi * r * h
        public override double FrontalArea { get { return 27.6579; } }

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

        public DragonTrunk(string craftDirectory, DVector2 position, DVector2 velocity)
            : base(craftDirectory, position, velocity, 0, 0, "Textures/dragonTrunk.png")
        {
            StageOffset = new DVector2(0, 2.8);

            Engines = new IEngine[0];
        }
    }
}
