using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoruns.Controller.RegistryFilter
{
    interface IFilter
    {
        bool Filter(Object value);
    }
}
