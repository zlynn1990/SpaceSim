using SpaceSim.Contracts.Commands;
using SpaceSim.Spacecrafts;

namespace SpaceSim.Commands
{
    class StageCommand : CommandBase
    {
        public StageCommand(Stage stage)
            : base(stage.StartTime, stage.Duration)
        {
        }

        public override void Initialize(SpaceCraftBase spaceCraft)
        {
            spaceCraft.Stage();

            EventManager.AddMessage("Staging", spaceCraft);
        }

        public override void Finalize(SpaceCraftBase spaceCraft) { }

        public override void Update(double elapsedTime, SpaceCraftBase spaceCraft) { }
    }
}
