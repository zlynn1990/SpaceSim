using System;
using System.Drawing;
using SpaceSim.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Spacecrafts.FalconCommon
{
    class Fairing : SpaceCraftBase
    {
        public override string CraftName { get { return _isLeft ? "Fairing Left" : "Fairing Right"; } }
        public override string CommandFileName { get { return _isLeft ? "fairingLeft.xml" : "fairingRight.xml"; } }

        public override double Width { get { return 2.59; } }
        public override double Height { get { return 13.0; } }

        public override double DryMass { get { return 875; } }

        DrogueChute _drogueChute;
        Parachute _parachute;

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
                if (_drogueChute.IsDeployed())
                    baseCd = GetBaseCd(0.5);
                if (_parachute.IsDeployed())
                    baseCd = GetBaseCd(1.0);

                double alpha = GetAlpha();

                return baseCd * Math.Cos(alpha);
            }
        }

        public override double LiftCoefficient
        {
            get
            {
                double baseCd = GetBaseCd(0.6);
                if (_parachute.IsDeployed())
                    baseCd = GetBaseCd(3.0);

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

        public override void DeployDrogues()
        {
            _drogueChute.Deploy();
        }

        public override void DeployParachutes()
        {
            _parachute.Deploy();
        }

        public void Hide()
        {
            _isHidden = true;
        }

        public Fairing(string craftDirectory, DVector2 position, DVector2 velocity, bool isLeft)
            : base(craftDirectory, position, velocity, 0, 0, isLeft ? "Falcon/Common/fairingLeft.png" : "Falcon/Common/fairingRight.png", null)
        {
            _isLeft = isLeft;

            if (_isLeft)
            {
                StageOffset = new DVector2(-1.26, -2.2);
                _drogueChute = new DrogueChute(this, new DVector2(-1.26, 6.5));
                _parachute = new Parachute(this, new DVector2(-1.26, 0.0), _isLeft);
            }
            else
            {
                StageOffset = new DVector2(1.26, -2.2);
                _drogueChute = new DrogueChute(this, new DVector2(1.26, 6.5));
                _parachute = new Parachute(this, new DVector2(1.26, 0.0), _isLeft);
            }

            Engines = new IEngine[0];
        }

        public override void Update(double dt)
        {
            base.Update(dt);

            _drogueChute.Update(dt);
            _parachute.Update(dt);
        }

        public override void RenderGdi(Graphics graphics, Camera camera)
        {
            if (_isHidden) return;

            base.RenderGdi(graphics, camera);
        }

        protected override void RenderShip(Graphics graphics, Camera camera, RectangleF screenBounds)
        {
            base.RenderShip(graphics, camera, screenBounds);

            if(_drogueChute.IsDeploying() || _drogueChute.IsDeployed())
                _drogueChute.RenderGdi(graphics, camera);

            if (_parachute.IsDeploying() || _parachute.IsDeployed())
                _parachute.RenderGdi(graphics, camera);
        }
    }
}
