using System.Drawing;
using SpaceSim.Particles;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class Raptor : EngineBase
    {
        public Raptor(int id, ISpaceCraft parent, DVector2 offset)
            : base(parent, offset, new EngineFlame(id, Color.FromArgb(255, 159, 227, 255), 100, 2, 0.2, 0.6, 0.1))
        {
        }

        public override double Thrust(double ispMultiplier)
        {
            return (3094000 + 239000 * ispMultiplier) * Throttle * 0.01;
        }

        public override double MassFlowRate()
        {
            return 930.85 * Throttle * 0.01;
        }

        public override IEngine Clone()
        {
            return new Raptor(0, Parent, Offset);
        }

        public override string ToString()
        {
            return "Raptor";
        }
    }
}
