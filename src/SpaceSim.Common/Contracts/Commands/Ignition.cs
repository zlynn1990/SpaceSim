using System;

namespace SpaceSim.Common.Contracts.Commands
{
    [Serializable]
    public class Ignition : Command
    {
        public double Throttle { get; set; }

        public int[] EngineIds { get; set; }
    }
}
