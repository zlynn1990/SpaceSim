using System;
using System.Drawing;
using VectorMath;

namespace SpaceSim.Gauges
{
    public class NavBall : IGauge
    {
        public RectangleF Bounds { get; private set; }

        private Point _center;
        private double _thrustAngle;
        private double _flightPathAngle;

        public NavBall(Point center)
        {
            _center = center;

            Bounds = new RectangleF(_center.X - 55, _center.Y - 55, 110, 110);
        }

        public void Update(double thrustAngle, double thrustMagnitude, double flightPathAngle)
        {
            _thrustAngle = thrustAngle;
            _flightPathAngle = flightPathAngle;
        }

        public void Render(Graphics graphics, RectangleD cameraBounds)
        {
            var end = new PointF(_center.X + (float)Math.Cos(_thrustAngle) * 50, _center.Y + (float)Math.Sin(_thrustAngle) * 50);
            graphics.DrawLine(new Pen(Color.Red, 2), _center, end);

            end = new PointF(_center.X + (float)Math.Cos(_flightPathAngle) * 50, _center.Y + (float)Math.Sin(_flightPathAngle) * 50);
            graphics.DrawLine(new Pen(Color.Yellow, 2), _center, end);

            graphics.DrawEllipse(new Pen(Color.White, 2), Bounds);
        }
    }
}
