using System;

namespace SpaceSim.Contracts.Commands
{
    [Serializable]
    public class Orient : Command
    {
        public double TargetOrientation { get; set; }
    }
}
