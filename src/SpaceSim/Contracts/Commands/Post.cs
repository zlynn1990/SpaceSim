using System;

namespace SpaceSim.Contracts.Commands
{
    [Serializable]
    public class Post : Command
    {
        public string Message { get; set; }
    }
}
