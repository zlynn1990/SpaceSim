﻿using System.Drawing;
using SpaceSim.Particles;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    class RL10 : EngineBase
    {
        public RL10(int id, ISpaceCraft parent, DVector2 offset)
            : base(parent, offset, new EngineFlame(id, Color.FromArgb(63, 221, 192, 220), 100, 2, 1.0, 1.2, 0.1))
        {
        }

        public override double Thrust(double ispMultiplier)
        {
            return (100000.0 + 10000.0 * ispMultiplier) * Throttle * 0.01;
        }

        // Based off of Isp = F/ṁ*g0 => ṁ = F/Isp*g0 Isp = 465.5
        public override double MassFlowRate(double ispMultiplier)
        {
            return 24.14 * Throttle * 0.01;
        }

        public override IEngine Clone()
        {
            return new RL10(0, Parent, Offset);
        }

        public override string ToString()
        {
            return "RL-10";
        }
    }
}
