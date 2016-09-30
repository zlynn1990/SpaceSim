using SpaceSim.Contracts.Commands;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Commands
{
    class PitchCommand : CommandBase
    {
        private double _targetOrientation;
        private double _currentOrientation;

        private double _displayOrientation;

        public PitchCommand(Pitch orient)
            : base(orient.StartTime, orient.Duration)
        {
            _targetOrientation = orient.TargetOrientation * MathHelper.DegreesToRadians;
            _displayOrientation = orient.TargetOrientation + 90;

        }

        public override void Initialize(SpaceCraftBase spaceCraft)
        {
            _currentOrientation = spaceCraft.Pitch;

            EventManager.AddMessage(string.Format("Pitching to {0} degrees", _displayOrientation.ToString("0.0")), spaceCraft);
        }

        public override void Finalize(SpaceCraftBase spaceCraft)
        {
            spaceCraft.SetPitch(_targetOrientation);
        }

        // Interpolate between current and target orientation over the duration
        public override void Update(double elapsedTime, SpaceCraftBase spaceCraft)
        {
            double ratio = (elapsedTime - StartTime) / Duration;

            spaceCraft.SetPitch(_currentOrientation * (1 - ratio) + _targetOrientation * ratio);
        }
    }
}
