using System.Drawing;
using SpaceSim.Particles;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class Curie : EngineBase
    {
        public Curie(ISpaceCraft parent, DVector2 offset)
            : base(parent, offset, new EngineFlame(0, Color.FromArgb(200, 255, 255, 0), 100, 2, 0.2, 0.6, 0.02))
        {
        }

        public override double Thrust(double ispMultiplier)
        {
            return 120 * Throttle * 0.01;
        }

        // Based off of ISP = F/ṁ*g0
        public override double MassFlowRate(double ispMultiplier)
        {
            return 0.0437 * Throttle * 0.01;
        }

        public override IEngine Clone()
        {
            return new Curie(Parent, Offset);
        }

        public override string ToString()
        {
            return "Curie";
        }
    }
}
