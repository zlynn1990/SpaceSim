using SpaceSim.Common.Contracts.Commands;
using SpaceSim.Spacecrafts;

namespace SpaceSim.Commands
{
    class PostCommand : CommandBase
    {
        private string _message;

        public PostCommand(Post post)
            : base(post.StartTime, post.Duration)
        {
            _message = post.Message;
        }

        public override void Initialize(SpaceCraftBase spaceCraft)
        {
            EventManager.AddMessage(_message, null);
        }

        public override void Finalize(SpaceCraftBase spaceCraft) { }

        public override void Update(double elapsedTime, SpaceCraftBase spaceCraft) { }
    }
}
