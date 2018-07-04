using System.Collections.Generic;
using SpaceSim.Common;
using SpaceSim.Common.Contracts.Commands;

namespace SpaceSim.Commands
{
    static class CommandManager
    {
        /// <summary>
        /// Loads commands from an xml format.
        /// </summary>
        public static List<CommandBase> Load(string path)
        {
            List<Command> contracts = CommandReader.Read(path);

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
                else if (contract is Terminate)
                {
                    commands.Add(new TerminateCommand(contract as Terminate));
                }
                else if (contract is Release)
                {
                    commands.Add(new ReleaseCommand(contract as Release));
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
                else if (contract is Rate)
                {
                    commands.Add(new RateCommand(contract as Rate));
                }
                else if (contract is Target)
                {
                    commands.Add(new TargetCommand(contract as Target));
                }
                else if (contract is Zoom)
                {
                    commands.Add(new ZoomCommand(contract as Zoom));
                }
            }

            return commands;
        }
    }
}
