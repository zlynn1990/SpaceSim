using SpaceSim.Drawing;
using SpaceSim.Spacecrafts;

namespace SpaceSim.Commands
{
    abstract class CommandBase
    {
        public double StartTime { get; private set; }
        public double Duration { get; private set; }

        protected EventManager EventManager;

        protected CommandBase(double startTime, double duration)
        {
            StartTime = startTime;
            Duration = duration;
        }

        public void LoadEventManager(EventManager eventManager)
        {
            EventManager = eventManager;
        }

        public abstract void Initialize(SpaceCraftBase spaceCraft);
        public abstract void Finalize(SpaceCraftBase spaceCraft);

        public abstract void Update(double elapsedTime, SpaceCraftBase spaceCraft);
    }
}
