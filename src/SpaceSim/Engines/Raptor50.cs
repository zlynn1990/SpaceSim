﻿using System.Drawing;
using SpaceSim.Particles;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class Raptor50 : EngineBase
    {
        public Raptor50(int id, ISpaceCraft parent, DVector2 offset)
            : base(parent, offset, new EngineFlame(id, Color.FromArgb(63, 159, 227, 255), 50, 2, 0.2, 0.6, 0.1))
        {
        }

        public override double Thrust(double ispMultiplier)
        {
            return (3094000.0 + 241000.0 * ispMultiplier) * Throttle * 0.01;
        }

        public override double MassFlowRate(double ispMultiplier)
        {
            return 930.85 * Throttle * 0.01;
        }

        public override IEngine Clone()
        {
            return new Raptor50(0, Parent, Offset);
        }

        public override string ToString()
        {
            return "Raptor50";
        }
    }
}
