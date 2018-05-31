using System.Drawing;
using SpaceSim.Particles;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class RaptorSL300 : EngineBase
    {
        public RaptorSL300(int id, ISpaceCraft parent, DVector2 offset)
            : base(parent, offset, new EngineFlame(id, Color.FromArgb(15, 209, 173, 199), 50, 2, 0.2, 0.6, 0.1))
        {
        }

        public override double Thrust(double ispMultiplier)
        {
            return (2040000.0 + 220000.0 * ispMultiplier) * Throttle * 0.01;
        }

        // Based off of ISP = F/m*g0
        public override double MassFlowRate(double ispMultiplier)
        {
            return 631 * Throttle * 0.01;
        }

        public override IEngine Clone()
        {
            return new RaptorSL(0, Parent, Offset);
        }

        public override string ToString()
        {
            return "RaptorSL";
        }
    }
}

