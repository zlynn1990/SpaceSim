using System;
using System.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Spacecrafts.DragonV2
{
    class DragonV2Trunk : SpaceCraftBase
    {
        public override string CraftName { get { return "DragonV2 Trunk"; } }
        public override string CommandFileName { get { return "dragonTrunk.xml"; } }

        public override double Width { get { return 4.9; } }
        public override double Height { get { return 4.1; } }

        public override double DryMass { get { return 1000; } }

        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExtendsFineness; } }

        public override double FormDragCoefficient
        {
            get
            {
                double baseCd = GetBaseCd(0.3);
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

        public DragonV2Trunk(string craftDirectory, DVector2 position, DVector2 velocity)
            : base(craftDirectory, position, velocity, 0, 0, "Dragon/V2/trunk.png")
        {
            StageOffset = new DVector2(0, 4);

            Engines = new IEngine[0];
        }
    }
}
