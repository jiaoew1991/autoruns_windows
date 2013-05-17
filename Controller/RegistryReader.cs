using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autoruns.Model;
using Autoruns.utils;
using Microsoft.Win32;

namespace Autoruns.Controller
{
    class RegistryReader
    {
        RegistryKey mKey;
        public RegistryReader(RegistryKey key)
        {
            mKey = key;
        }
        public List<BaseModel> GetModelListByValue()
        {
            List<BaseModel> list = new List<BaseModel>();
            foreach (string valuname in mKey.GetValueNames())
            {
                string value = mKey.GetValue(valuname).ToString();
                value = GetPureValueName(value);
                FileVersionHelper fvHelper = new FileVersionHelper(value);
                BaseModel model = fvHelper.GetFileInfoModel();
                model.Name = valuname;
                list.Add(model);
            }
            return list;
        }
        public List<string> GetValues()
        {
            List<string> list = new List<string>();
            foreach (string valuename in mKey.GetValueNames())
            {
                list.Add(mKey.GetValue(valuename).ToString());
            }
            return list;
        }
        public string GetEntryName()
        {
            string name = "";
            foreach (string valuename in mKey.GetValueNames())
            {
                if (valuename.Contains("Text"))
                {
                    name = mKey.GetValue(valuename).ToString();
                    break;
                }
            }
            return name;
        }
        public static string GetPureValueName(string src)
        {
            src = StringUtils.RemoveTailByTag(src, " /");
            src = StringUtils.RemoveTailByTag(src, " -");
            return StringUtils.RemoveQuote(src);
        }
    }
}
