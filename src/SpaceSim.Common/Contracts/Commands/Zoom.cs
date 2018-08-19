using System;

namespace SpaceSim.Common.Contracts.Commands
{
    [Serializable]
    public class Zoom : Command
    {
        public float TargetScale { get; set; }
    }
}
