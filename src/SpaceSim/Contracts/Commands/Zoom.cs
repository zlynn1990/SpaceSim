using System;

namespace SpaceSim.Contracts.Commands
{
    [Serializable]
    public class Zoom : Command
    {
        public float TargetScale { get; set; }
    }
}
