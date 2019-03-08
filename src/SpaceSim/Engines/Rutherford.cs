using System.Drawing;
using SpaceSim.Particles;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class Rutherford : EngineBase
    {
        public Rutherford(int id, ISpaceCraft parent, DVector2 offset)
            : base(parent, offset, new EngineFlame(id, Color.FromArgb(63, 255, 255, 191), 30, 2, 0.2, 0.6, 0.01))
        {
        }

        public override double Thrust(double ispMultiplier)
        {
            return (17000 + 3500 * ispMultiplier) * Throttle * 0.01;
        }

        // Based off of ISP = F/ṁ*g0 = 248-300
        public override double MassFlowRate(double ispMultiplier)
        {
            return 6.8 * Throttle * 0.01;
        }

        public override IEngine Clone()
        {
            return new Rutherford(0, Parent, Offset);
        }

        public override string ToString()
        {
            return "Rutherford";
        }
    }
}
