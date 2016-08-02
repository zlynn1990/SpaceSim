using System;

namespace SpaceSim.Contracts.Commands
{
    [Serializable]
    public class Yaw : Command
    {
        public double TargetOrientation { get; set; }
    }
}
