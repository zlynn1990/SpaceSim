using SpaceSim.Properties;
using System.Drawing;
using VectorMath;

namespace SpaceSim.Gauges
{
    class ProgradeButton : IGauge
    {
        public bool IsActive { get; private set; }

        public RectangleF Bounds { get; private set; }

        private Point _center;
        private Font _font;
        private float _size;

        public ProgradeButton(Point center)
        {
            _center = center;
            _font = Settings.Default.Font;
            _size = _font.Size;
            Bounds = new RectangleF(_center.X - _size, _center.Y - _size, _size * 2, _size * 2);
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

            graphics.DrawString("P", _font, new SolidBrush(Color.White), _center.X - _size * 0.6f, _center.Y - _size * 0.8f);
            graphics.DrawEllipse(new Pen(Color.White, 2), Bounds);
        }
    }
}
