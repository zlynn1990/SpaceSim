using System;
using System.Drawing;
using SpaceSim.Engines;
using VectorMath;
using SpaceSim.Common;
using SpaceSim.Physics;

namespace SpaceSim.Spacecrafts.DeltaIV
{
    class CommonBoosterCore : SpaceCraftBase
    {
        public override string CraftName { get { return "CBC"; } }
        public override Color IconColor { get { return Color.White; } }
        public override string CommandFileName { get { return "CBC.xml"; } }

        public override double DryMass { get { return 26760; } }

        public override double Width { get { return 5.1; } }
        public override double Height { get { return 40.8; } }

        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExtendsFineness; } }

        public override double FormDragCoefficient
        {
            get
            {
                double alpha = GetAlpha();
                double baseCd = GetBaseCd(0.4);
                bool isRetrograde = false;

                if (alpha > Constants.PiOverTwo || alpha < -Constants.PiOverTwo)
                {
                    baseCd = GetBaseCd(0.6);
                    isRetrograde = true;
                }

                double dragCoefficient = Math.Abs(baseCd * Math.Cos(alpha));
                double dragPreservation = 1.0;

                if (isRetrograde)
                {
                    // if retrograde
                    if (Throttle > 0 && MachNumber > 1.5 && MachNumber < 20.0)
                    {
                        double throttleFactor = Throttle / 50;
                        double cantFactor = Math.Sin(Engines[0].Cant * 2);
                        dragPreservation += throttleFactor * cantFactor;
                        dragCoefficient *= dragPreservation;
                    }
                }

                return Math.Abs(dragCoefficient);
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

        public override double FrontalArea
        {
            get
            {
                double area = Math.PI * Math.Pow(Width / 2, 2);
                double alpha = GetAlpha();

                return Math.Abs(area * Math.Cos(alpha));
            }
        }

        public override double ExposedSurfaceArea
        {
            get
            {
                // A = 2πrh + πr2
                return 2 * Math.PI * (Width / 2) * Height + FrontalArea;
            }
        }

        public override double LiftingSurfaceArea { get { return Width * Height; } }

        public CommonBoosterCore(string craftDirectory, DVector2 position, DVector2 velocity, double propellantMass = 199640, string texturePath = "DeltaIV/core.png")
            : base(craftDirectory, position, velocity, 0, propellantMass, texturePath)
        {
            StageOffset = new DVector2(0, 19.4);

            Engines = new IEngine[1];
            var offset = new DVector2(0, Height * 0.48);
            Engines[0] = new RS68A(0, this, offset);
        }
    }
}
