using System;

namespace SpaceSim.Contracts.Commands
{
    [Serializable]
    public class Retrograde : Command
    {
        public double InitialAdjustmentTime { get; set; }
    }
}
