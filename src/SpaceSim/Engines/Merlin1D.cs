using System.Drawing;
using SpaceSim.Particles;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class Merlin1D : EngineBase
    {
        public Merlin1D(int id, ISpaceCraft parent, DVector2 offset)
            : base(parent, offset, new EngineFlame(id, Color.FromArgb(200, 255, 255, 0), 100, 2, 0.2, 0.6, 0.1))
        {
        }

        public override double Thrust(double ispMultiplier)
        {
            return (756222.222 + 68888.889 * ispMultiplier) * Throttle * 0.01;
        }

        public override double MassFlowRate()
        {
            return 273.8 * Throttle * 0.01;
        }

        public override IEngine Clone()
        {
            return new Merlin1D(0, Parent, Offset);
        }

        public override string ToString()
        {
            return "Merlin1D";
        }
    }
}
