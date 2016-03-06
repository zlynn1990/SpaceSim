using SpaceSim.Drawing;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class Merlin1D : EngineBase
    {
        public Merlin1D(int id, ISpaceCraft parent, DVector2 offset)
            : base( parent, offset, new EngineFlame(id, 300, 3, 0.2,0.6))
        {
        }

        public override double Thrust(double ispMultiplier)
        {
            return (756222.222 + 68888.889 * ispMultiplier) * Throttle * 0.01;
        }

        public override double MassFlowRate()
        {
            return 274 * Throttle * 0.01;
        }
    }
}
