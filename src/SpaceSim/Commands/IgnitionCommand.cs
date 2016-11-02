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

        public override void Initialize(SpaceCraftBase spaceCraft)
        {
            // If no engines are requested start them all
            if (_engineIds == null)
            {
                _engineIds = new int[spaceCraft.Engines.Length];

                for (int i = 0; i < _engineIds.Length; i++)
                {
                    _engineIds[i] = i;
                }
            }

            if (_engineIds.Length > 0 && spaceCraft.Engines.Length > 0)
            {
                IEngine engine = spaceCraft.Engines[_engineIds[0]];

                if (_engineIds.Length == 1)
                {
                    if (spaceCraft.Engines.Length > 1)
                    {
                        EventManager.AddMessage(string.Format("Igniting {0} {1}", _engineIds.Length, engine), spaceCraft);
                    }
                    else
                    {
                        EventManager.AddMessage(string.Format("Igniting {0}", engine), spaceCraft);
                    }
                }
                else
                {
                    EventManager.AddMessage(string.Format("Igniting {0} {1}s", _engineIds.Length, engine), spaceCraft);
                }
            }

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

        public override void Finalize(SpaceCraftBase spaceCraft)
        {
            spaceCraft.SetThrottle(_throttle);
        }

        public override void Update(double elapsedTime, SpaceCraftBase spaceCraft)
        {
            double timeRatio = (elapsedTime - StartTime) / Duration;

            spaceCraft.SetThrottle(_throttle * timeRatio);
        }
    }
}
