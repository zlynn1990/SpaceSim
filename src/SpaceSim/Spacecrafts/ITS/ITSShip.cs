using System;
using System.Drawing;
using SpaceSim.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Spacecrafts.ITS
{
    class ITSShip : SpaceCraftBase
    {
        public override string CraftName { get { return "ITS Spaceship"; } }
        public override string CommandFileName { get { return "itsShip.xml"; } }

        public override double DryMass { get { return 136078; } }

        public override double Width { get { return 13.9; } }
        public override double Height { get { return 49.5; } }

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

        private SpriteSheet _spriteSheet;

        public ITSShip(string craftDirectory, DVector2 position, DVector2 velocity, double propellantMass = 1769010)
            : base(craftDirectory, position, velocity, propellantMass, null)
        {
            Engines = new IEngine[9];

            // Raptor Vac engines
            for (int i = 0; i < 6; i++)
            {
                double engineOffsetX = (i - 2.5) / 2.5;

                var offset = new DVector2(engineOffsetX * Width * 0.2, Height * 0.45);

                Engines[i] = new RaptorVac(i, this, offset);
            }

            // Raptor SL engines
            Engines[6] = new Raptor(6, this, new DVector2(-2, Height * 0.45));
            Engines[7] = new Raptor(7, this, new DVector2(0, Height * 0.45));
            Engines[8] = new Raptor(8, this, new DVector2(2, Height * 0.45));

            _spriteSheet = new SpriteSheet("Textures/itsShip.png", 6, 6);
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
            int rollAngle = (int)(Roll * MathHelper.RadiansToDegrees + 90) % 360;

            // Index into the sprite
            int spriteIndex = (rollAngle * 36) / 360;

            _spriteSheet.Draw(spriteIndex, graphics, screenBounds);

            graphics.ResetTransform();
        }

        public override void Update(double dt)
        {
            Roll += dt;
            base.Update(dt);
        }
    }
}
