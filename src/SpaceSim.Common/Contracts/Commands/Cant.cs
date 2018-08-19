using System;

namespace SpaceSim.Common.Contracts.Commands
{
    [Serializable]
    public class Cant : Command
    {
        public double TargetOrientation { get; set; }
    }
}
