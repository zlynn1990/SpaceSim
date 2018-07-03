using SpaceSim.Common.Contracts.Commands;
using SpaceSim.Spacecrafts;

namespace SpaceSim.Commands
{
    class ThrottleCommand : CommandBase
    {
        private double _targetThrottle;
        private double _currentThrottle;

        public ThrottleCommand(Throttle throttle)
            : base(throttle.StartTime, throttle.Duration)
        {
            _targetThrottle = throttle.TargetThrottle;
        }

        public override void Initialize(SpaceCraftBase spaceCraft)
        {
            EventManager.AddMessage($"Throttling to {_targetThrottle:0.0}%", spaceCraft);

            _currentThrottle = spaceCraft.Throttle;
        }

        public override void Finalize(SpaceCraftBase spaceCraft)
        {
            spaceCraft.SetThrottle(_targetThrottle);
        }

        // Interpolate between current and target throttle over the duration
        public override void Update(double elapsedTime, SpaceCraftBase spaceCraft)
        {
            double ratio = (elapsedTime - StartTime) / Duration;

            spaceCraft.SetThrottle(_currentThrottle * (1 - ratio) + _targetThrottle * ratio);
        }
    }
}
