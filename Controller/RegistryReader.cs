using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autoruns.Controller.RegistryFilter;
using Autoruns.Model;
using Autoruns.utils;
using Microsoft.Win32;

namespace Autoruns.Controller
{
    class RegistryReader
    {
        RegistryKey mKey;
        string baseName;
        public RegistryReader(RegistryKey key)
        {
            mKey = key;
            string s = key.ToString();
            baseName = s.Substring(s.LastIndexOf("\\") + 1);
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
        public string GetValueByName(string name)
        {
            return mKey.GetValue(name).ToString();
        }
        public List<string> GetValues(params IFilter[] filters)
        {
            List<string> values = new List<string>();
            foreach (string valueName in mKey.GetValueNames())
            {
                string value = mKey.GetValue(valueName).ToString();
                bool isFit = true;
                foreach (IFilter f in filters)
                {
                    if (!f.Filter(value))
                    {
                        isFit = false;
                        break;
                    }
                }
                if (isFit)
                {
                    values.Add(value);
                }
            }
            return values;
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
        public List<RegistryKey> GetSubKeys(Dictionary<string, IFilter> fDic)
        {
            List<RegistryKey> regList = new List<RegistryKey>();
            foreach (string subName in mKey.GetSubKeyNames())
            {
                RegistryKey key = mKey.OpenSubKey(subName);
                bool rst = true;
                if (fDic != null)
                {
                    foreach (KeyValuePair<string, IFilter> kv in fDic)
                    {
                        object regValue = key.GetValue(kv.Key);
                        if (regValue is string)
                        {
                            regValue = GetPureValueName(regValue.ToString());
                        }
                        if (!kv.Value.Filter(regValue))
                        {
                            rst = false;
                            break;
                        }
                    }
                }
                if (rst)
                {
                    regList.Add(key);
                }
            }
            return regList;
        }
        public string GetEntryName()
        {
            if (!baseName.StartsWith("{"))
            {
                return baseName;
            }
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
            src = StringUtils.RemoveTailByTag(src, ",-");
            return StringUtils.RemoveQuote(src);
        }
    }
}
