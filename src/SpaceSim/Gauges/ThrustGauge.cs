using System.Drawing;
using VectorMath;

namespace SpaceSim.Gauges
{
    class ThrustGauge : IGauge
    {
        public RectangleF Bounds { get; private set; }

        private Point _center;
        private double _thrustMagnitude;

        public ThrustGauge(Point center)
        {
            _center = center;

            Bounds = new RectangleF(_center.X - 5, _center.Y - 52, 10, 104);
        }

        public void Update(double thrustAngle, double thrustMagnitude)
        {
            _thrustMagnitude = thrustMagnitude;
        }

        public void Render(Graphics graphics, RectangleD cameraBounds)
        {
            graphics.FillRectangle(new SolidBrush(Color.White), _center.X - 1, _center.Y - 52, 2, 104);

            graphics.FillRectangle(new SolidBrush(Color.White), _center.X - 5, _center.Y - 52, 10, 3);
            graphics.FillRectangle(new SolidBrush(Color.White), _center.X - 5, _center.Y + 52, 10, 3);

            int thrustX = (int)(_thrustMagnitude * 104) - 52;

            graphics.FillRectangle(new SolidBrush(Color.Red), _center.X -5, _center.Y - thrustX, 10, 4);
        }
    }
}
