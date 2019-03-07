using System.Drawing;
using SpaceSim.Particles;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class Merlin1D : EngineBase
    {
        public Merlin1D(int id, ISpaceCraft parent, DVector2 offset)
            : base(parent, offset, new EngineFlame(id, Color.FromArgb(63, 255, 255, 191), 30, 2, 0.2, 0.8, 0.1))
        {
        }

        public override double Thrust(double ispMultiplier)
        {
            return (845000 + 69000 * ispMultiplier) * Throttle * 0.01;
        }

        // Based off of Isp = F/ṁ*g0 => ṁ = F/Isp*g0
        public override double MassFlowRate(double ispMultiplier)
        {
            return (305.76 - 5.88 * ispMultiplier) * Throttle * 0.01;
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
