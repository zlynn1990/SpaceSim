using System;

namespace SpaceSim.Common.Contracts.Commands
{
    [Serializable]
    public class Prograde : Command
    {
        public double InitialAdjustmentTime { get; set; }
    }
}
