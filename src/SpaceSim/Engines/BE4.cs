using System.Drawing;
using SpaceSim.Particles;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class BE4 : EngineBase
    {
        public BE4(int id, ISpaceCraft parent, DVector2 offset)
            : base(parent, offset, new EngineFlame(id, Color.FromArgb(63, 207, 239, 255), 100, 2, 0.2, 0.6, 0.1))
        {
        }

        public override double Thrust(double ispMultiplier)
        {
            return (2450000.0 + 200000.0 * ispMultiplier) * Throttle * 0.01;
        }

        // Isp = 290
        public override double MassFlowRate(double ispMultiplier)
        {
            return 860.00 * Throttle * 0.01;
        }

        public override IEngine Clone()
        {
            return new BE4(0, Parent, Offset);
        }

        public override string ToString()
        {
            return "BE-4";
        }
    }
}
