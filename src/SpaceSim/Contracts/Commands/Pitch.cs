using System;

namespace SpaceSim.Contracts.Commands
{
    [Serializable]
    public class Pitch : Command
    {
        public double TargetOrientation { get; set; }
    }
}
