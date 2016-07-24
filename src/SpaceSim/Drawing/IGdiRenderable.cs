using System.Drawing;
using VectorMath;

namespace SpaceSim.Drawing
{
    interface IGdiRenderable
    {
        void RenderGdi(Graphics graphics, RectangleD cameraBounds);
    }
}
