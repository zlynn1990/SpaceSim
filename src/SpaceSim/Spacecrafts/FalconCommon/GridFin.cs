using System;
using System.Drawing;
using VectorMath;

namespace SpaceSim.Spacecrafts.FalconCommon
{
    class GridFin : SpaceCraftPart
    {
        protected override double Width { get { return 1.2192; } }
        protected override double Height { get { return 1.79806518; } }

        private double _offsetLength;
        private double _offsetRotation;

        private bool _isLeft;
        private bool _isDeploying;
        private double _deployTimer;

        public GridFin(ISpaceCraft parent, DVector2 offset, bool isLeft)
            : base(parent)
        {
            _isLeft = isLeft;

            _offsetLength = offset.Length();
            _offsetRotation = _offsetRotation = offset.Angle() - Math.PI / 2.0;

            _texture = isLeft ? new Bitmap("Textures/Spacecrafts/Falcon/Common/gridFinLeft.png") :
                                new Bitmap("Textures/Spacecrafts/Falcon/Common//gridFinRight.png");

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
                    Pitch += 0.5*dt;
                }
                else
                {
                    Pitch -= 0.5*dt;
                }

                if (_deployTimer > 3)
                {
                    _isDeploying = false;
                }
            }
        }
    }
}
