using System;
using System.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;
using SpaceSim.Common;

namespace SpaceSim.Spacecrafts.Electron
{
    class ElectronS2 : SpaceCraftBase
    {
        public override string CraftName { get { return "Electron Stage 2"; } }
        public override string CommandFileName { get { return "ElectronS2.xml"; } }

        public override double DryMass { get { return 140; } }

        public override double Width { get { return 1.2; } }
        public override double Height { get { return 3.77142; } }

        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExtendsFineness; } }

        public override double LiftCoefficient
        {
            get
            {
                double baseCd = GetBaseCd(0.4);
                double alpha = GetAlpha();

                return baseCd * Math.Sin(alpha * 2);
            }
        }

        public override double FormDragCoefficient
        {
            get
            {
                double alpha = GetAlpha();
                double baseCd = GetBaseCd(0.4);

                if (alpha > Constants.PiOverTwo || alpha < -Constants.PiOverTwo)
                {
                    baseCd = GetBaseCd(0.7);
                }

                baseCd *= Math.Cos(alpha);

                return Math.Abs(baseCd);
            }
        }

        public override double StagingForce { get { return 100; } }

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

        public override double LiftingSurfaceArea
        {
            get
            {
                return Width * Height;
            }
        }

        public override Color IconColor
        {
            get
            {
                return Color.White;
            }
        }

        public ElectronS2(string craftDirectory, DVector2 position, DVector2 velocity, double propellantMass = 2050)
            : base(craftDirectory, position, velocity, 0, propellantMass, "Electron/ElectronS2.png")
        {
            StageOffset = new DVector2(0, 2.0);

            Engines = new IEngine[]
            {
                new RutherfordVac(this, new DVector2(0, Height * 0.4)), 
            };
        }
    }
}
