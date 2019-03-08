using System.Drawing;
using SpaceSim.SolarSystem;

namespace SpaceSim.Structures
{
    class TowersRight : StructureBase
    {
        public override double Width { get { return 20; } }
        public override double Height { get { return 100; } }

        public override Color IconColor { get { return Color.White; } }

        public TowersRight(double surfaceAngle, double height, IMassiveBody parent)
            : base(surfaceAngle, height, "Textures/Structures/towersRight.png", parent)
        {
        }
    }
}
