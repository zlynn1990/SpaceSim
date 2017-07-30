using System;
using System.Diagnostics;
using System.Drawing;
using SpaceSim.Drawing;
using SpaceSim.Engines;
using SpaceSim.Particles;
using SpaceSim.Physics;
using VectorMath;
using System.IO;

namespace SpaceSim.Spacecrafts.ITS
{
    class ScaledBFR : SpaceCraftBase
    {
        public override string CraftName { get { return "Scaled BFR"; } }
        public override string CommandFileName { get { return "miniBooster.xml"; } }

        public override double DryMass { get { return 115500; } }
        public override double Width { get { return 9; } }
        public override double Height { get { return 58.125; } }

        public override double LiftingSurfaceArea { get { return Width * Height; } }
        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExtendsFineness; } }
        DateTime timestamp = DateTime.Now;

        public override double LiftCoefficient
        {
            get
            {
                double baseCd = GetBaseCd(0.8);
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
                return 2 * Math.PI * (Width / 2) * Height + Math.PI * Math.Pow(Width / 2, 2);
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
                    // assume the grid fins are deployed
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

        //private SpriteSheet _spriteSheet;

        public ScaledBFR(string craftDirectory, DVector2 position, DVector2 velocity, double propellantMass = 2814000)
            : base(craftDirectory, position, velocity, 0, propellantMass, "Its/booster.png")
        {
            //StageOffset = new DVector2(0, 28.5);
            StageOffset = new DVector2(0, 43.5);

            Engines = new IEngine[19];

            for (int i = 0; i < 19; i++)
            {
                double engineOffsetX = (i - 9.5) / 9.5;

                var offset = new DVector2(engineOffsetX * Width * 0.34, Height * 0.48);

                Engines[i] = new Raptor40(i, this, offset);
                //Engines[i] = new Raptor50(i, this, offset);
            }

            //_spriteSheet = new SpriteSheet("Textures/Spacecraft/Its/booster.png", 4, 12);

            string texturePath = "Its/scaledBFR.png";
            string fullPath = Path.Combine("Textures/Spacecrafts", texturePath);
            this.Texture = new Bitmap(fullPath);
        }

        protected override void RenderShip(Graphics graphics, RectangleD cameraBounds, RectangleF screenBounds)
        {
            double drawingRotation = Pitch + Math.PI * 0.5;

            var offset = new PointF(screenBounds.X + screenBounds.Width * 0.5f,
                                    screenBounds.Y + screenBounds.Height * 0.5f);

            graphics.TranslateTransform(offset.X, offset.Y);

            graphics.RotateTransform((float)(drawingRotation * 180 / Math.PI));
            graphics.TranslateTransform(-offset.X, -offset.Y);

            // Normalize the angle to [0,360]
            int rollAngle = (int)(Roll * MathHelper.RadiansToDegrees) % 360;

            // Index into the sprite
            //int ships = _spriteSheet.Cols * _spriteSheet.Rows;
            //int spriteIndex = (rollAngle * ships) / 120;
            //while (spriteIndex >= ships)
            //    spriteIndex -= ships;

            //_spriteSheet.Draw(spriteIndex, graphics, screenBounds);

            graphics.DrawImage(this.Texture, screenBounds.X, screenBounds.Y, screenBounds.Width, screenBounds.Height);
            graphics.ResetTransform();

            //if (DateTime.Now - timestamp > TimeSpan.FromSeconds(1))
            //{
            //    string filename = MissionName + ".csv";

            //    if (!File.Exists(filename))
            //    {
            //        File.AppendAllText(filename, "Ma, FormDragCoefficient, SkinFrictionCoefficient, LiftCoefficient, rollAngle\r\n");
            //    }

            //    timestamp = DateTime.Now;
            //    string contents = string.Format("{0:N3}, {1:N3}, {2:N3}, {3:N3},  {4:N3}\r\n",
            //        MachNumber, FormDragCoefficient, SkinFrictionCoefficient, LiftCoefficient, rollAngle);
            //    File.AppendAllText(filename, contents);
            //}
        }
    }
}


