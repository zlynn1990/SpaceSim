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
        public override string CraftName { get { return "Starship"; } }
        public override string CommandFileName { get { return "BFS.xml"; } }

        public override double DryMass { get { return 90000; } }
        public override double Width { get { return 9; } }
        public override double Height { get { return 50; } }

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
                    baseCd = GetBaseCd(0.7);
                    isRetrograde = true;
                }

                double dragCoefficient = Math.Abs(baseCd * Math.Sin(alpha));

                // account for dihedral of the fins
                int finCount = Fins.GetLength(0);
                for(int i = 0; i < finCount; i++)
                {
                    double finDragCoefficient = Math.Abs(Math.Cos(Fins[i].Dihedral));
                    switch(i)
                    {
                        case 0:
                            dragCoefficient *= 1 + finDragCoefficient * 0.075;
                            break;
                        default:
                            dragCoefficient *= 1 + finDragCoefficient * 0.34;
                            break;
                    }
                }
                
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

        public BFS300(string craftDirectory, DVector2 position, DVector2 velocity, double payloadMass = 0, double propellantMass = 1000000)
            : base(craftDirectory, position, velocity, payloadMass, propellantMass, null)
        {
            StageOffset = new DVector2(0, 0);

            Fins = new Fin[2];
            Fins[0] = new Fin(this, new DVector2(3.8, -18.0), new DVector2(2.5, 5), 0, "Textures/Spacecrafts/ITS/Canard.png", 1000.0);
            Fins[1] = new Fin(this, new DVector2(2.4, 17.2), new DVector2(5.86, 13.0), -Math.PI / 6);
            //Fins[0] = new Fin(this, new DVector2(-0.8, -18.0), new DVector2(2.5, 5), 0, "Textures/Spacecrafts/ITS/Canard2.png", 1000.0);
            //Fins[1] = new Fin(this, new DVector2(-2.4, 17.2), new DVector2(5.86, 13.0), -Math.PI / 6);

            Engines = new IEngine[7];
            for (int i = 0; i < 7; i++)
            {
                double engineOffsetX = (i - 3.0) / 3.0;
                var offset = new DVector2(engineOffsetX * Width * 0.2, Height * 0.4);
                Engines[i] = new RaptorSL300(i, this, offset);
            }

            //_spriteSheet = new SpriteSheet("Textures/Spacecrafts/Its/scaledShip.png", 12, 12);

            string texturePath = "Its/Starship.png";
            string fullPath = Path.Combine("Textures/Spacecrafts", texturePath);
            this.Texture = new Bitmap(fullPath);

            this.payloadMass = payloadMass;
        }

        public override void Update(double dt)
        {
            base.Update(dt);

            foreach (Fin fin in Fins)
            {
                fin.Update(dt);
            }
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
                
                // vary the bow shock width with velocity
                double mach = this.MachNumber;
                double theta = Math.PI / (2 * mach);
                float width = screenBounds.Width * (float)Math.Sin(theta) * (10 + noise);
                float height = screenBounds.Height / (2 + noise / 40);
                RectangleF plasmaRect = screenBounds;
                plasmaRect.Inflate(new SizeF(width, height));

                if (rollAngle <= 90)
                {
                    plasmaRect.Offset(0.0f, screenBounds.Height / 2.25f);
                }
                else
                {
                    plasmaRect.Offset(-screenBounds.Width / 2.0f, screenBounds.Height / 2.0f);
                }

                int alpha = Math.Min(heatingRate / 10000, 255);
                int red = alpha;
                int green = 0;
                int blue = alpha;
                Color glow = Color.FromArgb(alpha, red, green, blue);

                float penWidth = width / 16;
                Pen glowPen = new Pen(glow, penWidth);
                glowPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                glowPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

                if (rollAngle <= 90)
                {
                    float startAngle = 235;
                    float sweepAngle = 32;
                    //int arcs = 20;
                    int arcs = 15;
                    for (int i = 0; i < arcs; i++)
                    {
                        glow = Color.FromArgb(alpha, (int)(red * (arcs - i) / (arcs * 1.3)), green, blue);
                        glowPen.Color = Color.FromArgb((int)(alpha * (arcs - i) / arcs), glow);
                        plasmaRect.Inflate(-penWidth, -penWidth);
                        graphics.DrawArc(glowPen, plasmaRect, startAngle - i * 6, sweepAngle + i * 6.75f);
                    }
                }
                else
                {
                    float startAngle = 265;
                    float sweepAngle = 30;
                    //int arcs = 20;
                    int arcs = 15;
                    for (int i = 0; i < arcs; i++)
                    {
                        glow = Color.FromArgb(alpha, (int)(red * (arcs - i) / (arcs * 1.3)), green, blue);
                        glowPen.Color = Color.FromArgb((int)(alpha * (arcs - i) / arcs), glow);
                        plasmaRect.Inflate(-penWidth, -penWidth);
                        try
                        {
                            graphics.DrawArc(glowPen, plasmaRect, startAngle + i * 1, sweepAngle + i * 6);
                        }
                        catch(Exception ex)
                        {
                            string message = ex.Message;
                        }
                    }
                }
            }

            if (rollAngle <= 90)
                graphics.DrawImage(this.Texture, screenBounds.X - screenBounds.Width * 0.43f, screenBounds.Y, screenBounds.Width * 1.8f, screenBounds.Height);
            else
                graphics.DrawImage(this.Texture, screenBounds.X + screenBounds.Width * 1.43f, screenBounds.Y, -screenBounds.Width * 1.8f, screenBounds.Height);

            // Index into the sprite
            //int ships = _spriteSheet.Cols * _spriteSheet.Rows;
            //int spriteIndex = (rollAngle * ships) / 360;
            //while (spriteIndex < 0)
            //    spriteIndex += ships;

            //_spriteSheet.Draw(spriteIndex, graphics, screenBounds);

            graphics.ResetTransform();

            foreach (Fin fin in Fins)
            {
                fin.RenderGdi(graphics, camera);
            }

            if (Settings.Default.WriteCsv && (DateTime.Now - timestamp > TimeSpan.FromSeconds(1)))
            {
                string filename = MissionName + ".csv";

                if (!File.Exists(filename))
                {
                    //File.AppendAllText(filename, "Velocity, Acceleration, Altitude, Throttle\r\n");
                    File.AppendAllText(filename, "Velocity, Acceleration, Altitude, Heating\r\n");
                }

                timestamp = DateTime.Now;

                string contents = string.Format("{0}, {1}, {2}, {3}\r\n",
                    this.GetRelativeVelocity().Length(),
                    this.GetRelativeAcceleration().Length() * 1000,
                    this.GetRelativeAltitude() / 10,
                    //this.Throttle * 100,
                    this.HeatingRate / 500);
                File.AppendAllText(filename, contents);
            }
        }
    }
}

