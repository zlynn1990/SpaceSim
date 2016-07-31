using SpaceSim.Contracts.Commands;
using SpaceSim.Engines;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Commands
{
    class CantCommand : CommandBase
    {
        private double _targetOrientation;
        private double _currentOrientation;

        public CantCommand(Cant cant)
            : base(cant.StartTime, cant.Duration)
        {
            _targetOrientation = cant.TargetOrientation * MathHelper.DegreesToRadians;
        }

        public override void Initialize(SpaceCraftBase spaceCraft)
        {
            foreach (IEngine engine in spaceCraft.Engines)
            {
                _currentOrientation = engine.Cant;
            }
        }

        public override void Finalize(SpaceCraftBase spaceCraft)
        {
            foreach (IEngine engine in spaceCraft.Engines)
            {
                engine.AdjustCant(_targetOrientation);
            }
        }

        // Interpolate between current and target orientation over the duration
        public override void Update(double elapsedTime, SpaceCraftBase spaceCraft)
        {
            double ratio = (elapsedTime - StartTime) / Duration;
            foreach (IEngine engine in spaceCraft.Engines)
            {
                engine.AdjustCant(_currentOrientation * (1 - ratio) + _targetOrientation * ratio);
            }
        }
    }
}
