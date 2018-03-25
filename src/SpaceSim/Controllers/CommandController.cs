using System.Collections.Generic;
using SpaceSim.Commands;
using SpaceSim.Drawing;
using SpaceSim.Spacecrafts;

namespace SpaceSim.Controllers
{
    class CommandController : SimpleFlightController
    {
        private readonly List<CommandBase> _queuedCommands;
        private readonly List<CommandBase> _activeCommands;

        public CommandController(List<CommandBase> commands, SpaceCraftBase spaceCraft, EventManager eventManager, double clockDelay)
            : base(spaceCraft, clockDelay)
        {
            _queuedCommands = new List<CommandBase>();
            _activeCommands = new List<CommandBase>();

            foreach (CommandBase command in commands)
            {
                _queuedCommands.Add(command);

                command.LoadEventManager(eventManager);
            }
        }

        // Finds the next upcoming burn time, zero if none are avaiable
        public double NextBurnTime()
        {
            foreach (CommandBase command in _queuedCommands)
            {
                if (command is IgnitionCommand)
                {
                    return command.StartTime;
                }
            }

            return 0;
        }

        public override void Update(double dt)
        {
            base.Update(dt);

            for (int i=0; i < _queuedCommands.Count; i++)
            {
                CommandBase command = _queuedCommands[i];

                if (ElapsedTime >= command.StartTime)
                {
                    command.Initialize(SpaceCraft);

                    _queuedCommands.RemoveAt(i--);

                    _activeCommands.Add(command);
                }
            }

            for (int i=0; i < _activeCommands.Count; i++)
            {
                CommandBase command = _activeCommands[i];

                if (ElapsedTime - command.StartTime >= command.Duration)
                {
                    command.Finalize(SpaceCraft);

                    _activeCommands.RemoveAt(i--);
                }
                else
                {
                    command.Update(ElapsedTime, SpaceCraft);
                }
            }
        }
    }
}
