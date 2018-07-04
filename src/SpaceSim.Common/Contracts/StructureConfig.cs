using System;

namespace SpaceSim.Common.Contracts
{
    [Serializable]
    public class StructureConfig
    {
        public string Type { get; set; }

        public double DownrangeDistance { get; set; }

        public double HeightOffset { get; set; }
    }
}
