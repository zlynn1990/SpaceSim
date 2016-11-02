using System.Drawing;
using SpaceSim.Particles;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class SuperDraco : EngineBase
    {
        private double _angle;

        public SuperDraco(int id, ISpaceCraft parent, DVector2 offset, double angle)
            : base(parent, offset, new EngineFlame(id, Color.Yellow, 200, 4, 0.1, 0.15, 0.02, angle))
        {
            _angle = angle;
        }

        public override double Thrust(double ispMultiplier)
        {
            return 66723.25 * Throttle * 0.01;
        }

        public override double MassFlowRate()
        {
            return 31.25 * Throttle * 0.01;
        }

        public override IEngine Clone()
        {
            return new SuperDraco(0, Parent, Offset, _angle);
        }

        public override string ToString()
        {
            return "Super Draco";
        }
    }
}
