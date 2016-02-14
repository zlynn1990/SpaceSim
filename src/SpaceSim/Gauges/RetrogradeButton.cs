using System.Drawing;
using VectorMath;

namespace SpaceSim.Gauges
{
    class RetrogradeButton : IGauge
    {
        public bool IsActive { get; private set; }

        public RectangleF Bounds { get; private set; }

        private Point _center;
        private Font _font;

        public RetrogradeButton(Point center)
        {
            _center = center;
            Bounds = new RectangleF(_center.X - 15, _center.Y - 15, 30, 30);

            _font = new Font("Verdana Bold", 12);
        }

        public void Enable()
        {
            IsActive = true;
        }

        public void Disable()
        {
            IsActive = false;
        }

        public void Update(double thrustAngle, double thrustMagnitude) { }

        public void Render(Graphics graphics, RectangleD cameraBounds)
        {
            if (IsActive)
            {
                graphics.FillEllipse(new SolidBrush(Color.Red), Bounds);
            }

            graphics.DrawString("R", _font, new SolidBrush(Color.White), _center.X - 8, _center.Y - 8);

            graphics.DrawEllipse(new Pen(Color.White, 2), Bounds);
        }
    }
}
