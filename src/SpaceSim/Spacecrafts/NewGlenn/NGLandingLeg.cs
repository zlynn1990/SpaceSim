using System;
using System.Drawing;
using SpaceSim.Drawing;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Spacecrafts.NewGlenn
{
    class NGLandingLeg : SpaceCraftPart
    {
        public override double Width { get { return 1.0; } protected set { value = 1.0; } }
        public override double Height { get { return 5.0; } protected set { value = 5.0; } }
        protected override double DrawingOffset { get { return 0.6; } }

        private double _offsetLength;
        private double _offsetRotation;

        private bool _isLeft;
        private bool _isDeploying;
        private double _deployTimer;

        public NGLandingLeg(ISpaceCraft parent, DVector2 offset, bool isLeft)
            : base(parent, GenerateTexturePath(isLeft))
        {
            _parent = parent;
            _isLeft = isLeft;

            _offsetLength = -offset.Length();
            _offsetRotation = offset.Angle() + Math.PI / 2.0;
        }

        private static string GenerateTexturePath(bool isLeft)
        {
            return isLeft ? "Textures/Spacecrafts/NewGlenn/landingLegLeft.png"
                : "Textures/Spacecrafts/NewGlenn/landingLegRight.png";
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
                    Pitch += 0.15 * dt;
                }
                else
                {
                    Pitch -= 0.15 * dt;
                }

                if (_deployTimer > 3)
                {
                    _isDeploying = false;
                }
            }
        }
    }
}
