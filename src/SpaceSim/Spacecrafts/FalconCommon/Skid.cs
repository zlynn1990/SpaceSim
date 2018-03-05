using System;
using System.Drawing;
using SpaceSim.Drawing;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Spacecrafts.FalconCommon
{
    class Skid : IPhysicsBody, IGdiRenderable
    {
        public DVector2 Position { get; private set; }
        public DVector2 Velocity { get; private set; }
        public double Mass { get; private set; }
        public double Pitch { get; private set; }

        private const double Width = 0.5;
        private const double Height = 2.5;

        private Bitmap _texture;
        private ISpaceCraft _parent;

        private double _offsetLength;
        private double _offsetRotation;

        private bool _isLeft;
        private bool _isDeploying;
        private bool _isDeployed;
        private double _deployTimer;

        public Skid(ISpaceCraft parent, DVector2 offset, bool isLeft)
        {
            _parent = parent;
            _isLeft = isLeft;

            _offsetLength = -offset.Length();
            _offsetRotation = offset.Angle() + Math.PI / 2.0;

            _texture = isLeft ? new Bitmap("Textures/Spacecrafts/NewGlenn/landingLegLeft.png") :
                                new Bitmap("Textures/Spacecrafts/NewGlenn/landingLegRight.png");
        }

        public void Deploy()
        {
            // Been deployed already, can't again
            if (Pitch > 0) return;

            _isDeploying = true;
        }

        public bool IsDeploying()
        {
            return _isDeploying;
        }

        public bool IsDeployed()
        {
            return _isDeployed;
        }

        public void Update(double dt)
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
                    _isDeployed = true;
                }
            }
        }

        public void RenderGdi(Graphics graphics, Camera camera)
        {
            double drawingRotation = _parent.Pitch + Pitch;

            DVector2 drawingOffset = new DVector2(Math.Cos(drawingRotation), Math.Sin(drawingRotation)) * -1;

            RectangleF screenBounds = RenderUtils.ComputeBoundingBox(Position + drawingOffset, camera.Bounds, Width, Height);

            var offset = new PointF(screenBounds.X + screenBounds.Width * 0.5f, screenBounds.Y + screenBounds.Height * 0.5f);

            graphics.TranslateTransform(offset.X, offset.Y);

            graphics.RotateTransform((float)((drawingRotation + Math.PI * 0.5) * 180 / Math.PI));

            graphics.TranslateTransform(-offset.X, -offset.Y);

            graphics.DrawImage(_texture, screenBounds.X, screenBounds.Y, screenBounds.Width, screenBounds.Height);

            graphics.ResetTransform();
        }
    }
}

