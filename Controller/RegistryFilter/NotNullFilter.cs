using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoruns.Controller.RegistryFilter
{
    class NotNullFilter : IFilter
    {
        public bool Filter(Object value)
        {
            return value != null;
        }
    }
}
