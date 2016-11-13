using System;
using System.Diagnostics;
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

        public override double DryMass { get { return 150000; } }

        public override double Width { get { return 13; } }
        public override double Height { get { return 49.5; } }

        public override double LiftingSurfaceArea { get { return Width * Height; } }
        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExtendsFineness; } }

        public override double LiftCoefficient
        {
            get
            {
                double baseCd = GetBaseCd(0.4);
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
                    baseCd = GetBaseCd(0.6);
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

            _spriteSheet = new SpriteSheet("Textures/itsShip.png", 12, 12);
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

            if(this.MissionName.Contains("EDL"))
                graphics.RotateTransform(rotateAngle);
            else
                graphics.RotateTransform(pitchAngle);

            graphics.TranslateTransform(-offset.X, -offset.Y);

            // Normalize the angle to [0,360]
            int rollAngle = (int)(Roll * MathHelper.RadiansToDegrees) % 360;

            // Index into the sprite
            int ships = _spriteSheet.Cols * _spriteSheet.Rows;
            int spriteIndex = (rollAngle * ships) / 360;
            while (spriteIndex < 0)
                spriteIndex += ships;

            _spriteSheet.Draw(spriteIndex, graphics, screenBounds);

            graphics.ResetTransform();
        }
    }
}
