using System.Drawing;
using SpaceSim.SolarSystem;

namespace SpaceSim.Structures
{
    class TowersLeft : StructureBase
    {
        public override double Width { get { return 20; } }
        public override double Height { get { return 100; } }

        public override Color IconColor { get { return Color.White; } }

        public TowersLeft(double surfaceAngle, double height, IMassiveBody parent)
            : base(surfaceAngle, height, "Textures/Structures/towersLeft.png", parent)
        {
        }
    }
}
