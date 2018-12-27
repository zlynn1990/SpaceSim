using System;
using System.Drawing;
using SpaceSim.Engines;
using VectorMath;
using SpaceSim.Common;
using SpaceSim.Physics;

namespace SpaceSim.Spacecrafts.NewGlenn
{
    class NGS2 : SpaceCraftBase
    {
        public override string CraftName { get { return "NG S2"; } }
        public override string CommandFileName { get { return "NGS2.xml"; } }

        public override Color IconColor { get { return Color.White; } }
        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExtendsFineness; } }
        public override double DryMass { get { return 13000; } }

        public override double Width { get { return 7.0; } }
        public override double Height { get { return 24.6; } }

        public override double LiftingSurfaceArea { get { return Width * Height; } }

        public override double FormDragCoefficient
        {
            get
            {
                double alpha = GetAlpha();
                double baseCd = GetBaseCd(0.6);
                bool isRetrograde = false;

                if (alpha > Constants.PiOverTwo || alpha < -Constants.PiOverTwo)
                {
                    //if (_landingLegs[0].Pitch > 0)
                    //{
                    //    baseCd = GetBaseCd(1.2);
                    //}
                    //else
                    //{
                    baseCd = GetBaseCd(0.8);
                    //}

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
                double baseCd = GetBaseCd(0.4);
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

        //private LandingLeg[] _landingLegs;
        DateTime timestamp = DateTime.Now;

        //public NGS2(string craftDirectory, DVector2 position, DVector2 velocity, double propellantMass = 208000)
        public NGS2(string craftDirectory, DVector2 position, DVector2 velocity, double propellantMass = 175000)
            : base(craftDirectory, position, velocity, 0, propellantMass, "NewGlenn/ngS2.png")
        {
            StageOffset = new DVector2(0, 18.4);

            Engines = new IEngine[2];

            for (int i = 0; i < 2; i++)
            {
                var offset = new DVector2((i - 0.5) * 2.0, Height * 0.4);

                Engines[i] = new BE3U(i, this, offset);
            }
        }

        //protected override void RenderShip(Graphics graphics, RectangleD cameraBounds, RectangleF screenBounds)
        //{
        //    if (Settings.Default.WriteCsv && (DateTime.Now - timestamp > TimeSpan.FromSeconds(1)))
        //    {
        //        string filename = MissionName + ".csv";

        //        if (!File.Exists(filename))
        //        {
        //            File.AppendAllText(filename, "Velocity, Acceleration, Altitude\r\n");
        //        }

        //        timestamp = DateTime.Now;
        //        string contents = string.Format("{0}, {1}, {2}\r\n",
        //            this.GetRelativeVelocity().Length(), this.GetRelativeAcceleration().Length() * 100, this.GetRelativeAltitude() / 100);
        //        File.AppendAllText(filename, contents);
        //    }

        //    base.RenderShip(graphics, cameraBounds, screenBounds);
        //}
    }
}
