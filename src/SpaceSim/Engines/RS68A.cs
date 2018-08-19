using System.Drawing;
using SpaceSim.Particles;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class RS68A : EngineBase
    {
        public RS68A(int id, ISpaceCraft parent, DVector2 offset)
            : base(parent, offset, new EngineFlame(id, Color.FromArgb(63, 221, 192, 220), 100, 2, 0.2, 1.0, 0.15))
        {
        }

        public override double Thrust(double ispMultiplier)
        {
            return (3137000.0 + 446000.0 * ispMultiplier) * Throttle * 0.01;
        }

        // Based off of ISP = F/ṁ*g0
        public override double MassFlowRate(double ispMultiplier)
        {
            return 882.00 * Throttle * 0.01;
        }

        public override IEngine Clone()
        {
            return new RS68A(0, Parent, Offset);
        }

        public override string ToString()
        {
            return "RS-68A";
        }
    }
}
