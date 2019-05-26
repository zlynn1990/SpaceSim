using System.Drawing;
using SpaceSim.Particles;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class AJ10 : EngineBase
    {
        private double _angle;

        public AJ10(int id, ISpaceCraft parent, DVector2 offset, double angle)
            : base(parent, offset, new EngineFlame(id, Color.FromArgb(63, 255, 255, 159), 50, 2, 1.0, 1.2, 0.1, angle))
        {
            _angle = angle;
        }

        public override double Thrust(double ispMultiplier)
        {
            return 26600 * Throttle * 0.01;
        }

        // Based off of Isp = F/ṁ*g0 => ṁ = F/Isp*g0 Isp = 319
        public override double MassFlowRate(double ispMultiplier)
        {
            return 8.5 * Throttle * 0.01;
        }

        public override IEngine Clone()
        {
            return new AJ10(0, Parent, Offset, _angle);
        }

        public override string ToString()
        {
            return "AJ10";
        }
    }
}
