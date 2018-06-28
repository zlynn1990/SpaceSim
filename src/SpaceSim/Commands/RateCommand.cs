using SpaceSim.Common.Contracts.Commands;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Commands
{
    class RateCommand : CommandBase
    {
        private int _targetIndex;

        public RateCommand(Rate rate)
            : base(rate.StartTime, rate.Duration)
        {
            _targetIndex = rate.TargetIndex;
        }

        public override void Initialize(SpaceCraftBase spaceCraft)
        {
            EventManager.AddRate(_targetIndex, null);
        }

        public override void Finalize(SpaceCraftBase spaceCraft) { }

        public override void Update(double elapsedTime, SpaceCraftBase spaceCraft) { }
    }
}
