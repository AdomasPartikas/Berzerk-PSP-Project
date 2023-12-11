using Berzerk.Controllers;
using Berzerk.DTOs;
using Berzerk.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Berzerk.Abstraction
{
    internal class RobotControllerFactory : ControllerFactory
    {
        //MapDTO.Entity entity, RobotMindset robotMindset, RobotState robotState, RobotType robotType, Pathfind pathfind, PlayerController player, Panel gamePanel
        public override AbstractController CreateController(params object[] parameters)
        {
            if (parameters.Length != 7)
                throw new ArgumentException("7 parameters expected");
            if (!(parameters[0] is MapDTO.Entity))
                throw new ArgumentException("First parameter MapDTO.Entity expected");
            else if (!(parameters[1] is CharacterDTO.RobotMindset))
                throw new ArgumentException("Second parameter CharacterDTO.RobotMindset expected");
            else if (!(parameters[2] is CharacterDTO.RobotState))
                throw new ArgumentException("Third parameter CharacterDTO.RobotState expected");
            else if (!(parameters[3] is CharacterDTO.RobotType))
                throw new ArgumentException("Fourth parameter CharacterDTO.RobotType expected");
            else if (!(parameters[4] is Pathfind))
                throw new ArgumentException("Fifth parameter Pathfind expected");
            else if (!(parameters[5] is PlayerController))
                throw new ArgumentException("Sixth parameter PlayerController expected");
            else if (!(parameters[6] is Panel))
                throw new ArgumentException("Seventh parameter Panel expected");

            return new RobotController((MapDTO.Entity)parameters[0], (CharacterDTO.RobotMindset)parameters[1], (CharacterDTO.RobotState)parameters[2], (CharacterDTO.RobotType)parameters[3],
                (Pathfind)parameters[4], (PlayerController)parameters[5], (Panel)parameters[6]);
        }
    }
}
