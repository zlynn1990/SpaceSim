using System;

namespace SpaceSim.Common.Contracts.Commands
{
    [Serializable]
    public class Roll : Command
    {
        public double TargetOrientation { get; set; }
    }
}
