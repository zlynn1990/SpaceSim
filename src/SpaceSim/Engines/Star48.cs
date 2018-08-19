using System.Drawing;
using SpaceSim.Particles;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class Star48 : EngineBase
    {
        public Star48(int id, ISpaceCraft parent, DVector2 offset)
            : base(parent, offset, new EngineFlame(id, Color.FromArgb(255, 255, 255, 255), 250, 2, 0.3, 0.8, 0.2))
        {
        }

        public override double Thrust(double ispMultiplier)
        {
            return 66000 * Throttle * 0.01;
        }

        // Based off of ISP = F/m*g0
        public override double MassFlowRate(double ispMultiplier)
        {
            return 24 * Throttle * 0.01;
        }

        public override IEngine Clone()
        {
            return new Star48(0, Parent, Offset);
        }

        public override string ToString()
        {
            return "Star-48";
        }
    }
}
