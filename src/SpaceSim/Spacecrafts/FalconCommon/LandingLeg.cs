using System;
using SpaceSim.Common;
using VectorMath;

namespace SpaceSim.Spacecrafts.FalconCommon
{
    class LandingLeg : SpaceCraftPart
    {
        public override double Width { get { return 1.8; } protected set { value = 1.8; } }
        public override double Height { get { return 10.052819015; } protected set { value = 10.052819015; } }
        protected override double DrawingOffset { get { return -4.2; } }

        private double _offsetLength;
        private double _offsetRotation;

        private bool _isLeft;
        private bool _isDeploying;
        private double _deployTimer;

        public LandingLeg(ISpaceCraft parent, DVector2 offset, int block, bool isLeft)
            : base(parent, GenerateTexturePath(block, isLeft))
        {
            _isLeft = isLeft;

            _offsetLength = offset.Length();
            _offsetRotation = offset.Angle() - Constants.PiOverTwo;
        }

        private static string GenerateTexturePath(int block, bool isLeft)
        {
            if (block == 5)
            {
                return isLeft ? "Textures/Spacecrafts/Falcon/Common//landingLegLeftB5.png"
                              : "Textures/Spacecrafts/Falcon/Common//landingLegRightB5.png";
            }

            return isLeft ? "Textures/Spacecrafts/Falcon/Common//landingLegLeft.png"
                          : "Textures/Spacecrafts/Falcon/Common//landingLegRight.png";
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