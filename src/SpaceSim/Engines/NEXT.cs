using System.Drawing;
using SpaceSim.Particles;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class NEXT : EngineBase
    {
        public NEXT(int id, ISpaceCraft parent, DVector2 offset)
            : base(parent, offset, new EngineFlame(id, Color.FromArgb(63, 121, 222, 230), 100, 2, 0.2, 0.2, 0.05))
        {
        }

        public override double Thrust(double ispMultiplier)
        {
            return 0.236 * Throttle * 0.01;
        }

        // Based off of Isp = F/ṁ*g0 where Isp = 4190s
        public override double MassFlowRate(double ispMultiplier)
        {
            return 0.00000575 * Throttle * 0.01;
        }

        public override IEngine Clone()
        {
            return new NEXT(0, Parent, Offset);
        }

        public override string ToString()
        {
            return "NEXT";
        }
    }
}
