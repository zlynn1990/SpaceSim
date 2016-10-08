using System.Drawing;
using SpaceSim.SolarSystem;

namespace SpaceSim.Structures
{
    class Strongback : StructureBase
    {
        public override double Width { get { return 22; } }
        public override double Height { get { return 70; } }

        public override Color IconColor { get { return Color.White; } }

        public Strongback(double surfaceAngle, double height, IMassiveBody parent)
            : base(surfaceAngle, height, "Textures/strongback.png", parent)
        {
        }
    }
}
