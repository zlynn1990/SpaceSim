using System;

namespace SpaceSim.Contracts.Commands
{
    [Serializable]
    public class Prograde : Command
    {
        public double InitialAdjustmentTime { get; set; }
    }
}
