using System.Drawing;
using SpaceSim.SolarSystem;

namespace SpaceSim.Structures
{
    class ITSMount : StructureBase
    {
        public override double Width { get { return 60; } }
        public override double Height { get { return 140; } }

        public override Color IconColor { get { return Color.White; } }

        public ITSMount(double surfaceAngle, double height, IMassiveBody parent)
            : base(surfaceAngle, height, "Textures/Structures/itsMount.png", parent)
        {
        }
    }
}
