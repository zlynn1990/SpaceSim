using System;
using System.Drawing;
using SpaceSim.Common;
using SpaceSim.Drawing;
using SpaceSim.SolarSystem;
using VectorMath;

namespace SpaceSim.Structures
{
    abstract class StructureBase : IGdiRenderable
    {
        public DVector2 Position { get; protected set; }

        public abstract double Width { get; }
        public abstract double Height { get; }

        public abstract Color IconColor { get; }

        private Bitmap _texture;

        private IMassiveBody _parent;

        private double _rotationOffset;
        private double _initialRotation;
        private double _initialDistance;

        protected StructureBase(double surfaceAngle, double height, string texturePath, IMassiveBody parent)
        {
            _parent = parent;

            _initialDistance = parent.SurfaceRadius - height;

            _rotationOffset = surfaceAngle;
            _initialRotation = parent.Pitch;

            _texture = new Bitmap(texturePath);
        }

        public double Visibility(RectangleD cameraBounds)
        {
            double distanceRatio = Height / cameraBounds.Width;

            if (distanceRatio > 0.0025)
            {
                return 1;
            }

            if (distanceRatio > 0.002)
            {
                return (distanceRatio - 0.002) * 2000;
            }

            return 0;
        }

        public RectangleD ComputeBoundingBox()
        {
            throw new NotImplementedException();
        }

        public void RenderGdi(Graphics graphics, Camera camera)
        {
            // Update position and rotation given the parent's motion
            double currentRotation = (_parent.Pitch - _initialRotation) + _rotationOffset;

            DVector2 rotationNormal = DVector2.FromAngle(currentRotation);

            Position = _parent.Position + rotationNormal * _initialDistance;

            var bounds = new RectangleD(Position.X - Width * 0.5, Position.Y - Height * 0.5, Width, Height);

            // Not in range easy return
            if (!camera.Intersects(bounds))
            {
                return;
            }

            RectangleF screenBounds = RenderUtils.ComputeBoundingBox(Position, camera.Bounds, Width, Height);

            // Saftey
            if (screenBounds.Width > RenderUtils.ScreenWidth * 500) return;

            double drawingRotation = currentRotation + Constants.PiOverTwo;

            var offset = new PointF(screenBounds.X + screenBounds.Width * 0.5f,
                                    screenBounds.Y + screenBounds.Height * 0.5f);

            camera.ApplyScreenRotation(graphics);
            camera.ApplyRotationMatrix(graphics, offset, drawingRotation);

            graphics.DrawImage(_texture, screenBounds.X, screenBounds.Y, screenBounds.Width, screenBounds.Height);

            graphics.ResetTransform();

            double visibility = Visibility(camera.Bounds);

            if (visibility < 1)
            {
                PointF iconPoint = RenderUtils.WorldToScreen(Position, camera.Bounds);

                var iconBounds = new RectangleF(iconPoint.X - 5, iconPoint.Y - 5, 10, 10);

                var iconColor = Color.FromArgb((int)((1 - visibility) * 255), IconColor.R, IconColor.G, IconColor.B);

                camera.ApplyScreenRotation(graphics);

                graphics.FillEllipse(new SolidBrush(iconColor), iconBounds);

                graphics.ResetTransform();
            }
        }
    }
}