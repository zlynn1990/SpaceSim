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

        public override void Initialize(ISpaceCraft spaceCraft)
        {
            spaceCraft.Stage();
        }

        public override void Finalize(ISpaceCraft spaceCraft) { }

        public override void Update(double elapsedTime, ISpaceCraft spaceCraft) { }
    }
}
