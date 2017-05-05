using System.Drawing;
using SpaceSim.SolarSystem;

namespace SpaceSim.Structures
{
    class ASDS : StructureBase
    {
        public override double Width { get { return 100; } }
        public override double Height { get { return 15; } }

        public override Color IconColor { get { return Color.White; } }

        public ASDS(double surfaceAngle, double height, IMassiveBody parent)
            : base(surfaceAngle, height, "Textures/Structures/asds.png", parent)
        {
        }
    }
}
