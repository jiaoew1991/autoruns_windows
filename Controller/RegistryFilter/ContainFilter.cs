using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoruns.Controller.RegistryFilter
{
    class ContainFileter : IFilter
    {
        List<string> mList = new List<string>();
        public ContainFileter(params string[] str)
        {
            mList.AddRange(str);
        }
        public bool Filter(string src)
        {
            foreach (string s in mList)
            {
                if (src.Contains(s))
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
