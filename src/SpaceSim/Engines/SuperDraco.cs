using SpaceSim.Drawing;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class SuperDraco : EngineBase
    {
        public SuperDraco(int id, ISpaceCraft parent, DVector2 offset, double angle)
            : base(parent, offset, new EngineFlame(id, 200, 4, 0.1, 0.15, 0.02, angle))
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

        public override string ToString()
        {
            return "Super Draco";
        }
    }
}
