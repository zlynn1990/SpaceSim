using System;

namespace SpaceSim.Common.Contracts.Commands
{
    [Serializable]
    public class Retrograde : Command
    {
        public double InitialAdjustmentTime { get; set; }
    }
}
