using System;

namespace SpaceSim.Common.Contracts.Commands
{
    [Serializable]
    public class Yaw : Command
    {
        public double TargetOrientation { get; set; }
    }
}
