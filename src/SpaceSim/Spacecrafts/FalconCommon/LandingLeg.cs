using System;
using System.Drawing;
using SpaceSim.Drawing;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Spacecrafts.FalconCommon
{
    class LandingLeg : IPhysicsBody, IGdiRenderable
    {
        public DVector2 Position { get; private set; }
        public DVector2 Velocity { get; private set; }
        public double Mass { get; private set; }
        public double Pitch { get; private set; }

        private const double Width = 1.8;
        private const double Height = 10.052819015;

        private Bitmap _texture;
        private ISpaceCraft _parent;

        private double _offsetLength;
        private double _offsetRotation;

        private bool _isLeft;
        private bool _isDeploying;
        private double _deployTimer;

        public LandingLeg(ISpaceCraft parent, DVector2 offset, bool isLeft)
        {
            _parent = parent;
            _isLeft = isLeft;

            _offsetLength = offset.Length();
            _offsetRotation = _offsetRotation = offset.Angle() - Math.PI/2.0;

            _texture = isLeft ? new Bitmap("Textures/landingLegLeft.png") : new Bitmap("Textures/landingLegRight.png");
        }

        public void Deploy()
        {
            // Been deployed already, can't again
            if (Pitch > 0) return;

            _isDeploying = true;
        }

        public void Update(double dt)
        {
            double rotation = _parent.Pitch - _offsetRotation;

            DVector2 offset = new DVector2(Math.Cos(rotation), Math.Sin(rotation))*_offsetLength;

            Position = _parent.Position - offset;

            if (_isDeploying)
            {
                _deployTimer += dt;

                if (_isLeft)
                {
                    Pitch -= 0.5*dt;
                }
                else
                {
                    Pitch += 0.5*dt;
                }

                if (_deployTimer > 4)
                {
                    _isDeploying = false;
                }
            }
        }

        public void RenderGdi(Graphics graphics, RectangleD cameraBounds)
        {
            double drawingRotation = _parent.Pitch + Pitch;

            DVector2 drawingOffset = new DVector2(Math.Cos(drawingRotation), Math.Sin(drawingRotation)) * -4.2;

            RectangleF screenBounds = RenderUtils.ComputeBoundingBox(Position - drawingOffset, cameraBounds, Width,Height);

            var offset = new PointF(screenBounds.X + screenBounds.Width*0.5f, screenBounds.Y + screenBounds.Height*0.5f);

            graphics.TranslateTransform(offset.X, offset.Y);

            graphics.RotateTransform((float) ((drawingRotation + Math.PI*0.5)*180/Math.PI));

            graphics.TranslateTransform(-offset.X, -offset.Y);

            graphics.DrawImage(_texture, screenBounds.X, screenBounds.Y, screenBounds.Width, screenBounds.Height);

            graphics.ResetTransform();
        }
    }
}