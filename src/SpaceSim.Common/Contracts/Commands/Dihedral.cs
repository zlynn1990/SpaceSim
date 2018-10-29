using System;

namespace SpaceSim.Common.Contracts.Commands
{
    [Serializable]
    public class Dihedral : Command
    {
        public int[] FinIds { get; set; }
        public double TargetOrientation { get; set; }
    }
}
