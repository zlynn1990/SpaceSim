using System.Collections.Generic;
using SpaceSim.Commands;
using SpaceSim.Drawing;
using SpaceSim.Spacecrafts;

namespace SpaceSim.Controllers
{
    class CommandController : SimpleFlightController
    {
        private List<CommandBase> _commands;

        private List<CommandBase> _queuedCommands;
        private List<CommandBase> _activeCommands;

        public CommandController(List<CommandBase> commands, ISpaceCraft spaceCraft, EventManager eventManager)
            : base(spaceCraft)
        {
            _commands = commands;

            _queuedCommands = new List<CommandBase>();
            _activeCommands = new List<CommandBase>();

            foreach (CommandBase command in _commands)
            {
                _queuedCommands.Add(command);

                command.LoadEventManager(eventManager);
            }
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
