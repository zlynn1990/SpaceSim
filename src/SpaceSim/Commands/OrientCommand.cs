using SpaceSim.Contracts.Commands;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Commands
{
    class OrientCommand : CommandBase
    {
        private double _targetOrientation;
        private double _curentOrientation;

        private double _displayOrientation;

        public OrientCommand(Orient orient)
            : base(orient.StartTime, orient.Duration)
        {
            _targetOrientation = orient.TargetOrientation * MathHelper.DegreesToRadians;
            _displayOrientation = orient.TargetOrientation + 90;
        }

        public override void Initialize(SpaceCraftBase spaceCraft)
        {
            EventManager.AddMessage(string.Format("Pitching to {0} degrees", _displayOrientation.ToString("0.0")), spaceCraft);

            _curentOrientation = spaceCraft.Rotation;
        }

        public override void Finalize(SpaceCraftBase spaceCraft)
        {
            spaceCraft.SetRotation(_targetOrientation);
        }

        // Interpolate between current and target orientation over the duration
        public override void Update(double elapsedTime, SpaceCraftBase spaceCraft)
        {
            double ratio = (elapsedTime - StartTime) / Duration;

            spaceCraft.SetRotation(_curentOrientation * (1 - ratio) + _targetOrientation * ratio);
        }
    }
}
