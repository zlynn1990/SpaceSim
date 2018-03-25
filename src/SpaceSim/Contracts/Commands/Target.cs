using System;

namespace SpaceSim.Contracts.Commands
{
    [Serializable]
    public class Target : Command
    {
        public bool Next { get; set; }
    }
}
