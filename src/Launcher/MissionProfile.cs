using System.Collections.Generic;
using SpaceSim.Common.Contracts;
using SpaceSim.Common.Contracts.Commands;

namespace Launcher
{
    class MissionProfile
    {
        public MissionConfig Config { get; set; }
        
        public Dictionary<string, List<Command>> Commands { get; set; }
    }
}
