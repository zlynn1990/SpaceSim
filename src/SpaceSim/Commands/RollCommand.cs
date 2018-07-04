using SpaceSim.Common.Contracts.Commands;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Commands
{
    class RollCommand : CommandBase
    {
        private double _targetOrientation;
        private double _currentOrientation;
        private double _displayOrientation;

        public RollCommand(Roll orient)
            : base(orient.StartTime, orient.Duration)
        {
            _targetOrientation = orient.TargetOrientation * MathHelper.DegreesToRadians;
            _displayOrientation = -orient.TargetOrientation;
        }

        public override void Initialize(SpaceCraftBase spaceCraft)
        {
            _currentOrientation = spaceCraft.Roll;

            EventManager.AddMessage(string.Format("Rolling to {0} degrees", _displayOrientation.ToString("0.0")), spaceCraft);
        }

        public override void Finalize(SpaceCraftBase spaceCraft)
        {
            spaceCraft.SetRoll(_targetOrientation);
        }

        // Interpolate between current and target orientation over the duration
        public override void Update(double elapsedTime, SpaceCraftBase spaceCraft)
        {
            double ratio = (elapsedTime - StartTime) / Duration;

            spaceCraft.SetRoll(_currentOrientation * (1 - ratio) + _targetOrientation * ratio);
        }
    }
}
