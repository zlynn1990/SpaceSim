using System.Drawing;
using VectorMath;

namespace SpaceSim.Gauges
{
    interface IGauge
    {
        RectangleF Bounds { get; }

        void Update(double thrustAngle, double thrustMagnitude);

        void Render(Graphics graphics, RectangleD cameraBounds);
    }
}
