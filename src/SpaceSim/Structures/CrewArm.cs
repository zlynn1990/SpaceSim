using System.Drawing;
using SpaceSim.SolarSystem;

namespace SpaceSim.Structures
{
    class CrewArm : StructureBase
    {
        public override double Width { get { return 46; } }
        public override double Height { get { return 15.3; } }

        public override Color IconColor { get { return Color.White; } }

        public CrewArm(double surfaceAngle, double height, IMassiveBody parent)
            : base(surfaceAngle, height, "Textures/Structures/crewArm.png", parent)
        {
        }
    }
}
