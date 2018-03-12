using SpaceSim.Contracts.Commands;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Commands
{
    class RateCommand : CommandBase
    {
        private int _targetDelta;

        public RateCommand(Rate rate)
            : base(rate.StartTime, rate.Duration)
        {
            _targetDelta = rate.TargetDelta;
        }

        public override void Initialize(SpaceCraftBase spaceCraft)
        {
            EventManager.AddRate(_targetDelta, null);
        }

        public override void Finalize(SpaceCraftBase spaceCraft) { }

        public override void Update(double elapsedTime, SpaceCraftBase spaceCraft) { }
    }
}
