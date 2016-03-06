using SpaceSim.Drawing;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class Merlin1DVac : EngineBase
    {
        public Merlin1DVac(ISpaceCraft parent, DVector2 offset)
            : base(parent, offset, new EngineFlame(0, 500, 3, 0.5, 0.5))
        {
        }

        public override double Thrust(double ispMultiplier)
        {
            return (770000 + 165000.0 * ispMultiplier) * Throttle * 0.01;
        }

        public override double MassFlowRate()
        {
            return 274 * Throttle * 0.01;
        }
    }
}
