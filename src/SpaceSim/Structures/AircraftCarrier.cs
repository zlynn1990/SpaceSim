using System.Drawing;
using SpaceSim.SolarSystem;

namespace SpaceSim.Structures
{
    class AircraftCarrier : StructureBase
    {
        public override double Width { get { return 400; } }
        public override double Height { get { return 60; } }

        public override Color IconColor { get { return Color.White; } }

        public AircraftCarrier(double surfaceAngle, double height, IMassiveBody parent)
            : base(surfaceAngle, height, "Textures/Structures/aircraftCarrier.png", parent)
        {
        }
    }
}
