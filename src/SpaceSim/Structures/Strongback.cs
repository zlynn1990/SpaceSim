using System.Drawing;
using SpaceSim.SolarSystem;
using VectorMath;

namespace SpaceSim.Structures
{
    class Strongback : StructureBase
    {
        public override double Width { get { return 17.139; } }
        public override double Height { get { return 70; } }

        public override Color IconColor { get { return Color.White; } }

        public Strongback(DVector2 relativePosition, IMassiveBody parent)
            : base(relativePosition, "Textures/strongback.png", parent)
        {
        }
    }
}
