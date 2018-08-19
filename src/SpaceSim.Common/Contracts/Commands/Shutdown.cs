using System;

namespace SpaceSim.Common.Contracts.Commands
{
    [Serializable]
    public class Shutdown : Command
    {
        public int[] EngineIds { get; set; }
    }
}
