using System.Drawing;
using SpaceSim.SolarSystem;

namespace SpaceSim.Structures
{
    class Ocean : StructureBase
    {
        public override double Width { get { return 500; } }
        public override double Height { get { return 100; } }

        public override Color IconColor { get { return Color.DeepSkyBlue; } }

        public Ocean(double surfaceAngle, double height, IMassiveBody parent)
            : base(surfaceAngle, height, "Textures/Structures/ocean.png", parent)
        {
        }
    }
}
