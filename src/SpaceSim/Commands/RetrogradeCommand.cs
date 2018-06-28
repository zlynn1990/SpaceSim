using SpaceSim.Common.Contracts.Commands;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Commands
{
    class RetrogradeCommand : CommandBase
    {
        private double _adjustmentTime;
        private double _curentOrientation;

        public RetrogradeCommand(Retrograde retrograde)
            : base(retrograde.StartTime, retrograde.Duration)
        {
            _adjustmentTime = retrograde.InitialAdjustmentTime;
        }

        public override void Initialize(SpaceCraftBase spaceCraft)
        {
            // EventManager.AddMessage("Maneuvering to retrograde", spaceCraft);

            _curentOrientation = spaceCraft.Pitch;
        }

        // Nothing to finalize
        public override void Finalize(SpaceCraftBase spaceCraft) { }

        // Interpolate between current and target orientation over the duration
        public override void Update(double elapsedTime, SpaceCraftBase spaceCraft)
        {
            DVector2 retrograde = spaceCraft.GetRelativeVelocity();
            retrograde.Negate();
            retrograde.Normalize();

            double retrogradeAngle = retrograde.Angle();

            double adjustRatio = (elapsedTime - StartTime) / _adjustmentTime;

            if (adjustRatio > 1)
            {
                spaceCraft.SetPitch(retrogradeAngle);
            }
            else
            {
                double interpolatedAdjust = MathHelper.LerpAngle(_curentOrientation, retrogradeAngle, adjustRatio);

                spaceCraft.SetPitch(interpolatedAdjust);
            }
        }
    }
}
