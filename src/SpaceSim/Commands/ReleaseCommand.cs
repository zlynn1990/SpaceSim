using SpaceSim.Contracts.Commands;
using SpaceSim.Spacecrafts;

namespace SpaceSim.Commands
{
    class ReleaseCommand : CommandBase
    {
        public ReleaseCommand(Release release)
            : base(release.StartTime, 0)
        {
        }

        public override void Initialize(SpaceCraftBase spaceCraft)
        {
            spaceCraft.Release();
        }

        public override void Finalize(SpaceCraftBase spaceCraft)
        {
        }

        public override void Update(double elapsedTime, SpaceCraftBase spaceCraft)
        {
        }
    }
}
