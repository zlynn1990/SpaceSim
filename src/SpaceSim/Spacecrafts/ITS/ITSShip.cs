using System;
using System.Diagnostics;
using System.Drawing;
using SpaceSim.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;
using System.IO;

namespace SpaceSim.Spacecrafts.ITS
{
    class ITSShip : SpaceCraftBase
    {
        public override string CraftName { get { return "ITS Spaceship"; } }
        public override string CommandFileName { get { return "itsShip.xml"; } }

        public override double DryMass { get { return 150000 + payloadMass; } }

        public override double Width { get { return 13; } }
        public override double Height { get { return 49.5; } }

        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExtendsFineness; } }

        DateTime timestamp = DateTime.Now;
        double payloadMass = 0;

        public override double LiftingSurfaceArea { get { return Math.Abs(Width * Height * Math.Cos(GetAlpha())); } }

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
                double alpha = GetAlpha();
                double crossSectionalArea = Math.PI * Math.Pow(Width / 2, 2);
                double sideArea = Width * Height;

                return Math.Abs(crossSectionalArea * Math.Cos(alpha)) + Math.Abs(sideArea * Math.Sin(alpha));
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
                    baseCd = GetBaseCd(0.6);
                    isRetrograde = true;
                }

                double dragCoefficient = Math.Abs(baseCd * Math.Sin(alpha));
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

        private SpriteSheet _spriteSheet;

        public ITSShip(string craftDirectory, DVector2 position, DVector2 velocity, double payloadMass, double propellantMass = 1769010)
            : base(craftDirectory, position, velocity, payloadMass, propellantMass, null)
        {
            Engines = new IEngine[9];

            // Raptor Vac engines
            for (int i = 0; i < 6; i++)
            {
                double engineOffsetX = (i - 2.5) / 2.5;

                var offset = new DVector2(engineOffsetX * Width * 0.25, Height * 0.48);

                Engines[i] = new RaptorVac(i, this, offset);
            }

            // Raptor SL 50 engines
            Engines[6] = new Raptor50(6, this, new DVector2(-2, Height * 0.48));
            Engines[7] = new Raptor50(7, this, new DVector2(0, Height * 0.48));
            Engines[8] = new Raptor50(8, this, new DVector2(2, Height * 0.48));

            _spriteSheet = new SpriteSheet("Textures/Spacecraft/Its/ship.png", 12, 12);

            this.payloadMass = payloadMass;
        }

        protected override void RenderShip(Graphics graphics, RectangleD cameraBounds, RectangleF screenBounds)
        {
            double drawingRotation = Pitch + Math.PI * 0.5;

            var offset = new PointF(screenBounds.X + screenBounds.Width * 0.5f,
                                    screenBounds.Y + screenBounds.Height * 0.5f);

            graphics.TranslateTransform(offset.X, offset.Y);

            float pitchAngle = (float)(drawingRotation * 180 / Math.PI);
            float rollFactor = (float)Math.Cos(Roll);
            float alphaAngle = (float)(GetAlpha() * 180 / Math.PI);
            float rotateAngle = (pitchAngle - alphaAngle) + alphaAngle * rollFactor;

            if(this.MissionName.Contains("EDL") || this.MissionName.Contains("Aerocapture") || this.MissionName.Contains("Direct"))
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

            //if (DateTime.Now - timestamp > TimeSpan.FromSeconds(1))
            //{
            //    string filename = MissionName + ".csv";

            //    if (!File.Exists(filename))
            //    {
            //        File.AppendAllText(filename, "Ma, FormDragCoefficient, SkinFrictionCoefficient, LiftCoefficient, pitchAngle\r\n");
            //    }

            //    timestamp = DateTime.Now;
            //    string contents = string.Format("{0:N3}, {1:N3}, {2:N3}, {3:N3},  {4:N3}\r\n",
            //        MachNumber, FormDragCoefficient, SkinFrictionCoefficient, LiftCoefficient, pitchAngle);
            //    File.AppendAllText(filename, contents);
            //}
        }
    }
}
