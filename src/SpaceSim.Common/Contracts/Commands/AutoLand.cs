using System;

namespace SpaceSim.Common.Contracts.Commands
{
    [Serializable]
    public class AutoLand : Command
    {
        public int[] EngineIds { get; set; }
    }
}
