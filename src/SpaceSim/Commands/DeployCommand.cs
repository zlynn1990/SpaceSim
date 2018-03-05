using System;
using SpaceSim.Contracts.Commands;
using SpaceSim.Spacecrafts;

namespace SpaceSim.Commands
{
    class DeployCommand : CommandBase
    {
        private string _part;

        public DeployCommand(Deploy deloy)
            : base(deloy.StartTime, deloy.Duration)
        {
            if (string.IsNullOrEmpty(deloy.Part))
            {
                throw new Exception("Deployment part must be specified in the flight profile!");
            }

            _part = deloy.Part;
        }

        public override void Initialize(SpaceCraftBase spaceCraft)
        {
            EventManager.AddMessage(string.Format("Deploying " + _part), spaceCraft);

            switch (_part)
            {
                case "Fairing":
                    spaceCraft.DeployFairing();
                    break;
                case "Grid Fins":
                    spaceCraft.DeployGridFins();
                    break;
                case "Landing Legs":
                    spaceCraft.DeployLandingLegs();
                    break;
                case "Drogues":
                    spaceCraft.DeployDrogues();
                    break;
                case "Parachutes":
                    spaceCraft.DeployParachutes();
                    break;
                default:
                    throw new Exception(string.Format("{0} is not a known deployable!", _part));
            }
        }

        public override void Finalize(SpaceCraftBase spaceCraft) { }

        public override void Update(double elapsedTime, SpaceCraftBase spaceCraft) { }
    }
}
