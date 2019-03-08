using System;
using System.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;
using SpaceSim.Common;
using SpaceSim.Drawing;

namespace SpaceSim.Spacecrafts.Electron
{
    class ElectronS1 : SpaceCraftBase
    {
        public override string CraftName { get { return "Electron Stage 1"; } }
        public override string CommandFileName { get { return "ElectronS1.xml"; } }

        public override double DryMass { get { return 950; } }

        public override double Width { get { return 1.2; } }
        public override double Height { get { return 15.4928; } }

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

        public override double FormDragCoefficient
        {
            get
            {
                double alpha = GetAlpha();
                double baseCd = GetBaseCd(0.3);

                if (alpha > Constants.PiOverTwo || alpha < -Constants.PiOverTwo)
                {
                    baseCd = GetBaseCd(0.6);
                }

                baseCd *= Math.Cos(alpha);

                return Math.Abs(baseCd);
            }
        }

        public override double StagingForce { get { return 1000; } }

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

        public override double LiftingSurfaceArea
        {
            get
            {
                return Width * Height;
            }
        }

        public override Color IconColor
        {
            get
            {
                return Color.White;
            }
        }

        public ElectronS1(string craftDirectory, DVector2 position, DVector2 velocity, double propellantMass = 9250)
            : base(craftDirectory, position, velocity, 0, propellantMass, "Electron/ElectronS1.png")
        {
            StageOffset = new DVector2(0, 8.0);

            Engines = new IEngine[9];

            for (int i = 0; i < 9; i++)
            {
                double engineOffsetX = (i - 4.0) / 4.5;

                var offset = new DVector2(engineOffsetX * Width * 0.3, Height * 0.45);

                Engines[i] = new Rutherford(i, this, offset);
            }
        }

        protected override void RenderShip(Graphics graphics, Camera camera, RectangleF screenBounds)
        {
            double drawingRotation = Pitch + Math.PI * 0.5;

            var offset = new PointF(screenBounds.X + screenBounds.Width * 0.5f,
                                    screenBounds.Y + screenBounds.Height * 0.5f);

            camera.ApplyScreenRotation(graphics);
            camera.ApplyRotationMatrix(graphics, offset, drawingRotation);

            graphics.DrawImage(Texture, screenBounds.X - screenBounds.Width * 0.05f, screenBounds.Y, screenBounds.Width * 1.1f, screenBounds.Height);

            graphics.ResetTransform();
        }
    }
}
