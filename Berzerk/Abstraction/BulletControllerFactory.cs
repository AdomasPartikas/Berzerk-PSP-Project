using Berzerk.Controllers;
using Berzerk.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Berzerk.Abstraction
{
    internal class BulletControllerFactory : ControllerFactory
    {
        public override AbstractController CreateController(params object[] parameters)
        {
            if (parameters.Length != 3)
                throw new ArgumentException("3 parameters expected");
            if (!(parameters[0] is MapDTO.Entity))
                throw new ArgumentException("First parameter MapDto.Entity expected");
            else if (!(parameters[1] is int))
                throw new ArgumentException("Second parameter Integer expected");
            else if (!(parameters[2] is CharacterDTO.Direction))
                throw new ArgumentException("Third parameter CharacterDTO.Direction expected");

            return new BulletController((MapDTO.Entity)parameters[0], (int)parameters[1], (CharacterDTO.Direction)parameters[2]);
        }
    }
}
