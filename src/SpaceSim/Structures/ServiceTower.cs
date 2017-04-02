using System.Drawing;
using SpaceSim.SolarSystem;

namespace SpaceSim.Structures
{
    class ServiceTower : StructureBase
    {
        public override double Width { get { return 18.75; } }
        public override double Height { get { return 90; } }

        public override Color IconColor { get { return Color.White; } }

        public ServiceTower(double surfaceAngle, double height, IMassiveBody parent)
            : base(surfaceAngle, height, "Textures/servicetower.png", parent)
        {
        }
    }
}
