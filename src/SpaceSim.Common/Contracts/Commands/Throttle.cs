using System;

namespace SpaceSim.Common.Contracts.Commands
{
    [Serializable]
    public class Throttle : Command 
    {
        public int[] EngineIds { get; set; }

        public double TargetThrottle { get; set; }
    }
}
