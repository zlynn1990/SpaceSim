using System.Drawing;
using SpaceSim.Particles;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class BE3U : EngineBase
    {
        public BE3U(int id, ISpaceCraft parent, DVector2 offset)
            : base(parent, offset, new EngineFlame(id, Color.FromArgb(63, 221, 192, 220), 100, 2, 0.2, 0.6, 0.1))
        {
        }

        public override double Thrust(double ispMultiplier)
        {
            return 530000.0 * Throttle * 0.01;
        }

        // Isp = 415
        public override double MassFlowRate(double ispMultiplier)
        {
            return 130.00 * Throttle * 0.01;
        }

        public override IEngine Clone()
        {
            return new BE3U(0, Parent, Offset);
        }

        public override string ToString()
        {
            return "BE-3U";
        }
    }
}
