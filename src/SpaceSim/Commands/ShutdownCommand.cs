using System;
using SpaceSim.Contracts.Commands;
using SpaceSim.Engines;
using SpaceSim.Spacecrafts;

namespace SpaceSim.Commands
{
    [Serializable]
    class ShutdownCommand : CommandBase
    {
        private double _currentThrottle;

        public ShutdownCommand(Shutdown shutdown)
            : base(shutdown.StartTime, shutdown.Duration)
        {
        }

        public override void Initialize(ISpaceCraft spaceCraft)
        {
            _currentThrottle = spaceCraft.Throttle;
        }

        public override void Finalize(ISpaceCraft spaceCraft)
        {
            spaceCraft.SetThrottle(0);

            foreach (IEngine engine in spaceCraft.Engines)
            {
                engine.Shutdown();
            }
        }

        public override void Update(double elapsedTime, ISpaceCraft spaceCraft)
        {
            double shutdownRatio = (elapsedTime - StartTime) * 2;

            spaceCraft.SetThrottle(_currentThrottle * (1-shutdownRatio));
        }
    }
}
