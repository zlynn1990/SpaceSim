using SpaceSim.Contracts.Commands;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Commands
{
    class ZoomCommand : CommandBase
    {
        private float _targetScale;

        public ZoomCommand(Zoom zoom)
            : base(zoom.StartTime, zoom.Duration)
        {
            _targetScale = zoom.TargetScale;
        }

        public override void Initialize(SpaceCraftBase spaceCraft)
        {
            EventManager.AddZoom(_targetScale, null);
        }

        public override void Finalize(SpaceCraftBase spaceCraft) { }

        public override void Update(double elapsedTime, SpaceCraftBase spaceCraft) { }
    }
}

