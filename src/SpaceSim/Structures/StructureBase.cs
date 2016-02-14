using System;
using System.Drawing;
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

        protected StructureBase(DVector2 relativePosition, string texturePath, IMassiveBody parent)
        {
            _parent = parent;

            _initialDistance = relativePosition.Length();

            _rotationOffset = relativePosition.Angle();
            _initialRotation = parent.Rotation;

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

        public void RenderGdi(Graphics graphics, RectangleD cameraBounds)
        {
            // Update position and rotation given the parent's motion
            double currentRotation = (_parent.Rotation - _initialRotation) + _rotationOffset;

            DVector2 rotationNormal = DVector2.FromAngle(currentRotation);

            Position = _parent.Position + rotationNormal * _initialDistance;

            var bounds = new RectangleD(Position.X - Width * 0.5, Position.Y - Height * 0.5, Width, Height);

            // Not in range easy return
            if (!cameraBounds.IntersectsWith(bounds))
            {
                return;
            }

            RectangleF screenBounds = RenderUtils.ComputeBoundingBox(Position, cameraBounds, Width, Height);

            // Saftey
            if (screenBounds.Width > RenderUtils.ScreenWidth * 500) return;

            double drawingRotation = currentRotation + Math.PI * 0.5;

            var offset = new PointF(screenBounds.X + screenBounds.Width * 0.5f,
                                    screenBounds.Y + screenBounds.Height * 0.5f);

            graphics.TranslateTransform(offset.X, offset.Y);

            graphics.RotateTransform((float)(drawingRotation * 180 / Math.PI));

            graphics.TranslateTransform(-offset.X, -offset.Y);

            graphics.DrawImage(_texture, screenBounds.X, screenBounds.Y, screenBounds.Width, screenBounds.Height);

            graphics.ResetTransform();

            double visibility = Visibility(cameraBounds);

            if (visibility < 1)
            {
                PointF iconPoint = RenderUtils.WorldToScreen(Position, cameraBounds);

                var iconBounds = new RectangleF(iconPoint.X - 5, iconPoint.Y - 5, 10, 10);

                var iconColor = Color.FromArgb((int)((1 - visibility) * 255), IconColor.R, IconColor.G, IconColor.B);

                graphics.FillEllipse(new SolidBrush(iconColor), iconBounds);
            }
        }
    }
}
