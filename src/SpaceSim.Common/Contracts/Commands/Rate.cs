using System;

namespace SpaceSim.Common.Contracts.Commands
{
    [Serializable]
    public class Rate : Command
    {
        public int TargetIndex { get; set; }
    }
}
