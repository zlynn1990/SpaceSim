using System;

namespace SpaceSim.Contracts.Commands
{
    [Serializable]
    public class Target : Command
    {
        public bool TargetNext { get; set; }
    }
}
