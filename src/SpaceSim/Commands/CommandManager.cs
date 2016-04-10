using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using SpaceSim.Contracts.Commands;

namespace SpaceSim.Commands
{
    static class CommandManager
    {
        private static XmlSerializer Serializer;

        static CommandManager()
        {
            Serializer = new XmlSerializer(typeof(List<Command>), new[]
            {
                typeof (Ignition), typeof (Shutdown), typeof (Stage),
                typeof (Throttle), typeof (Orient), typeof(Fairing), typeof(Retrograde)
            });
        }

        /// <summary>
        /// Loads commands from an xml format.
        /// </summary>
        public static List<CommandBase> Load(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                var contracts = (List<Command>)Serializer.Deserialize(fileStream);

                var commands = new List<CommandBase>();

                // Not a great way to do this unfortunately...
                foreach (Command contract in contracts)
                {
                    if (contract is Ignition)
                    {
                        commands.Add(new IgnitionCommand(contract as Ignition));
                    }
                    else if (contract is Shutdown)
                    {
                        commands.Add(new ShutdownCommand(contract as Shutdown));
                    }
                    else if (contract is Throttle)
                    {
                        commands.Add(new ThrottleCommand(contract as Throttle));
                    }
                    else if (contract is Stage)
                    {
                        commands.Add(new StageCommand(contract as Stage));
                    }
                    else if (contract is Orient)
                    {
                        commands.Add(new OrientCommand(contract as Orient));
                    }
                    else if (contract is Fairing)
                    {
                        commands.Add(new FairingCommand(contract as Fairing));
                    }
                    else if (contract is Retrograde)
                    {
                        commands.Add(new RetrogradeCommand(contract as Retrograde));
                    }
                }

                return commands;
            }
        }

        /// <summary>
        /// Writes commands to an xml format.
        /// </summary>
        public static void Write(string path, List<Command> commands)
        {
            using (var fileStream = new FileStream(path, FileMode.OpenOrCreate))
            {
                Serializer.Serialize(fileStream, commands);
            }
        }
    }
}
