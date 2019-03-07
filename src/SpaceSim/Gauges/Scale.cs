using System.Drawing;
using SpaceSim.Common;
using SpaceSim.Drawing;
using VectorMath;
using SpaceSim.Properties;

namespace SpaceSim.Gauges
{
    class Scale : IGauge
    {
        public RectangleF Bounds { get; private set; }

        private const int ScaleWidth = 100;

        private Point _center;
        private Font _font;
        private double _widthPercentage;
        private float _size;

        public Scale(Point center)
        {
            _center = center;

            _font = Settings.Default.Font;
            _size = _font.Size;

            _widthPercentage = (double)ScaleWidth / RenderUtils.ScreenWidth;
        }

        public void Update(double thrustAngle, double thrustMagnitude, double flightPathAngle) { }

        public void Render(Graphics graphics, RectangleD cameraBounds)
        {
            double scale = cameraBounds.Width * _widthPercentage;

            graphics.FillRectangle(new SolidBrush(Color.White), _center.X - 50, _center.Y - 2, 100, 4);
            graphics.FillRectangle(new SolidBrush(Color.White), _center.X - 51, _center.Y - _size, 4, 20);
            graphics.FillRectangle(new SolidBrush(Color.White), _center.X + 51, _center.Y - _size, 4, 20);

            graphics.DrawString(UnitDisplay.Distance(scale), _font, new SolidBrush(Color.White), _center.X + 65, _center.Y - _size);
        }
    }
}
