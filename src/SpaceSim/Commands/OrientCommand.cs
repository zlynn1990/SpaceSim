using SpaceSim.Contracts.Commands;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Commands
{
    class OrientCommand : CommandBase
    {
        private double _targetOrientation;
        private double _curentOrientation;

        public OrientCommand(Orient orient)
            : base(orient.StartTime, orient.Duration)
        {
            _targetOrientation = orient.TargetOrientation * MathHelper.DegreesToRadians;
        }

        public override void Initialize(ISpaceCraft spaceCraft)
        {
            _curentOrientation = spaceCraft.Rotation;
        }

        public override void Finalize(ISpaceCraft spaceCraft)
        {
            spaceCraft.SetRotation(_targetOrientation);
        }

        // Interpolate between current and target orientation over the duration
        public override void Update(double elapsedTime, ISpaceCraft spaceCraft)
        {
            double ratio = (elapsedTime - StartTime) / Duration;

            spaceCraft.SetRotation(_curentOrientation * (1 - ratio) + _targetOrientation * ratio);
        }
    }
}
