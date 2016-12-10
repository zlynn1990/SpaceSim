using System;
using System.Drawing;
using System.Drawing.Imaging;
using SpaceSim.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Spacecrafts.FalconCommon
{
    abstract class F9S1Base : SpaceCraftBase
    {
        public override double LiftingSurfaceArea { get { return Width * Height; } }

        public override Color IconColor { get { return Color.White; } }

        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExtendsFineness; } }

        public override double FormDragCoefficient
        {
            get
            {
                double alpha = GetAlpha();
                double baseCd = GetBaseCd(0.4);
                bool isRetrograde = false;

                if (alpha > Constants.PiOverTwo || alpha < -Constants.PiOverTwo)
                {
                    if (_landingLegs[0].Pitch > 0)
                    {
                        baseCd = GetBaseCd(1.4);
                    }
                    else if (_gridFins[0].Pitch > 0)
                    {
                        baseCd = GetBaseCd(1.25);
                    }
                    else
                    {
                        baseCd = GetBaseCd(0.8);
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
                return 2 * Math.PI * (Width / 2) * Height + FrontalArea;
            }
        }

        private GridFin[] _gridFins;
        private LandingLeg[] _landingLegs;

        private Bitmap _drawingBuffer;
        private Bitmap _sootTexture;

        private double _sootRatio;

        protected F9S1Base(string craftDirectory, DVector2 position, DVector2 velocity, double propellantMass, string texturePath, double finOffset = -16.3)
            : base(craftDirectory, position, velocity, 0, propellantMass, texturePath)
        {
            _gridFins = new[]
            {
                new GridFin(this, new DVector2(1.3, finOffset), true),
                new GridFin(this, new DVector2(-1.3, finOffset), false)
            };

            _landingLegs = new[]
            {
                new LandingLeg(this, new DVector2(0.94, 21), true),
                new LandingLeg(this, new DVector2(-0.94, 21), false)
            };

            string sootTexturePath = texturePath.Replace(".png", "Soot.png");

            // Initialized 'soot' texture and allocate the drawing buffer
            _sootTexture = new Bitmap(sootTexturePath);
            _drawingBuffer = new Bitmap(_sootTexture.Width, _sootTexture.Height);
        }

        public override void DeployGridFins()
        {
            foreach (GridFin gridFin in _gridFins)
            {
                gridFin.Deploy();
            }
        }

        public override void DeployLandingLegs()
        {
            foreach (LandingLeg landingLeg in _landingLegs)
            {
                landingLeg.Deploy();
            }
        }

        public override void Update(double dt)
        {
            base.Update(dt);

            foreach (GridFin gridFin in _gridFins)
            {
                gridFin.Update(dt);
            }

            foreach (LandingLeg landingLeg in _landingLegs)
            {
                landingLeg.Update(dt);
            }

            DVector2 velocity = GetRelativeVelocity();
            double altitude = GetRelativeAltitude();

            velocity.Normalize();

            DVector2 rotation = new DVector2(Math.Cos(Pitch), Math.Sin(Pitch));

            // If we are going retro-grade and firing rockets adds soot
            if (altitude < 70000 && velocity.Dot(rotation) < 0)
            {
                foreach (IEngine engine in Engines)
                {
                    if (engine.IsActive && engine.Throttle > 0)
                    {
                        _sootRatio = Math.Min(_sootRatio + 0.015 * dt, 1.0);
                    }
                }
            }
        }

        protected override void RenderShip(Graphics graphics, RectangleD cameraBounds, RectangleF screenBounds)
        {
            // Build the main texture (a combination of base and soot)
            using (Graphics graphics2 = RenderUtils.GetContext(false, _drawingBuffer))
            {
                if (_sootRatio > 0.99)
                {
                    graphics2.DrawImage(_sootTexture, new Rectangle(0, 0, _drawingBuffer.Width, _drawingBuffer.Height));
                }
                else if (_sootRatio < 0.05)
                {
                    graphics2.DrawImage(Texture, new Rectangle(0, 0, _drawingBuffer.Width, _drawingBuffer.Height));
                }
                else
                {
                    graphics2.DrawImage(Texture, new Rectangle(0, 0, _drawingBuffer.Width, _drawingBuffer.Height));

                    float[][] matrixAlpha =
                    {
                        new float[] {1, 0, 0, 0, 0},
                        new float[] {0, 1, 0, 0, 0},
                        new float[] {0, 0, 1, 0, 0},
                        new float[] {0, 0, 0, (float)_sootRatio, 0}, 
                        new float[] {0, 0, 0, 0, 1}
                    };

                    ColorMatrix colorMatrix = new ColorMatrix(matrixAlpha);

                    ImageAttributes iaAlphaBlend = new ImageAttributes();
                    iaAlphaBlend.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    graphics2.DrawImage(_sootTexture, new Rectangle(0, 0, _drawingBuffer.Width, _drawingBuffer.Height), 0, 0,
                                        _drawingBuffer.Width, _drawingBuffer.Height, GraphicsUnit.Pixel, iaAlphaBlend);
                }
            }

            double drawingRotation = Pitch + Math.PI * 0.5;

            var offset = new PointF(screenBounds.X + screenBounds.Width * 0.5f,
                                    screenBounds.Y + screenBounds.Height * 0.5f);

            graphics.TranslateTransform(offset.X, offset.Y);

            graphics.RotateTransform((float)(drawingRotation * 180 / Math.PI));

            graphics.TranslateTransform(-offset.X, -offset.Y);

            graphics.DrawImage(_drawingBuffer, screenBounds.X, screenBounds.Y, screenBounds.Width, screenBounds.Height);

            graphics.ResetTransform();

            foreach (GridFin gridFin in _gridFins)
            {
                gridFin.RenderGdi(graphics, cameraBounds);
            }

            foreach (LandingLeg landingLeg in _landingLegs)
            {
                landingLeg.RenderGdi(graphics, cameraBounds);
            }
        }
    }
}
