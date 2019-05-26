using System;
using System.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Spacecrafts.DragonV2
{
    class Interstage : SpaceCraftBase
    {
        public override string CraftName { get { return "Interstage"; } }
        public override string CommandFileName { get { return "Interstage.xml"; } }

        public override double Width { get { return 4.5; } }
        public override double Height { get { return 3.0; } }
        //public override double Height { get { return 5.0; } }

        public override double DryMass { get { return 0; } }

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

        public Interstage(string craftDirectory, DVector2 position, DVector2 velocity, double yOffset = 3.0)
            : base(craftDirectory, position, velocity, 0, 0, "SLS/Interstage.png")
        {
            StageOffset = new DVector2(0, yOffset);

            Engines = new IEngine[]{};
        }
    }
}
