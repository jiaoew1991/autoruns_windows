using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoruns.Controller.RegistryFilter
{
    class EndWithFileter : IFilter
    {
        List<string> mList = new List<string>();
        public EndWithFileter(params string[] str)
        {
            mList.AddRange(str);
        }
        public bool Filter(string src)
        {
            foreach (string s in mList)
            {
                if (src.EndsWith(s))
                {
                    return true;
                }
            }
            return false;
        }

        #region IFilter 成员

        bool IFilter.Filter(object value)
        {
            if (value is string)
                return Filter((string)value);
            return false;
        }

        #endregion
    }
}
