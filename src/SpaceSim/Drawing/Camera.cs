using System;
using System.Drawing;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Drawing
{
    class Camera
    {
        public double Zoom { get; private set; }
        public double Rotation { get; private set; }

        public RectangleD Bounds { get { return _cachedBounds; } }

        private IPhysicsBody _target;
        private IPhysicsBody _lastTarget;
        private DVector2 _position;

        private double _targetRotation;

        private double _minimumZoom;
        private double _maximumZoom;

        private bool _isInterpolating;
        private double _interpolationTime;

        private RectangleD _cachedBounds;
        private RectangleD _rotatedBounds;

        public Camera(IPhysicsBody target, double zoom = 1)
        {
            _target = target;
            _lastTarget = target;

            _minimumZoom = 0.05;
            _maximumZoom = 1000000000000;

            _position = _target.Position.Clone();

            Zoom = zoom;

            ComputeBounds();
        }

        public void ChangeZoom(double amount)
        {
            Zoom = MathHelper.Clamp(Zoom + amount, _minimumZoom, _maximumZoom);
        }

        public void SetRotation(double rotation)
        {
            _targetRotation = rotation;
        }

        public void UpdateTarget(IPhysicsBody target)
        {
            _lastTarget = _target;
            _target = target;

            _isInterpolating = true;
            _interpolationTime = 0;
        }

        public void Update(double dt)
        {
            if (_isInterpolating)
            {
                if (_interpolationTime > 1)
                {
                    _isInterpolating = false;
                }

                // Interpolate using a logistic function
                // https://en.wikipedia.org/wiki/Logistic_function
                double sigmoidX = (_interpolationTime - 0.5) * 6;

                double sigmoidY = (1.0 / (1 + Math.Exp(-3.0 * sigmoidX)));

                // Lerp between the two targets
                _position = DVector2.Lerp(_lastTarget.Position, _target.Position, sigmoidY);

                _interpolationTime += dt;

                Rotation = MathHelper.LerpAngle(Rotation, _targetRotation, sigmoidY);
            }
            else
            {
                _position = _target.Position;
                Rotation = _targetRotation;
            }

            ComputeBounds();
        }

        public void ApplyScreenRotation(Graphics graphics)
        {
            ApplyRotationMatrix(graphics, RenderUtils.ScreenWidth * 0.5f, RenderUtils.ScreenHeight * 0.5f, Rotation);
        }

        public void ApplyRotationMatrix(Graphics graphics, PointF offset, double angle)
        {
            ApplyRotationMatrix(graphics, offset.X, offset.Y, angle);
        }

        public void ApplyRotationMatrix(Graphics graphics, float x, float y, double angle)
        {
            graphics.TranslateTransform(x, y);
            graphics.RotateTransform((float)(angle * MathHelper.RadiansToDegrees));
            graphics.TranslateTransform(-x, -y);
        }

        public bool Contains(DVector2 point)
        {
            return _rotatedBounds.Contains(point);
        }

        public bool Intersects(RectangleD rectangle)
        {
            return _rotatedBounds.IntersectsWith(rectangle);
        }

        private void ComputeBounds()
        {
            double width = RenderUtils.ScreenWidth * Zoom;
            double height = RenderUtils.ScreenHeight * Zoom;

            _cachedBounds = new RectangleD(_position.X - width * 0.5, _position.Y - height * 0.5, width, height);
            _rotatedBounds = new RectangleD(_position.X - width, _position.Y - height, width * 2, height * 2);
        }
    }
}