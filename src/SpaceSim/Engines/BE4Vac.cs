using System.Drawing;
using SpaceSim.Particles;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class BE4Vac : EngineBase
    {
        public BE4Vac(int id, ISpaceCraft parent, DVector2 offset)
            : base(parent, offset, new EngineFlame(id, Color.FromArgb(63, 207, 239, 255), 500, 3, 0.7, 0.75, 0.1))
        {
        }

        public override double Thrust(double ispMultiplier)
        {
            return (2450000.0 + 300000.0 * ispMultiplier) * Throttle * 0.01;
        }

        public override double MassFlowRate(double ispMultiplier)
        {
            return 850.00 * Throttle * 0.01;
        }

        public override IEngine Clone()
        {
            return new BE4Vac(0, Parent, Offset);
        }

        public override string ToString()
        {
            return "BE-4 Vac";
        }
    }
}
