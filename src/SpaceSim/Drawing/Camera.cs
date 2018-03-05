using System;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Drawing
{
    class Camera
    {
        public double Zoom { get; private set; }

        private IPhysicsBody _target;
        private IPhysicsBody _lastTarget;
        private DVector2 _position;

        private bool _isInterpolating;
        private double _interpolationTime;

        public Camera(IPhysicsBody target, double zoom = 1)
        {
            _target = target;
            _lastTarget = target;

            _position = _target.Position.Clone();

            Zoom = zoom;
        }

        public void ChangeZoom(double amount)
        {
            Zoom = MathHelper.Clamp(Zoom + amount, 0.05, 1000000000000);
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
                _position = _lastTarget.Position * (1 - sigmoidY) + _target.Position * sigmoidY;

                _interpolationTime += dt;
            }
            else
            {
                _position = _target.Position;
            }
        }

        public RectangleD GetBounds()
        {
            double width = RenderUtils.ScreenWidth * Zoom;
            double height = RenderUtils.ScreenHeight * Zoom;

            return new RectangleD(_position.X - width * 0.5, _position.Y - height * 0.5, width, height);
        }
    }
}
