using System;
using System.Drawing;
using SpaceSim.Drawing;
using SpaceSim.Engines;
using VectorMath;

using SpaceSim.Physics;
using SpaceSim.Properties;

namespace SpaceSim.Spacecrafts.NewGlenn
{
    sealed class NGS1 : SpaceCraftBase
    {
        public override string CraftName { get { return "NG S1"; } }
        public override string CommandFileName { get { return "NGS1.xml"; } }

        public override Color IconColor { get { return Color.White; } }
        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExtendsFineness; } }
        public override double DryMass { get { return 70000; } }

        public override double Width { get { return 12; } }
        public override double Height { get { return 53.0; } }

        public override double LiftingSurfaceArea { get { return Width * Height; } }

        public override double FormDragCoefficient
        {
            get
            {
                double alpha = GetAlpha();
                double baseCd = GetBaseCd(0.4);
                //bool isRetrograde = false;

                if (alpha > Constants.PiOverTwo || alpha < -Constants.PiOverTwo)
                {
                    baseCd = GetBaseCd(0.6);
                }

                double dragCoefficient = Math.Abs(baseCd * Math.Cos(alpha));
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

        private NGLandingLeg[] _landingLegs;
        DateTime timestamp = DateTime.Now;

        public NGS1(string craftDirectory, DVector2 position, DVector2 velocity, double propellantMass = 1050000)
            : base(craftDirectory, position, velocity, 0, propellantMass, "NewGlenn/ngS1.png")
        {
            StageOffset = new DVector2(0, 28.4);

            _landingLegs = new[]
            {
                new NGLandingLeg(this, new DVector2(3.3, 19.8), true),
                new NGLandingLeg(this, new DVector2(-3.3, 19.8), false)
            };

            Engines = new IEngine[7];

            for (int i = 0; i < 7; i++)
            {
                double engineOffsetX = (i - 3.0) / 3.0;

                var offset = new DVector2(engineOffsetX * Width * 0.3, Height * 0.45);

                Engines[i] = new BE4(i, this, offset);
            }
        }

        public override void DeployLandingLegs()
        {
            foreach (NGLandingLeg landingLeg in _landingLegs)
            {
                landingLeg.Deploy();
            }
        }

        public override void Update(double dt)
        {
            base.Update(dt);

            foreach (NGLandingLeg landingLeg in _landingLegs)
            {
                landingLeg.Update(dt);
            }
        }

        protected override void RenderShip(Graphics graphics, Camera camera, RectangleF screenBounds)
        {
            foreach (NGLandingLeg landingLeg in _landingLegs)
            {
                landingLeg.RenderGdi(graphics, camera);
            }

            double drawingRotation = Pitch + Math.PI * 0.5;

            var offset = new PointF(screenBounds.X + screenBounds.Width * 0.5f,
                screenBounds.Y + screenBounds.Height * 0.5f);

            graphics.TranslateTransform(offset.X, offset.Y);

            float pitchAngle = (float)(drawingRotation * 180 / Math.PI);

            graphics.RotateTransform(pitchAngle);
            graphics.TranslateTransform(-offset.X, -offset.Y);

            int heatingRate = Math.Min((int)this.HeatingRate, 1000000);
            if (heatingRate > 50000)
            {
                Random rnd = new Random();
                float noise = (float)rnd.NextDouble() * 5;
                float width = screenBounds.Width / (3 + noise);
                float height = screenBounds.Height / (18 + noise);
                RectangleF plasmaRect = screenBounds;
                plasmaRect.Inflate(screenBounds.Width, screenBounds.Height / 12);

                int alpha = Math.Min(heatingRate / 3900, 255);
                int red = alpha;
                int green = Math.Max(red - 128, 0) * 2;
                int blue = 0;
                Color glow = Color.FromArgb(alpha, red, green, blue);

                float penWidth = width / 12;
                Pen glowPen = new Pen(glow, penWidth);
                glowPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                glowPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                graphics.DrawArc(glowPen, plasmaRect, 40, 100);

                glowPen.Color = Color.FromArgb((int)(alpha * 0.75), glow);
                plasmaRect.Inflate(-penWidth, -penWidth);
                graphics.DrawArc(glowPen, plasmaRect, 20, 140);

                glowPen.Color = Color.FromArgb((int)(alpha * 0.5), glow);
                plasmaRect.Inflate(-penWidth, -penWidth);
                graphics.DrawArc(glowPen, plasmaRect, 0, 180);

                glowPen.Color = Color.FromArgb((int)(alpha * 0.25), glow);
                plasmaRect.Inflate(-penWidth, -penWidth);
                graphics.DrawArc(glowPen, plasmaRect, -20, 220);
            }

            graphics.DrawImage(Texture, screenBounds.X, screenBounds.Y, screenBounds.Width, screenBounds.Height);
            graphics.ResetTransform();

            //    if (Settings.Default.WriteCsv && (DateTime.Now - timestamp > TimeSpan.FromSeconds(1)))
            //    {
            //        string filename = MissionName + ".csv";

            //        if (!File.Exists(filename))
            //        {
            //            File.AppendAllText(filename, "Velocity, VelocityX, VelocityY, Acceleration, Altitude, Throttle\r\n");
            //        }

            //        timestamp = DateTime.Now;

            //        string contents = string.Format("{0}, {1}, {2}, {3}, {4}, {5}\r\n",
            //            this.GetRelativeVelocity().Length(),
            //            -this.GetRelativeVelocity().X,
            //            -this.GetRelativeVelocity().Y,
            //            this.GetRelativeAcceleration().Length() * 100,
            //            this.GetRelativeAltitude() / 100,
            //            this.Throttle * 10);
            //        File.AppendAllText(filename, contents);
            //    }
        }
    }
}
