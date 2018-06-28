using System;

namespace SpaceSim.Common.Contracts.Commands
{
    [Serializable]
    public class Target : Command
    {
        public bool Next { get; set; }
    }
}
