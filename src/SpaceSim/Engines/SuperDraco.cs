using SpaceSim.Drawing;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class SuperDraco : EngineBase
    {
        public SuperDraco(int id, ISpaceCraft parent, DVector2 offset)
            : base(parent, offset, new EngineFlame(id, 50, 1, 0.1, 0.15))
        {
        }

        public override double Thrust(double ispMultiplier)
        {
            return 66723.25 * Throttle * 0.01;
        }

        public override double MassFlowRate()
        {
            return 31.25 * Throttle * 0.01;
        }
    }
}
