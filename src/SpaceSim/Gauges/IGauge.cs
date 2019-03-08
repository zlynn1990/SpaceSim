using System.Drawing;
using VectorMath;

namespace SpaceSim.Gauges
{
    interface IGauge
    {
        RectangleF Bounds { get; }

        void Update(double thrustAngle, double thrustMagnitude, double flightPathAngle);

        void Render(Graphics graphics, RectangleD cameraBounds);
    }
}
