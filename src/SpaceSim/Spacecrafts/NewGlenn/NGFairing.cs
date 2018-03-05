using System;
using System.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;

using System.IO;
using SpaceSim.Drawing;

namespace SpaceSim.Spacecrafts.NewGlenn
{
    class NGFairing : SpaceCraftBase
    {
        public override string CraftName { get { return _isLeft ? "Fairing Left" : "Fairing Right"; } }
        public override string CommandFileName { get { return _isLeft ? "fairingLeft.xml" : "fairingRight.xml"; } }

        public override double Width { get { return 3.55; } }
        public override double Height { get { return 18.0; } }

        public override double DryMass { get { return 875; } }

        public override AeroDynamicProperties GetAeroDynamicProperties
        {
            get { return AeroDynamicProperties.ExposedToAirFlow; }
        }

        public override double StagingForce
        {
            get { return 1500; }
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

        public override Color IconColor { get { return Color.White; } }

        private bool _isLeft;
        private bool _isHidden;

        public void Hide()
        {
            _isHidden = true;
        }

        public NGFairing(string craftDirectory, DVector2 position, DVector2 velocity, bool isLeft)
            : base(craftDirectory, position, velocity, 0, 0, isLeft ? "NewGlenn/fairingLeft.png" : "NewGlenn/fairingRight.png", null)
        {
            _isLeft = isLeft;

            if (_isLeft)
            {
                StageOffset = new DVector2(-1.75, 0);
            }
            else
            {
                StageOffset = new DVector2(1.75, 0);
            }

            Engines = new IEngine[0];
        }

        public override void RenderGdi(Graphics graphics, Camera camera)
        {
            if (_isHidden) return;

            base.RenderGdi(graphics, camera);
        }
    }
}
