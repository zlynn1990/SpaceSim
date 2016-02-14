using System.Drawing;
using VectorMath;

namespace SpaceSim.Drawing
{
    interface IGdiRenderable : IIconRenderable
    {
        void RenderGdi(Graphics graphics, RectangleD cameraBounds);
    }
}
