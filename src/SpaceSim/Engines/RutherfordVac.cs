using System.Drawing;
using SpaceSim.Particles;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class RutherfordVac : EngineBase
    {
        public RutherfordVac(ISpaceCraft parent, DVector2 offset)
            : base(parent, offset, new EngineFlame(0, Color.FromArgb(63, 255, 255, 255), 300, 2, 1.0, 1.3, 0.02))
        {
        }

        public override double Thrust(double ispMultiplier)
        {
            return 24000 * Throttle * 0.01;
        }

        // Based off of ISP = F/m*g0
        public override double MassFlowRate(double ispMultiplier)
        {
            return 6.5 * Throttle * 0.01;
        }

        public override IEngine Clone()
        {
            return new RutherfordVac(Parent, Offset);
        }

        public override string ToString()
        {
            return "Rutherford Vacuum";
        }
    }
}
