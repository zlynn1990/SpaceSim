using System.Drawing;

namespace SpaceSim.Drawing
{
    interface IGdiRenderable
    {
        void RenderGdi(Graphics graphics, Camera camera);
    }
}
