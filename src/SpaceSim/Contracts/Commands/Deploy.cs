using System;

namespace SpaceSim.Contracts.Commands
{
    [Serializable]
    public class Deploy : Command
    {
        public string Part { get; set; }
    }
}
