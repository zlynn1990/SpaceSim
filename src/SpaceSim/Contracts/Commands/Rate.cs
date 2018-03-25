using System;

namespace SpaceSim.Contracts.Commands
{
    [Serializable]
    public class Rate : Command
    {
        public int TargetIndex { get; set; }
    }
}
