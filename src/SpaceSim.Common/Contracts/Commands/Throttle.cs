using System;

namespace SpaceSim.Common.Contracts.Commands
{
    [Serializable]
    public class Throttle : Command 
    {
        public double TargetThrottle { get; set; }
    }
}
