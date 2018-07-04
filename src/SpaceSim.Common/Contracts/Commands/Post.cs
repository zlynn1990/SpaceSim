using System;

namespace SpaceSim.Common.Contracts.Commands
{
    [Serializable]
    public class Post : Command
    {
        public string Message { get; set; }
    }
}
