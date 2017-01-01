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
                typeof (Throttle), typeof(Deploy),
                typeof(Retrograde), typeof(Prograde), typeof(AutoLand),
                typeof(Cant), typeof (Pitch), typeof (RelativePitch),
                typeof(Roll), typeof(Yaw), typeof(Post)
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
                    else if (contract is Deploy)
                    {
                        commands.Add(new DeployCommand(contract as Deploy));
                    }
                    else if (contract is Retrograde)
                    {
                        commands.Add(new RetrogradeCommand(contract as Retrograde));
                    }
                    else if (contract is Prograde)
                    {
                        commands.Add(new ProgradeCommand(contract as Prograde));
                    }
                    else if (contract is AutoLand)
                    {
                        commands.Add(new AutoLandCommand(contract as AutoLand));
                    }
                    else if (contract is Cant)
                    {
                        commands.Add(new CantCommand(contract as Cant));
                    }
                    else if (contract is Pitch)
                    {
                        commands.Add(new PitchCommand(contract as Pitch));
                    }
                    else if (contract is RelativePitch)
                    {
                        commands.Add(new RelativePitchCommand(contract as RelativePitch));
                    }
                    else if (contract is Roll)
                    {
                        commands.Add(new RollCommand(contract as Roll));
                    }
                    else if (contract is Yaw)
                    {
                        commands.Add(new YawCommand(contract as Yaw));
                    }
                    else if (contract is Post)
                    {
                        commands.Add(new PostCommand(contract as Post));
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
