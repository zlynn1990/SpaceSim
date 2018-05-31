using SpaceSim.Contracts.Commands;
using SpaceSim.Spacecrafts;

namespace SpaceSim.Commands
{
    class TerminateCommand : CommandBase
    {
        public TerminateCommand(Terminate terminate)
            : base(terminate.StartTime, 0)
        {
        }

        public override void Initialize(SpaceCraftBase spaceCraft)
        {
            spaceCraft.Terminate();
        }

        public override void Finalize(SpaceCraftBase spaceCraft) { }

        public override void Update(double elapsedTime, SpaceCraftBase spaceCraft) { }
    }
}
