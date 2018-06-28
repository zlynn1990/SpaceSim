using System;
using System.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;
using System.IO;
using SpaceSim.Common;
using SpaceSim.Drawing;
using SpaceSim.Properties;

namespace SpaceSim.Spacecrafts.ITS
{
    class BFS300 : SpaceCraftBase
    {
        public override string CraftName { get { return "BFS"; } }
        public override string CommandFileName { get { return "BFS.xml"; } }

        public override double DryMass { get { return 85000; } }
        //public override double DryMass { get { return 65000; } }
        public override double Width { get { return 9; } }
        public override double Height { get { return 48; } }

        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExposedToAirFlow; } }

        DateTime timestamp = DateTime.Now;
        double payloadMass = 0;

        public override double LiftingSurfaceArea { get { return Math.Abs(Width * Height * Math.Cos(GetAlpha())); } }

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
                double baseCd = GetBaseCd(0.5);
                bool isRetrograde = false;

                if (alpha > Constants.PiOverTwo || alpha < -Constants.PiOverTwo)
                {
                    baseCd = GetBaseCd(0.8);
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

        //private SpriteSheet _spriteSheet;

        public BFS300(string craftDirectory, DVector2 position, DVector2 velocity, double payloadMass = 0, double propellantMass = 1100000)
            : base(craftDirectory, position, velocity, payloadMass, propellantMass, null)
        {
            StageOffset = new DVector2(0, 0);

            Engines = new IEngine[7];

            // Raptor Vac engines
            for (int i = 0; i < 4; i++)
            {
                double engineOffsetX = (i - 1.5) / 1.5;
                var offset = new DVector2(engineOffsetX * Width * 0.25, Height * 0.45);
                Engines[i] = new RaptorVac300(i, this, offset);
            }

            // Raptor SL engines
            for (int i = 4; i < 7; i++)
            {
                double engineOffsetX = (i - 4.0) / 2.0;
                var offset = new DVector2(engineOffsetX * Width * 0.1, Height * 0.475);
                Engines[i] = new RaptorSL300(i, this, offset);
            }

            //_spriteSheet = new SpriteSheet("Textures/Spacecrafts/Its/scaledShip.png", 12, 12);

            string texturePath = "Its/BFS.png";
            string fullPath = Path.Combine("Textures/Spacecrafts", texturePath);
            this.Texture = new Bitmap(fullPath);

            this.payloadMass = payloadMass;
        }

        protected override void RenderShip(Graphics graphics, Camera camera, RectangleF screenBounds)
        {
            double drawingRotation = Pitch + Math.PI * 0.5;

            var offset = new PointF(screenBounds.X + screenBounds.Width * 0.5f,
                                    screenBounds.Y + screenBounds.Height * 0.5f);

            camera.ApplyScreenRotation(graphics);
            camera.ApplyRotationMatrix(graphics, offset, drawingRotation);

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

            graphics.DrawImage(this.Texture, screenBounds.X, screenBounds.Y, screenBounds.Width, screenBounds.Height);

            // Index into the sprite
            //int ships = _spriteSheet.Cols * _spriteSheet.Rows;
            //int spriteIndex = (rollAngle * ships) / 360;
            //while (spriteIndex < 0)
            //    spriteIndex += ships;

            //_spriteSheet.Draw(spriteIndex, graphics, screenBounds);

            graphics.ResetTransform();

            if (Settings.Default.WriteCsv && (DateTime.Now - timestamp > TimeSpan.FromSeconds(1)))
            {
                string filename = MissionName + ".csv";

                if (!File.Exists(filename))
                {
                    File.AppendAllText(filename, "Velocity, Acceleration, Altitude, Throttle\r\n");
                }

                timestamp = DateTime.Now;

                string contents = string.Format("{0}, {1}, {2}, {3}\r\n",
                    this.GetRelativeVelocity().Length(),
                    this.GetRelativeAcceleration().Length() * 100,
                    this.GetRelativeAltitude() / 100,
                    this.Throttle * 10);
                File.AppendAllText(filename, contents);
            }
        }
    }
}

