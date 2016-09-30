using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using SpaceSim.Engines;
using SpaceSim.Particles;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Spacecrafts.ITS
{
    class ITSBooster : SpaceCraftBase
    {
        public override string CraftName { get { return "ITS Booster"; } }
        public override string CommandFileName { get { return "itsBooster.xml"; } }

        public override double DryMass { get { return 249476; } }

        public override double Width { get { return 12; } }
        public override double Height { get { return 77.5; } }

        public override double LiftingSurfaceArea { get { return Width * Height; } }
        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExtendsFineness; } }

        public override double LiftCoefficient
        {
            get
            {
                double baseCd = GetBaseCd(0.6);
                double alpha = GetAlpha();

                return baseCd * Math.Sin(alpha * 2);
            }
        }

        public override double CrossSectionalArea
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
                return 2 * Math.PI * (Width / 2) * Height + CrossSectionalArea;
            }
        }

        public override Color IconColor
        {
            get { return Color.White; }
        }

        public override double FormDragCoefficient
        {
            get
            {
                double alpha = GetAlpha();
                double baseCd = GetBaseCd(0.4);
                bool isRetrograde = false;

                if (alpha > Constants.PiOverTwo || alpha < -Constants.PiOverTwo)
                {
                    baseCd = GetBaseCd(0.8);

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

        public ITSBooster(string craftDirectory, DVector2 position, DVector2 velocity, double propellantMass = 6078138)
            : base(craftDirectory, position, velocity, propellantMass, "Textures/itsBooster.png")
        {
            StageOffset = new DVector2(0, 61);

            Engines = new IEngine[42];

            for (int i = 0; i < 42; i++)
            {
                double engineOffsetX = (i - 21.0) / 21.0;

                var offset = new DVector2(engineOffsetX * Width * 0.38, Height * 0.45);

                Engines[i] = new Raptor(i, this, offset);
            }
        }
    }
}
