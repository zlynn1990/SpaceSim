using System;

namespace SpaceSim.Contracts.Commands
{
    [Serializable]
    public class Rate : Command
    {
        public int TargetDelta { get; set; }
    }
}
