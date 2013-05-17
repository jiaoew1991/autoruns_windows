using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoruns.utils
{
    class StringUtils
    {
        public static string RemoveQuote(string src)
        {
            const string QUOTE = "\"";
            int start = src.IndexOf(QUOTE);
            if (start == -1)
            {
                return src;
            }
            int end = src.LastIndexOf(QUOTE);
            if (start == end)
            {
                return src;
            }
            string rst = src.Substring(start + QUOTE.Length, end - start - QUOTE.Length);
            return rst;
        }
        public static string RemoveTailByTag(string src, string tag)
        {
            int end = src.IndexOf(tag);
            if (end == -1) 
                return src;
            return src.Substring(0, end);
        }
    }
}
