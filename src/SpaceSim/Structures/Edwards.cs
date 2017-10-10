using System.Drawing;
using SpaceSim.SolarSystem;

namespace SpaceSim.Structures
{
    class Edwards : StructureBase
    {
        public override double Width { get { return 2000; } }
        public override double Height { get { return 200; } }

        public override Color IconColor { get { return Color.White; } }

        public Edwards(double surfaceAngle, double height, IMassiveBody parent)
            : base(surfaceAngle, height, "Textures/Structures/edwards.png", parent)
        {
        }
    }
}
