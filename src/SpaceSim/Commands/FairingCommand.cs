using SpaceSim.Contracts.Commands;
using SpaceSim.Spacecrafts;

namespace SpaceSim.Commands
{
    class FairingCommand : CommandBase
    {
        public FairingCommand(Fairing stage)
            : base(stage.StartTime, stage.Duration)
        {
        }

        public override void Initialize(ISpaceCraft spaceCraft)
        {
            EventManager.AddMessage("Fairing Seperation", spaceCraft);

            spaceCraft.DeployFairing();
        }

        public override void Finalize(ISpaceCraft spaceCraft) { }

        public override void Update(double elapsedTime, ISpaceCraft spaceCraft) { }
    }
}
