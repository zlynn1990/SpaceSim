using SpaceSim.Common.Contracts.Commands;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Commands
{
    class RelativePitchCommand : CommandBase
    {
        private double _targetRelativePitch;
        private double _currentRelativePitch;

        public RelativePitchCommand(RelativePitch orient)
              : base(orient.StartTime, orient.Duration)
        {
            _targetRelativePitch = orient.TargetOrientation * MathHelper.DegreesToRadians;
        }

        public override void Initialize(SpaceCraftBase spaceCraft)
        {
            _currentRelativePitch = spaceCraft.GetRelativePitch();
        }

        public override void Finalize(SpaceCraftBase spaceCraft)
        {
            spaceCraft.SetRelativePitch(_targetRelativePitch);
        }

        // Interpolate between current and target orientation over the duration
        public override void Update(double elapsedTime, SpaceCraftBase spaceCraft)
        {
            double ratio = (elapsedTime - StartTime) / Duration;

            spaceCraft.SetRelativePitch(MathHelper.LerpAngle(_currentRelativePitch, _targetRelativePitch, ratio));
        }
    }
}
