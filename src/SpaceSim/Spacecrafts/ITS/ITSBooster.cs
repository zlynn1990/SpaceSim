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
    class ITSBooster : SpaceCraftBase
    {
        public override string CraftName { get { return "ITS Booster"; } }
        public override string CommandFileName { get { return "itsBooster.xml"; } }

        public override double DryMass { get { return 275000; } }

        public override double Width { get { return 12; } }
        public override double Height { get { return 77.5; } }

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

        private SpriteSheet _spriteSheet;

        public ITSBooster(string craftDirectory, DVector2 position, DVector2 velocity, double propellantMass = 6700000)
            : base(craftDirectory, position, velocity, 0, propellantMass, "Its/booster.png")
        {
            StageOffset = new DVector2(0, 59.5);

            Engines = new IEngine[42];

            for (int i = 0; i < 42; i++)
            {
                double engineOffsetX = (i - 21.0) / 21.0;

                var offset = new DVector2(engineOffsetX * Width * 0.34, Height * 0.48);

                Engines[i] = new Raptor40(i, this, offset);
            }

            _spriteSheet = new SpriteSheet("Textures/Spacecraft/Its/booster.png", 4, 12);
        }

        protected override void RenderShip(Graphics graphics, Camera camera, RectangleF screenBounds)
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
            int ships = _spriteSheet.Cols * _spriteSheet.Rows;
            int spriteIndex = (rollAngle * ships) / 120;
            while (spriteIndex >= ships)
                spriteIndex -= ships;

            _spriteSheet.Draw(spriteIndex, graphics, screenBounds);

            graphics.ResetTransform();

            if (DateTime.Now - timestamp > TimeSpan.FromSeconds(1))
            {
                string filename = MissionName + ".csv";

                if (!File.Exists(filename))
                {
                    File.AppendAllText(filename, "Ma, FormDragCoefficient, SkinFrictionCoefficient, LiftCoefficient, rollAngle\r\n");
                }

                timestamp = DateTime.Now;
                string contents = string.Format("{0:N3}, {1:N3}, {2:N3}, {3:N3},  {4:N3}\r\n",
                    MachNumber, FormDragCoefficient, SkinFrictionCoefficient, LiftCoefficient, rollAngle);
                File.AppendAllText(filename, contents);
            }
        }
    }
}
