using System;
using System.Windows.Input;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Controllers
{
    class SimpleFlightController : IController
    {
        public bool IsPrograde { get; protected set; }

        public bool IsRetrograde { get; protected set; }

        public double ElapsedTime { get; private set; }

        protected SpaceCraftBase SpaceCraft;

        public SimpleFlightController(SpaceCraftBase spaceCraft)
        {
            SpaceCraft = spaceCraft;
        }

        public void KeyUp(Key key)
        {
            if (key == Key.Space)
            {
                SpaceCraft.SetThrottle(0);

                SpaceCraft.Stage();
            }

            if (key == Key.X)
            {
                SpaceCraft.SetThrottle(0);
            }

            if (key == Key.Z)
            {
                SpaceCraft.SetThrottle(100);
            }

            if (key == Key.Q && !IsRetrograde)
            {
                IsPrograde = !IsPrograde;
            }

            if (key == Key.W && !IsPrograde)
            {
                IsRetrograde = !IsRetrograde;
            }
        }

        public void KeyDown(Key key)
        {
            if (key == Key.LeftCtrl)
            {
                SpaceCraft.SetThrottle(Math.Max(0, SpaceCraft.Throttle - 5));
            }

            if (key == Key.LeftShift)
            {
                SpaceCraft.SetThrottle(Math.Min(SpaceCraft.Throttle + 5, 100));
            }

            if (key == Key.A)
            {
                SpaceCraft.OffsetRotation(-0.05);
            }

            if (key == Key.D)
            {
                SpaceCraft.OffsetRotation(0.05);
            }
        }

        public virtual void Update(double dt)
        {
            ElapsedTime += dt;

            if (IsPrograde)
            {
                DVector2 prograde = SpaceCraft.GetRelativeVelocity();
                prograde.Normalize();

                SpaceCraft.SetRotation(prograde.Angle());
            }

            if (IsRetrograde)
            {
                DVector2 retrograde = SpaceCraft.GetRelativeVelocity();
                retrograde.Negate();
                retrograde.Normalize();

                SpaceCraft.SetRotation(retrograde.Angle());
            }
        }
    }
}
