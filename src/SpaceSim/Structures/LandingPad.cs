using System.Drawing;
using SpaceSim.SolarSystem;

namespace SpaceSim.Structures
{
    class LandingPad : StructureBase
    {
        public override double Width { get { return 60; } }
        public override double Height { get { return 10; } }

        public override Color IconColor { get { return Color.White; } }

        public LandingPad(double surfaceAngle, double height, IMassiveBody parent)
            : base(surfaceAngle, height, "Textures/Structures/landingPad.png", parent)
        {
        }
    }
}
