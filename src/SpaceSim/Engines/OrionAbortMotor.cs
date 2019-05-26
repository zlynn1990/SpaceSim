using System.Drawing;
using SpaceSim.Particles;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class OrionAbortMotor : EngineBase
    {
        private double _angle;

        public OrionAbortMotor(int id, ISpaceCraft parent, DVector2 offset, double angle)
            : base(parent, offset, new EngineFlame(id, Color.FromArgb(63, 255, 255, 159), 50, 2, 0.1, 0.15, 0.003, angle))
        {
            _angle = angle;
        }

        public override double Thrust(double ispMultiplier)
        {
            return 563250 * Throttle * 0.01;
        }

        // ṁ = F / Isp * g0
        // Isp = 250
        public override double MassFlowRate(double ispMultiplier)
        {
            return 230 * Throttle * 0.01;
        }

        public override IEngine Clone()
        {
            return new OrionAbortMotor(0, Parent, Offset, _angle);
        }

        public override string ToString()
        {
            return "Orion Abort Motor";
        }
    }
}
