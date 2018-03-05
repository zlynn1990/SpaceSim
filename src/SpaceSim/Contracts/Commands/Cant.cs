using System;

namespace SpaceSim.Contracts.Commands
{
    [Serializable]
    public class Cant : Command
    {
        public double TargetOrientation { get; set; }
    }
}
