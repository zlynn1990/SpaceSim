using System.Drawing;
using SpaceSim.Particles;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class StarDrive : EngineBase
    {
        public StarDrive(int id, ISpaceCraft parent, DVector2 offset)
            : base(parent, offset, new EngineFlame(id, Color.FromArgb(63, 121, 222, 230), 50, 2, 0.2, 0.0005, 0.05))
        {
        }

        public override double Thrust(double ispMultiplier)
        {
            return 0.059 * Throttle * 0.01;
        }

        // Based off of Isp = F/ṁ*g0 where Isp = 1600s
        // ṁ = F / Isp * g0
        public override double MassFlowRate(double ispMultiplier)
        {
            return 0.00000375 * Throttle * 0.01;
        }

        public override IEngine Clone()
        {
            return new StarDrive(0, Parent, Offset);
        }

        public override string ToString()
        {
            return "StarDrive";
        }
    }
}
