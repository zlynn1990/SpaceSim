using SpaceSim.Contracts.Commands;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Commands
{
    class TargetCommand : CommandBase
    {
        private bool _targetNext;

        public TargetCommand(Target target)
            : base(target.StartTime, target.Duration)
        {
            _targetNext = target.TargetNext;
        }

        public override void Initialize(SpaceCraftBase spaceCraft)
        {
            EventManager.AddTarget(_targetNext, null);
        }

        public override void Finalize(SpaceCraftBase spaceCraft) { }

        public override void Update(double elapsedTime, SpaceCraftBase spaceCraft) { }
    }
}