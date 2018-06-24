using System.Drawing;
using SpaceSim.Particles;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class RS25 : EngineBase
    {
        public RS25(int id, ISpaceCraft parent, DVector2 offset)
            : base(parent, offset, new EngineFlame(id, Color.FromArgb(63, 207, 239, 255), 100, 2, 0.2, 0.6, 0.1))
        {
        }

        public override double Thrust(double ispMultiplier)
        {
            return (1860000.0 + 419000.0 * ispMultiplier) * Throttle * 0.01;
        }

        // Based off of ISP = F/m*g0
        public override double MassFlowRate(double ispMultiplier)
        {
            return 514.00 * Throttle * 0.01;
        }

        public override IEngine Clone()
        {
            return new RS25(0, Parent, Offset);
        }

        public override string ToString()
        {
            return "RS-25";
        }
    }
}
