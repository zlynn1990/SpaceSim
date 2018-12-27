using System.Drawing;
using SpaceSim.Spacecrafts;
using VectorMath;

using System;
using SpaceSim.Physics;
using SpaceSim.Engines;
using SpaceSim.Drawing;

namespace SpaceSim.Spacecrafts.Electron
{
    class ElectronBattery : SpaceCraftBase
    {
        public override Color IconColor { get { return Color.White; } }

        public override double Width { get { return 0.3; } }
        public override double Height { get { return 0.3; } }

        public ElectronBattery(string craftDirectory, DVector2 position, DVector2 velocity, bool isLeft)
            : base(craftDirectory, position, velocity, 0, 0, isLeft ? "Electron/batteryLeft.png" : "Electron/batteryRight.png")
        {
            _isLeft = isLeft;

            if (_isLeft)
            {
                StageOffset = new DVector2(-0.28, 2.6);
            }
            else
            {
                StageOffset = new DVector2(0.28, 2.6);
            }

            Engines = new IEngine[0];
        }

        public override string CraftName { get { return _isLeft ? "Battery Left" : "Battery Right"; } }
        public override string CommandFileName { get { return _isLeft ? "batteryLeft.xml" : "batteryRight.xml"; } }

        public override double DryMass { get { return 25; } }

        public override AeroDynamicProperties GetAeroDynamicProperties
        {
            get { return AeroDynamicProperties.None; }
        }

        public override double StagingForce
        {
            get { return 10; }
        }

        public override double FormDragCoefficient
        {
            get
            {
                double baseCd = GetBaseCd(0.4);
                double alpha = GetAlpha();
                return baseCd * Math.Cos(alpha);
            }
        }

        public override double LiftCoefficient
        {
            get
            {
                double baseCd = GetBaseCd(0.6);
                double alpha = GetAlpha();
                return baseCd * Math.Sin(alpha * 2.0);
            }
        }

        public override double FrontalArea { get { return Math.PI * Math.Pow(Width / 2, 2); } }
        public override double ExposedSurfaceArea { get { return 2 * Math.PI * (Width / 2) * Height + FrontalArea; } }
        public override double LiftingSurfaceArea { get { return Width * Height; } }

        private bool _isLeft;
        private bool _isHidden;

        public void Hide()
        {
            _isHidden = true;
        }

        public override void RenderGdi(Graphics graphics, Camera camera)
        {
            if (_isHidden) return;

            base.RenderGdi(graphics, camera);
        }
    }
}
