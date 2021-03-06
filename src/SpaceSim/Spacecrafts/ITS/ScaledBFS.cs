﻿using System;
using System.Drawing;
using SpaceSim.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;

using SpaceSim.Properties;
using System.IO;
using SpaceSim.Common;
using VectorMath;

namespace SpaceSim.Spacecrafts.ITS
{
    class ScaledBFS : SpaceCraftBase
    {
        public override string CraftName { get { return "Scaled BFS"; } }
        public override string CommandFileName { get { return "miniCargo.xml"; } }

        public override double DryMass { get { return 37800; } }
        public override double Width { get { return 10; } }
        public override double Height { get { return 37.125; } }

        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExtendsFineness; } }

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

        private SpriteSheet _spriteSheet;

        public ScaledBFS(string craftDirectory, DVector2 position, DVector2 velocity, double payloadMass = 0, double propellantMass = 1050000)
            : base(craftDirectory, position, velocity, payloadMass, propellantMass, null)
        {
            //StageOffset = new DVector2(0, 0);
            StageOffset = new DVector2(0, 24);

            Engines = new IEngine[6];

            // Raptor Vac engines
            for (int i = 0; i < 3; i++)
            {
                double engineOffsetX = (i - 1.0) / 1.0;
                var offset = new DVector2(engineOffsetX * Width * 0.2, Height * 0.45);
                Engines[i] = new RaptorVac2016(i, this, offset);
            }

            // Raptor SL 40 engines
            //Engines[3] = new Raptor50(3, this, new DVector2(0, Height * 0.4));

            //Engines = new IEngine[9];

            //// Mini Raptor Vac engines
            //for (int i = 0; i < 6; i++)
            //{
            //    double engineOffsetX = (i - 3.0) / 3.0;
            //    var offset = new DVector2(engineOffsetX * Width * 0.25, Height * 0.475);
            //    Engines[i] = new MiniRaptorVac(i, this, offset);
            //}

            // Mini Raptor SL 50 engines
            for (int i = 3; i < 6; i++)
            {
                double engineOffsetX = (i - 4.0) / 2.0;
                var offset = new DVector2(engineOffsetX * Width * 0.1, Height * 0.475);
                Engines[i] = new MiniRaptor50(i, this, offset);
            }

            _spriteSheet = new SpriteSheet("Textures/Spacecrafts/Its/scaledShip.png", 12, 12);

            //string texturePath = "Its/scaledBFS.png";
            //string fullPath = Path.Combine("Textures/Spacecrafts", texturePath);
            //this.Texture = new Bitmap(fullPath);

            this.payloadMass = payloadMass;
        }

        protected override void RenderShip(Graphics graphics, Camera camera, RectangleF screenBounds)
        {
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

            //graphics.DrawImage(this.Texture, screenBounds.X, screenBounds.Y, screenBounds.Width, screenBounds.Height);
            graphics.ResetTransform();

            //if (Settings.Default.WriteCsv && (DateTime.Now - timestamp > TimeSpan.FromSeconds(1)))
            //{
            //    string filename = MissionName + ".csv";

            //    if (!File.Exists(filename))
            //    {
            //        File.AppendAllText(filename, "Velocity, Acceleration, Altitude, Throttle\r\n");
            //    }

            //    timestamp = DateTime.Now;

            //    string contents = string.Format("{0}, {1}, {2}, {3}\r\n",
            //        this.GetRelativeVelocity().Length(),
            //        this.GetRelativeAcceleration().Length() * 100,
            //        this.GetRelativeAltitude() / 100,
            //        this.Throttle * 10);
            //    File.AppendAllText(filename, contents);
            //}
        }
    }
}


