using System;

namespace SpaceSim.Contracts.Commands
{
    [Serializable]
    public class Roll : Command
    {
        public double TargetOrientation { get; set; }
    }
}
