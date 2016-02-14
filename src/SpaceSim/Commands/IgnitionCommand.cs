using System;
using SpaceSim.Contracts.Commands;
using SpaceSim.Engines;
using SpaceSim.Spacecrafts;

namespace SpaceSim.Commands
{
    class IgnitionCommand : CommandBase
    {
        private double _throttle;
        private int[] _engineIds;

        public IgnitionCommand(Ignition ignition)
            : base(ignition.StartTime , ignition.Duration)
        {
            _throttle = ignition.Throttle;
            _engineIds = ignition.EngineIds;
        }

        public override void Initialize(ISpaceCraft spaceCraft)
        {
            foreach (int engineId in _engineIds)
            {
                if (engineId >= spaceCraft.Engines.Length)
                {
                    throw new Exception("The spacecraft does not contain engine id " + engineId + "!");
                }

                IEngine engine = spaceCraft.Engines[engineId];

                engine.Startup();
            }
        }

        public override void Finalize(ISpaceCraft spaceCraft)
        {
            spaceCraft.SetThrottle(_throttle);
        }

        public override void Update(double elapsedTime, ISpaceCraft spaceCraft)
        {
            double timeRatio = elapsedTime - StartTime;

            spaceCraft.SetThrottle(_throttle * timeRatio);
        }
    }
}
