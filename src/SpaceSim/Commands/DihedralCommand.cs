using SpaceSim.Common.Contracts.Commands;
using SpaceSim.Engines;
using SpaceSim.Spacecrafts;
using System.Collections.Generic;
using System.Linq;
using VectorMath;

namespace SpaceSim.Commands
{
    class DihedralCommand : CommandBase
    {
        private int[] _finIds;
        private double _targetOrientation;
        private readonly Dictionary<int, double> _initialDihedrals;

        public DihedralCommand(Dihedral dihedral)
            : base(dihedral.StartTime, dihedral.Duration)
        {
            _targetOrientation = dihedral.TargetOrientation * MathHelper.DegreesToRadians;
            _finIds = dihedral.FinIds;
            _initialDihedrals = new Dictionary<int, double>();
        }

        public override void Initialize(SpaceCraftBase spaceCraft)
        {
            if (_finIds == null)
            {
                EventManager.AddMessage($"Adjusting dihedral to {MathHelper.RadiansToDegrees * _targetOrientation:0.0}°", spaceCraft);

                _finIds = Enumerable.Range(0, spaceCraft.Fins.Length).ToArray();
            }
            else
            {
                EventManager.AddMessage($"Adjusting dihedral [{string.Join(",", _finIds)}] to {MathHelper.RadiansToDegrees * _targetOrientation:0.0}°", spaceCraft);
            }

            foreach (int finId in _finIds)
            {
                _initialDihedrals.Add(finId, spaceCraft.Fins[finId].Dihedral);
            }
        }

        public override void Finalize(SpaceCraftBase spaceCraft)
        {
            spaceCraft.SetDihedral(_targetOrientation, _finIds);
        }

        // Interpolate between current and target orientation over the duration
        public override void Update(double elapsedTime, SpaceCraftBase spaceCraft)
        {
            double ratio = (elapsedTime - StartTime) / Duration;
            foreach (int finId in _finIds)
            {
                double targetDihedral = _initialDihedrals[finId] * (1 - ratio) + _targetOrientation * ratio;
                spaceCraft.SetDihedral(targetDihedral, new[] { finId });
            }
        }
    }
}
