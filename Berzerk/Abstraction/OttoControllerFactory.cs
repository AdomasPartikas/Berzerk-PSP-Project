using Berzerk.Controllers;
using Berzerk.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Berzerk.Abstraction
{
    internal class OttoControllerFactory : ControllerFactory
    {
        public override AbstractController CreateController(params object[] parameters)
        {
            if (parameters.Length != 3)
                throw new ArgumentException("3 parameters expected ");
            if (!(parameters[0] is Pathfind))
                throw new ArgumentException("First parameter Pathfind expected");
            if (!(parameters[1] is Panel))
                throw new ArgumentException("Second parameter Panel expected");
            if (!(parameters[2] is PlayerController))
                throw new ArgumentException("Third parameter PlayerController expected");

            return new OttoController((Pathfind)parameters[0], (Panel)parameters[1], (PlayerController)parameters[2]);
        }
    }
}
