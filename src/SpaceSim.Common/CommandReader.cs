using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using SpaceSim.Common.Contracts.Commands;

namespace SpaceSim.Common
{
    public static class CommandReader
    {
        private static readonly XmlSerializer Serializer;

        static CommandReader()
        {
            Serializer = new XmlSerializer(typeof(List<Command>), new[]
            {
                typeof(Ignition), typeof(Shutdown), typeof(Stage),
                typeof(Throttle), typeof(Deploy), typeof(Release), typeof(Terminate),
                typeof(Retrograde), typeof(Prograde), typeof(AutoLand),
                typeof(Cant), typeof(Pitch), typeof(RelativePitch),
                typeof(Roll), typeof(Yaw), typeof(Post), typeof(Rate), typeof(Target), typeof(Zoom),
            });
        }

        public static List<Command> Read(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                return (List<Command>) Serializer.Deserialize(fileStream);
            }
        }
    }
}
