using System;
using System.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;

using SpaceSim.Drawing;
using SpaceSim.Spacecrafts.FalconCommon;
using SpaceSim.Properties;
using System.IO;

namespace SpaceSim.Spacecrafts.Falcon9
{
    sealed class F9S2B : SpaceCraftBase
    {
        public override string CraftName { get { return "F9 S2"; } }
        public override string CommandFileName { get { return "F9S2.xml"; } }

        public override double DryMass { get { return 5000; } }

        public override double Width { get { return 5; } }
        public override double Height { get { return 16; } }

        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExtendsFineness; } }

        public override double FormDragCoefficient
        {
            get
            {
                double baseCd = GetBaseCd(0.5);
                if (_drogueChute.IsDeployed())
                    baseCd = GetBaseCd(0.65);
                if (_parachute.IsDeployed())
                    baseCd = GetBaseCd(1.0);

                double alpha = GetAlpha();

                return Math.Abs(baseCd * Math.Cos(alpha));
            }
        }

        public override double LiftCoefficient
        {
            get
            {
                double baseCd = GetBaseCd(0.6);
                if (_parachute.IsDeployed())
                    baseCd = GetBaseCd(3.0);

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

        public override void DeployDrogues()
        {
            _drogueChute.Deploy();
        }

        public override void DeployParachutes()
        {
            _parachute.Deploy();
        }

        public override void DeployLandingLegs()
        {
            foreach (Skid skid in _skids)
            {
                skid.Deploy();
            }
        }

        private SpriteSheet _spriteSheet;
        DrogueChute _drogueChute;
        Parafoil _parachute;
        Skid[] _skids;
        DateTime timestamp = DateTime.Now;

        public F9S2B(string craftDirectory, DVector2 position, DVector2 velocity, double stageOffset, double propellantMass = 103500)
            : base(craftDirectory, position, velocity, 0, propellantMass, "Falcon/9/S2B.png")
        {
            StageOffset = new DVector2(0, stageOffset);

            _drogueChute = new DrogueChute(this, new DVector2(-2.5, 5));
            _parachute = new Parafoil(this, new DVector2(-1.5, 0.0), true);

            Engines = new IEngine[]
            {
                new Merlin1DVac(this, new DVector2(0, Height * 0.38))
            };

            _skids = new[]
            {
                new Skid(this, new DVector2(1.1, -5), true),
                new Skid(this, new DVector2(1.1, 0.5), true)
            };

            _spriteSheet = new SpriteSheet("Textures/Spacecrafts/Falcon/9/scaledShip.png", 12, 12);
        }

        public override void Update(double dt)
        {
            base.Update(dt);

            _drogueChute.Update(dt);
            _parachute.Update(dt);

            foreach (Skid skid in _skids)
            {
                skid.Update(dt);
            }
        }

        protected override void RenderShip(Graphics graphics, Camera camera, RectangleF screenBounds)
        {
            foreach (Skid skid in _skids)
            {
                if (skid.IsDeploying() || skid.IsDeployed())
                    skid.RenderGdi(graphics, camera);
            }

            double drawingRotation = Pitch + Math.PI * 0.5;

            var offset = new PointF(screenBounds.X + screenBounds.Width * 0.5f,
                                    screenBounds.Y + screenBounds.Height * 0.5f);

            graphics.TranslateTransform(offset.X, offset.Y);

            float pitchAngle = (float)(drawingRotation * 180 / Math.PI);
            float rollFactor = (float)Math.Cos(Roll);
            float alphaAngle = (float)(GetAlpha() * 180 / Math.PI);
            float rotateAngle = (pitchAngle - alphaAngle) + alphaAngle * rollFactor;

            if (this.MissionName.Contains("EDL") || this.MissionName.Contains("Aerocapture") || this.MissionName.Contains("Direct"))
                graphics.RotateTransform(rotateAngle);
            else
                graphics.RotateTransform(pitchAngle);

            graphics.TranslateTransform(-offset.X, -offset.Y);

            // Normalize the angle to [0,360]
            int rollAngle = (int)(Roll * MathHelper.RadiansToDegrees) % 360;

            int heatingRate = Math.Min((int)this.HeatingRate, 2000000);
            if (heatingRate > 100000)
            {
                Random rnd = new Random();
                float noise = (float)rnd.NextDouble();
                float width = screenBounds.Width / (3 + noise);
                float height = screenBounds.Height / (18 + noise);
                RectangleF plasmaRect = screenBounds;
                plasmaRect.Inflate(new SizeF(width, height));

                int alpha = Math.Min(heatingRate / 7800, 255);
                int red = alpha;
                int green = Math.Max(red - 128, 0) * 2;
                int blue = 0;
                Color glow = Color.FromArgb(alpha, red, green, blue);

                float penWidth = width / 12;
                Pen glowPen = new Pen(glow, penWidth);
                glowPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                glowPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                graphics.DrawArc(glowPen, plasmaRect, 220, 100);

                glowPen.Color = Color.FromArgb((int)(alpha * 0.75), glow);
                plasmaRect.Inflate(-penWidth, -penWidth);
                graphics.DrawArc(glowPen, plasmaRect, 200, 140);

                glowPen.Color = Color.FromArgb((int)(alpha * 0.5), glow);
                plasmaRect.Inflate(-penWidth, -penWidth);
                graphics.DrawArc(glowPen, plasmaRect, 180, 180);

                glowPen.Color = Color.FromArgb((int)(alpha * 0.25), glow);
                plasmaRect.Inflate(-penWidth, -penWidth);
                graphics.DrawArc(glowPen, plasmaRect, 160, 220);
            }

            // Index into the sprite
            int ships = _spriteSheet.Cols * _spriteSheet.Rows;
            int spriteIndex = (rollAngle * ships) / 360;
            while (spriteIndex < 0)
                spriteIndex += ships;

            _spriteSheet.Draw(spriteIndex, graphics, screenBounds);

            graphics.ResetTransform();

            if (_drogueChute.IsDeploying() || _drogueChute.IsDeployed())
                _drogueChute.RenderGdi(graphics, camera);

            if (_parachute.IsDeploying() || _parachute.IsDeployed())
                _parachute.RenderGdi(graphics, camera);

            if (Settings.Default.WriteCsv && (DateTime.Now - timestamp > TimeSpan.FromSeconds(1)))
            {
                string filename = MissionName + ".csv";

                if (!File.Exists(filename))
                {
                    File.AppendAllText(filename, "Velocity, Acceleration, Altitude, Alpha\r\n");
                }

                timestamp = DateTime.Now;

                string contents = string.Format("{0}, {1}, {2}, {3}\r\n",
                    this.GetRelativeVelocity().Length(),
                    this.GetRelativeAcceleration().Length() * 100,
                    this.GetRelativeAltitude() / 10,
                    this.GetAlpha() * 10);
                File.AppendAllText(filename, contents);
            }
        }
    }
}
