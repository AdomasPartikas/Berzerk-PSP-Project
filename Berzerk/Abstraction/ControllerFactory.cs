using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Berzerk.Abstraction
{
    public abstract class ControllerFactory
    {
        public abstract AbstractController CreateController(params object[] parameters);

        //public AbstractController CreateEntity()
        //{
        //    AbstractController entity = this.MakeEntity();
        //    return entity;
        //}
    }
}
