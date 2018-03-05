using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceSim.Contracts.Commands
{
    [Serializable]
    public class AutoLand : Command
    {
        public int[] EngineIds { get; set; }
    }
}
