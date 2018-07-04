using System;

namespace SpaceSim.Common.Contracts.Commands
{
    [Serializable]
    public class Deploy : Command
    {
        public string Part { get; set; }
    }
}
