using SpaceSim.Drawing;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class Merlin1D : EngineBase
    {
        public Merlin1D(int id, ISpaceCraft parent, DVector2 offset)
            : base( parent, offset, new EngineFlame(id, 100, 2, 0.2, (id >0) ? 0.6 : 0.2, 0.1))
        {
        }

        public override double Thrust(double ispMultiplier)
        {
            return (756222.222 + 68888.889 * ispMultiplier) * Throttle * 0.01;
        }

        public override double MassFlowRate()
        {
            return 273.8 * Throttle * 0.01;
        }

        public override string ToString()
        {
            return "Merlin1D";
        }
    }
}
