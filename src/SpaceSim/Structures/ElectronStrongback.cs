using System.Drawing;
using SpaceSim.SolarSystem;

namespace SpaceSim.Structures
{
    class ElectronStrongback : StructureBase
    {
        public override double Width { get { return 5; } }
        public override double Height { get { return 17; } }

        public override Color IconColor { get { return Color.White; } }

        public ElectronStrongback(double surfaceAngle, double height, IMassiveBody parent)
            : base(surfaceAngle, height, "Textures/Structures/electronStrongback.png", parent)
        {
        }
    }
}
