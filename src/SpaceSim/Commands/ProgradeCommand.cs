using SpaceSim.Contracts.Commands;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Commands
{
    class ProgradeCommand : CommandBase
    {
        private double _adjustmentTime;
        private double _curentOrientation;

        public ProgradeCommand(Prograde prograde)
            : base(prograde.StartTime, prograde.Duration)
        {
            _adjustmentTime = prograde.InitialAdjustmentTime;
        }

        public override void Initialize(SpaceCraftBase spaceCraft)
        {
            EventManager.AddMessage("Maneuvering to prograde", spaceCraft);

            _curentOrientation = spaceCraft.Rotation;
        }

        // Nothing to finalize
        public override void Finalize(SpaceCraftBase spaceCraft) { }

        // Interpolate between current and target orientation over the duration
        public override void Update(double elapsedTime, SpaceCraftBase spaceCraft)
        {
            DVector2 prograde = spaceCraft.GetRelativeVelocity();
            prograde.Normalize();

            double retrogradeAngle = prograde.Angle();

            double adjustRatio = (elapsedTime - StartTime) / _adjustmentTime;

            if (adjustRatio > 1)
            {
                spaceCraft.SetRotation(retrogradeAngle);
            }
            else
            {
                double interpolatedAdjust = _curentOrientation * (1 - adjustRatio) + retrogradeAngle * adjustRatio;

                spaceCraft.SetRotation(interpolatedAdjust);
            }
        }
    }
}
