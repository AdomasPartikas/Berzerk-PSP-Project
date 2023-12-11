using Berzerk.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Berzerk.Abstraction
{
    public class PlayerControllerFactory : ControllerFactory
    {
        public override AbstractController CreateController(params object[] parameters)
        {
            if (parameters.Length != 1)
                throw new ArgumentException("1 parameters expected");
            if (!(parameters[0] is Panel))
                throw new ArgumentException("Given parameter not a Panel");

            return new PlayerController((Panel)parameters[0]);
        }
    }
}
