using System;

namespace SpaceSim.Common.Contracts.Commands
{
    [Serializable]
    public class Command
    {
        public double StartTime { get; set; }

        public double Duration { get; set; }

        public override string ToString()
        {
            var start = TimeSpan.FromSeconds(StartTime);

            return $"{GetType().Name}  {start:mm\\:ss}";
        }
    }
}
