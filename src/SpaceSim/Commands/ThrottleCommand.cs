using System.Collections.Generic;
using System.Linq;
using SpaceSim.Common.Contracts.Commands;
using SpaceSim.Spacecrafts;

namespace SpaceSim.Commands
{
    class ThrottleCommand : CommandBase
    {
        private int[] _engineIds;
        private readonly double _targetThrottle;
        private readonly Dictionary<int, double> _initialThrottles;

        public ThrottleCommand(Throttle throttle)
            : base(throttle.StartTime, throttle.Duration)
        {
            _targetThrottle = throttle.TargetThrottle;
            _engineIds = throttle.EngineIds;

            _initialThrottles = new Dictionary<int, double>();
        }

        public override void Initialize(SpaceCraftBase spaceCraft)
        {
            if (_engineIds == null)
            {
                EventManager.AddMessage($"Throttling to {_targetThrottle:0.0}%", spaceCraft);

                _engineIds = Enumerable.Range(0, spaceCraft.Engines.Length).ToArray();
            }
            else
            {
                EventManager.AddMessage($"Throttling engines [{string.Join(",", _engineIds)}] to {_targetThrottle:0.0}%", spaceCraft);
            }

            foreach (int engineId in _engineIds)
            {
                _initialThrottles.Add(engineId, spaceCraft.Engines[engineId].Throttle);
            }
        }

        public override void Finalize(SpaceCraftBase spaceCraft)
        {
            spaceCraft.SetThrottle(_targetThrottle, _engineIds);
        }

        // Interpolate between current and target throttle over the duration
        public override void Update(double elapsedTime, SpaceCraftBase spaceCraft)
        {
            double ratio = (elapsedTime - StartTime) / Duration;

            foreach (int engineId in _engineIds)
            {
                double targetThrottle = _initialThrottles[engineId] * (1 - ratio) + _targetThrottle * ratio;

                spaceCraft.SetThrottle(targetThrottle, new []{ engineId });
            }
        }
    }
}
