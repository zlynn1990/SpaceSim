using System.Drawing;
using VectorMath;

namespace SpaceSim.Drawing
{
    interface IMapRenderable
    {
        Color IconColor { get; }

        double Visibility(RectangleD cameraBounds);

        RectangleD ComputeBoundingBox();
    }
}
