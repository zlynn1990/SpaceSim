using System;

namespace SpaceSim.Contracts.Commands
{
    [Serializable]
    public class Throttle : Command 
    {
        public double TargetThrottle { get; set; }
    }
}
