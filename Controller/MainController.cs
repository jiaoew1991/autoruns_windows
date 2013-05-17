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
    class MainController
    {
        public const string SOFTWARE_CURRENTVERSION_RUN = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        public const string USER_SOFTWARE_CURRENTVERSION_RUN = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        public static List<BaseModel> GetLogonList()
        {
            List<BaseModel> modelList = new List<BaseModel>();
            //RegistryReader regReader = new RegistryReader(key);
            //modelList.AddRange(regReader.GetModelListByValue());
            RegistryKey key = Registry.LocalMachine.OpenSubKey(SOFTWARE_CURRENTVERSION_RUN);
            modelList.AddRange(GetModelListByKey(key));
            //regReader = new RegistryReader(key);
            //modelList.AddRange(regReader.GetModelListByValue());
            key = Registry.CurrentUser.OpenSubKey(USER_SOFTWARE_CURRENTVERSION_RUN);
            modelList.AddRange(GetModelListByKey(key));
            return modelList;
        }

        public const string SOFTWARE_EXPLORER_BROWSER = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Browser Helper Objects";
        public const string SOFTWARE_INTERNET_EXTENSIONS = "Software\\Microsoft\\Internet Explorer\\Extensions";

        public const string SOFTWARE_CLSID = "Software\\Classes\\CLSID";
        public const string INPROCSERVER = "InprocServer32";
        public static List<BaseModel> GetIEList()
        {
            List<BaseModel> modelList = new List<BaseModel>();
            RegistryKey dataKey = Registry.LocalMachine.OpenSubKey(SOFTWARE_CLSID);
            modelList.AddRange(MakeListByKey(Registry.LocalMachine.OpenSubKey(SOFTWARE_EXPLORER_BROWSER), dataKey));
            modelList.AddRange(MakeListByKey(Registry.LocalMachine.OpenSubKey(SOFTWARE_INTERNET_EXTENSIONS)));
            return modelList;
        }
        public const string SYSTEM_SERVICES = "System\\CurrentControlSet\\Services";
        public const string IMAGE_PATH = "ImagePath";
        public const string DESCRIPTION = "Description";
        public static List<BaseModel> GetServicesList()
        {
            List<BaseModel> modelList = new List<BaseModel>();
            Dictionary<string, IFilter> dic = new Dictionary<string, IFilter>();
            dic.Add("Type", new EqualFilter<int>(16, 32));
            dic.Add("Type", new EqualFilter<string>("LocalSystem"));
            dic.Add(DESCRIPTION, new EndWithFileter(".exe"));
            dic.Add(IMAGE_PATH, new ContainFileter("svchost"));
            RegistryReader regReader = new RegistryReader(Registry.LocalMachine.OpenSubKey(SYSTEM_SERVICES));
            List<RegistryKey> regList = regReader.GetSubKeysWithFilter(dic);
            foreach (RegistryKey r in regList)
            {
                RegistryKey paramKey = r.OpenSubKey("Parameters");
                string name = "";
                if (paramKey != null)
                {
                    name = paramKey.GetValue("ServiceDLL").ToString();
                }
                BaseModel model = new FileVersionHelper(name).GetFileInfoModel();
                modelList.Add(model);
            }
            return modelList;
        }
        private static List<BaseModel> GetModelListByKey(RegistryKey key)
        {
            RegistryReader regReader = new RegistryReader(key);
            return regReader.GetModelListByValue();
        }
        private static List<BaseModel> MakeListByKey(RegistryKey objKey, RegistryKey dataKey)
        {
            List<BaseModel> modelList = new List<BaseModel>();
            string[] valueList = objKey.GetSubKeyNames();
            foreach (string value in valueList)
            {
                try
                {
                    RegistryKey rightKey = dataKey.OpenSubKey(value);
                    if (rightKey != null)
                    {
                        RegistryKey subKey = rightKey.OpenSubKey(INPROCSERVER);
                        FileVersionHelper vHelper = new FileVersionHelper(subKey.GetValue("").ToString());
                        modelList.Add(vHelper.GetFileInfoModel(rightKey.GetValue("").ToString()));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            return modelList;
        }
        private static List<BaseModel> MakeListByKey(RegistryKey objKey)
        {
            List<BaseModel> modelList = new List<BaseModel>();
            foreach (string value in objKey.GetSubKeyNames())
            {
                try
                {
                    RegistryKey subKey = objKey.OpenSubKey(value);
                    Object exec = subKey.GetValue("Exec");
                    if (exec == null)
                    {
                        exec = subKey.GetValue("Script");
                    }
                    if (exec != null)
                    {
                        FileVersionHelper fHelper = new FileVersionHelper(exec.ToString());
                        RegistryReader regReader = new RegistryReader(subKey);
                        modelList.Add(fHelper.GetFileInfoModel(regReader.GetEntryName()));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            return modelList;
        }
    }
}
