using System.Drawing;
using SpaceSim.Particles;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class MiniRaptor50 : EngineBase
    {
        public MiniRaptor50(int id, ISpaceCraft parent, DVector2 offset)
            : base(parent, offset, new EngineFlame(id, Color.FromArgb(63, 207, 239, 255), 75, 2, 0.2, 0.4, 0.025))
        {
        }

        public override double Thrust(double ispMultiplier)
        {
            return (1031000.0 + 80000.0 * ispMultiplier) * Throttle * 0.01;
        }

        public override double MassFlowRate(double ispMultiplier)
        {
            return 310 * Throttle * 0.01;
        }

        public override IEngine Clone()
        {
            return new MiniRaptor50(0, Parent, Offset);
        }

        public override string ToString()
        {
            return "MiniRaptor50";
        }
    }
}
