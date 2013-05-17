using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autoruns.Controller.RegistryFilter
{
    class EqualFilter<T> : IFilter
    {
        List<T> typeList = new List<T>();
        public EqualFilter(params T[] type)
        {
            typeList.AddRange(type);
        }
        public void AddType(T[] type)
        {
            typeList.AddRange(type);
        }
        public bool Filter(T value) 
        {
            foreach (T type in typeList)
            {
                if (value.Equals(type))
                {
                    return true;
                }
            }
            return false;
        }

        #region IFilter 成员

        bool IFilter.Filter(object value)
        {
            if (value is T)
                return Filter((T)value);
            return false;
        }

        #endregion
    }
}
