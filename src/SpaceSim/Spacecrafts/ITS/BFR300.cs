using System;
using System.Drawing;
using SpaceSim.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;
using System.IO;
using SpaceSim.Common;
using SpaceSim.Spacecrafts.FalconCommon;

namespace SpaceSim.Spacecrafts.ITS
{
    class BFR300 : SpaceCraftBase
    {
        public override string CraftName { get { return "BFR"; } }
        public override string CommandFileName { get { return "BFR.xml"; } }

        public override double DryMass { get { return 130000; } }
        public override double Width { get { return 9; } }
        public override double Height { get { return 68; } }

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
                    if (_gridFins[0].Pitch > 0)
                    {
                        baseCd = GetBaseCd(0.8);
                    }
                    else
                    {
                        baseCd = GetBaseCd(0.6);
                    }

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

        private GridFin[] _gridFins;

        //private SpriteSheet _spriteSheet;

        public BFR300(string craftDirectory, DVector2 position, DVector2 velocity, double propellantMass = 3470000)
            : base(craftDirectory, position, velocity, 0, propellantMass, "Its/BFB2.png")
        {
            StageOffset = new DVector2(0, 58.3);

            _gridFins = new[]
            {
                new GridFin(this, new DVector2(2.7, -31.1), 6, true),
                new GridFin(this, new DVector2(-2.7, -31.1), 6, false)
            };

            Engines = new IEngine[31];

            for (int i = 0; i < 31; i++)
            {
                double engineOffsetX = (i - 15.5) / 15.5;

                var offset = new DVector2(engineOffsetX * Width * 0.4, Height * 0.48);

                Engines[i] = new RaptorSL300(i, this, offset);
            }

            //_spriteSheet = new SpriteSheet("Textures/Spacecraft/Its/booster.png", 4, 12);

            string texturePath = "Its/BFB2.png";
            string fullPath = Path.Combine("Textures/Spacecrafts", texturePath);
            this.Texture = new Bitmap(fullPath);
        }

        public override void DeployGridFins()
        {
            foreach (GridFin gridFin in _gridFins)
            {
                gridFin.Deploy();
            }
        }

        public override void Update(double dt)
        {
            base.Update(dt);

            foreach (GridFin gridFin in _gridFins)
            {
                gridFin.Update(dt);
            }
        }

        protected override void RenderShip(Graphics graphics, Camera camera, RectangleF screenBounds)
        {
            double drawingRotation = Pitch + Math.PI * 0.5;

            var offset = new PointF(screenBounds.X + screenBounds.Width * 0.5f,
                                    screenBounds.Y + screenBounds.Height * 0.5f);

            camera.ApplyScreenRotation(graphics);
            camera.ApplyRotationMatrix(graphics, offset, drawingRotation);

            graphics.DrawImage(this.Texture, screenBounds.X, screenBounds.Y, screenBounds.Width, screenBounds.Height);
            graphics.ResetTransform();

            foreach (GridFin gridFin in _gridFins)
            {
                gridFin.RenderGdi(graphics, camera);
            }
        }
    }
}

