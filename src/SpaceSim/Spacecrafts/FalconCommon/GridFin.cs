using System;
using System.Drawing;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Spacecrafts.FalconCommon
{
    class GridFin : SpaceCraftPart
    {
        protected override double Width { get; }
        protected override double Height { get; }
        protected override double DrawingOffset { get; }

        private double _offsetLength;
        private double _offsetRotation;

        private bool _isLeft;
        private bool _isDeploying;
        private double _deployTimer;

        public GridFin(ISpaceCraft parent, DVector2 offset, int block, bool isLeft)
            : base(parent, GenerateTexturePath(block, isLeft))
        {
            if (block == 5)
            {
                Width = 1.3;
                Height = 2.0;
            }
            else
            {
                Width = 1.2192;
                Height = 1.79806518;
            }

            DrawingOffset = 0.6;

            _isLeft = isLeft;

            _offsetLength = offset.Length();
            _offsetRotation = offset.Angle() - Constants.PiOverTwo;
        }

        private static string GenerateTexturePath(int block, bool isLeft)
        {
            if (block == 5)
            {
                return isLeft ? "Textures/Spacecrafts/Falcon/Common/gridFinLeftB5.png"
                              : "Textures/Spacecrafts/Falcon/Common/gridFinRightB5.png";
            }

            return isLeft ? "Textures/Spacecrafts/Falcon/Common/gridFinLeft.png"
                          : "Textures/Spacecrafts/Falcon/Common/gridFinRight.png";
        }

        public void Deploy()
        {
            // Been deployed already, can't again
            if (Pitch > 0) return;

            _isDeploying = true;
        }

        public override void Update(double dt)
        {
            double rotation = _parent.Pitch -_offsetRotation;

            DVector2 offset = new DVector2(Math.Cos(rotation), Math.Sin(rotation)) * _offsetLength;

            Position = _parent.Position - offset;

            if (_isDeploying)
            {
                _deployTimer += dt;

                if (_isLeft)
                {
                    Pitch += 0.5 * dt;
                }
                else
                {
                    Pitch -= 0.5 * dt;
                }

                if (_deployTimer > 3)
                {
                    _isDeploying = false;
                }
            }
        }
    }
}