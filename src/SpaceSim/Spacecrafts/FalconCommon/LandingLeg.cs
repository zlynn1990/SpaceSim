using System;
using System.Drawing;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Spacecrafts.FalconCommon
{
    class LandingLeg : SpaceCraftPart
    {
        protected override double Width { get { return 1.8; } }
        protected override double Height { get { return 10.052819015; } }
        protected override double DrawingOffset { get { return -4.2; } }

        private double _offsetLength;
        private double _offsetRotation;

        private bool _isLeft;
        private bool _isDeploying;
        private double _deployTimer;

        public LandingLeg(ISpaceCraft parent, DVector2 offset, bool isLeft)
            : base(parent)
        {
            _isLeft = isLeft;

            _offsetLength = offset.Length();
            _offsetRotation = offset.Angle() - Constants.PiOverTwo;

            _texture = isLeft
                ? new Bitmap("Textures/Spacecrafts/Falcon/Common//landingLegLeftB5.png")
                : new Bitmap("Textures/Spacecrafts/Falcon/Common//landingLegRightB5.png");
        }

        public void Deploy()
        {
            // Been deployed already, can't again
            if (Pitch > 0) return;

            _isDeploying = true;
        }

        public override void Update(double dt)
        {
            double rotation = _parent.Pitch - _offsetRotation;

            DVector2 offset = new DVector2(Math.Cos(rotation), Math.Sin(rotation)) * _offsetLength;

            Position = _parent.Position - offset;

            if (_isDeploying)
            {
                _deployTimer += dt;

                if (_isLeft)
                {
                    Pitch -= 0.5 * dt;
                }
                else
                {
                    Pitch += 0.5 * dt;
                }

                if (_deployTimer > 4)
                {
                    _isDeploying = false;
                }
            }
        }
    }
}