using System;

namespace SpaceSim.Contracts.Commands
{
    [Serializable]
    public class Command
    {
        public double StartTime { get; set; }

        public double Duration { get; set; }
    }
}
