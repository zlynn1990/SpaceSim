using System.Drawing;
using SpaceSim.Particles;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class Merlin1DVac : EngineBase
    {
        public Merlin1DVac(ISpaceCraft parent, DVector2 offset)
            : base(parent, offset, new EngineFlame(0, Color.FromArgb(63, 255, 255, 255), 300, 3, 1.0, 1.3, 0.2))
        {
        }

        public override double Thrust(double ispMultiplier)
        {
            return (845000 + 89000 * ispMultiplier) * Throttle * 0.01;
        }

        public override double MassFlowRate(double ispMultiplier)
        {
            return 273.86 * Throttle * 0.01;
        }

        public override IEngine Clone()
        {
            return new Merlin1DVac(Parent, Offset);
        }

        public override string ToString()
        {
            return "Merlin1D Vac";
        }
    }
}
