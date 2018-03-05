using System;

namespace SpaceSim.Contracts.Commands
{
    [Serializable]
    public class Shutdown : Command
    {
        public int[] EngineIds { get; set; }
    }
}
