using SpaceSim.Contracts.Commands;
using SpaceSim.Spacecrafts;

namespace SpaceSim.Commands
{
    class FairingCommand : CommandBase
    {
        public FairingCommand(Fairing fairing)
            : base(fairing.StartTime, fairing.Duration)
        {
        }

        public override void Initialize(SpaceCraftBase spaceCraft)
        {
            EventManager.AddMessage("Fairing Seperation", spaceCraft);

            spaceCraft.DeployFairing();
        }

        public override void Finalize(SpaceCraftBase spaceCraft) { }

        public override void Update(double elapsedTime, SpaceCraftBase spaceCraft) { }
    }
}
