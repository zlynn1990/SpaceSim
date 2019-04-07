using System;
using System.Drawing;
using SpaceSim.Drawing;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Spacecrafts.FalconCommon
{
    class DrogueChute : IPhysicsBody, IGdiRenderable
    {
        public DVector2 Position { get; private set; }
        public DVector2 Velocity { get; private set; }
        public double Mass { get; private set; }
        public double Pitch { get; private set; }

        private double Width = 0.0;
        private double Height = 0.0;

        private Bitmap _texture;
        private ISpaceCraft _parent;

        private double _offsetLength;
        private double _offsetRotation;

        private bool _isDeploying = false;
        private bool _isDeployed = false;
        private double _deployTimer;

        public DrogueChute(ISpaceCraft parent, DVector2 offset)
        {
            _parent = parent;

            _offsetLength = offset.Length();
            _offsetRotation = offset.Angle() - Math.PI / 2.0;

            _texture = new Bitmap("Textures/Spacecrafts/Falcon/Common/drogueChutes.png");
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

                if (Pitch < 0.25)
                {
                    Pitch += 0.025 * dt;
                }

                if (Width < 6.0)
                {
                    Width += 1 * dt;
                }
                else
                {
                    _isDeploying = false;
                    _isDeployed = true;
                }

                if (Height < 13.0)
                {
                    Height += 10 * dt;
                }

            }
        }

        public void RenderGdi(Graphics graphics, Camera camera)
        {
            double drawingRotation = _parent.Pitch + Pitch;

            DVector2 drawingOffset = new DVector2(Math.Cos(drawingRotation), Math.Sin(drawingRotation)) * 6.5;

            RectangleF screenBounds = RenderUtils.ComputeBoundingBox(Position - drawingOffset, camera.Bounds, Width, Height);

            var offset = new PointF(screenBounds.X + screenBounds.Width * 0.5f, screenBounds.Y + screenBounds.Height * 0.5f);

            graphics.TranslateTransform(offset.X, offset.Y);

            //graphics.RotateTransform((float)((drawingRotation + Math.PI * 0.5) * 180 / Math.PI));
            graphics.RotateTransform((float)((drawingRotation - Math.PI) * 180 / Math.PI));

            graphics.TranslateTransform(-offset.X, -offset.Y);

            graphics.DrawImage(_texture, screenBounds.X, screenBounds.Y, screenBounds.Width, screenBounds.Height);

            graphics.ResetTransform();
        }
    }
}