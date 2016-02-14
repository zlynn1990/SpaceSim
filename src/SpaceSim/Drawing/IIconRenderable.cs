using System.Drawing;
using VectorMath;

namespace SpaceSim.Drawing
{
    interface IIconRenderable
    {
        Color IconColor { get; }

        double Visibility(RectangleD cameraBounds);

        RectangleD ComputeBoundingBox();
    }
}
