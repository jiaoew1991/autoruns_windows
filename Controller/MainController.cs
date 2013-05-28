using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
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
        public const string IMAGE_FILE_KEY = "Software\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options";
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
            dic.Add("Start", new EqualFilter<int>(2));
            ContainFileter svhost = new ContainFileter("svchost");
            RegistryReader regReader = new RegistryReader(Registry.LocalMachine.OpenSubKey(SYSTEM_SERVICES));
            List<RegistryKey> regList = regReader.GetSubKeys(dic);
            foreach (RegistryKey r in regList)
            {
                RegistryKey paramKey = r.OpenSubKey("Parameters");
                string name = "";
                if (!svhost.Filter(r.GetValue(IMAGE_PATH).ToString()))
                {
                    name = r.GetValue(IMAGE_PATH).ToString();
                    name = RegistryReader.GetPureValueName(name);
                }
                else if (paramKey != null)
                {
                    name = paramKey.GetValue("ServiceDLL").ToString();
                }
                else
                {
                    continue;
                }
                BaseModel model = new FileVersionHelper(name).GetFileInfoModel(StringUtils.GetLastSubString(r.Name, "\\"));
                modelList.Add(model);
            }
            return modelList;
        }
        public static List<BaseModel> GetDriversList()
        {
            List<BaseModel> modelList = new List<BaseModel>();
            Dictionary<String, IFilter> dic = new Dictionary<string, IFilter>();
            dic.Add("Type", new EqualFilter<int>(1));
            dic.Add(IMAGE_PATH, new EndWithFileter(".sys"));
            RegistryReader regReader = new RegistryReader(Registry.LocalMachine.OpenSubKey(SYSTEM_SERVICES));
            List<RegistryKey> regList = regReader.GetSubKeys(dic);
            foreach (RegistryKey r in regList)
            {
                string name = r.GetValue(IMAGE_PATH).ToString();
                if (name != null && !name.Equals(""))
                {
                    name = StringUtils.GetLastSubString(name, "system32");
                    name = "C:\\Windows\\s" + name;
                    BaseModel model = new FileVersionHelper(name).GetFileInfoModel(StringUtils.GetLastSubString(r.Name, "\\"));
                    modelList.Add(model);
                }
            }
            return modelList;
        }
        private const string WINDOWS_TASKS = "C:\\Windows\\System32\\Tasks";
        private const string SYSTEM_TASKS = "C:\\Windows\\Tasks";
        private const string COMMADN_TAG = "<Command>";
        private const string _COMMADN_TAG = "</Command>";
        public static List<BaseModel> GetSchedulerTaskList()
        {
            List<BaseModel> modelList = new List<BaseModel>();
            DirectoryInfo dir = new DirectoryInfo(WINDOWS_TASKS);
            DirectoryInfo taskDir = new DirectoryInfo(SYSTEM_TASKS);
            foreach (FileInfo file in dir.GetFiles())
            {
                if (!FileUtils.DirContainFile(taskDir, file.Name+ ".job"))
                {
                    continue;
                }
                StreamReader sReader = new StreamReader(file.FullName);
                string realName = "";
                string line = "";
                while ((line = sReader.ReadLine()) != null)
                {
                    int start = line.IndexOf(COMMADN_TAG);
                    if (start != -1)
                    {
                        realName = line.Substring(start + COMMADN_TAG.Length, line.IndexOf(_COMMADN_TAG) - COMMADN_TAG.Length - start);
                        break;
                    }
                }
                BaseModel model = new FileVersionHelper(realName).GetFileInfoModel(file.Name + ".job");
                model.ImagePath = realName;
                modelList.Add(model);
            }
            return modelList;
        }
        public const string BOOT_EXECUTE = "System\\CurrentControlSet\\Control\\Session Manager";
        public const string AUTOCHECK = "autocheck autochk *";
        public static List<BaseModel> GetBootExecuteList()
        {
            List<BaseModel> list = new List<BaseModel>();
            RegistryKey key = Registry.LocalMachine.OpenSubKey(BOOT_EXECUTE);
            object value = key.GetValue("BootExecute");
            if (value != null && !value.ToString().Equals(AUTOCHECK))
            {
                BaseModel model = new FileVersionHelper("C:\\Windows\\System32\\autochk.exe").GetFileInfoModel(AUTOCHECK);
                list.Add(model);
            }
            return list;
        }
        public static List<BaseModel> GetImageHijacks()
        {
            List<BaseModel> list = new List<BaseModel>();
            RegistryReader rReader = new RegistryReader(Registry.LocalMachine.OpenSubKey(IMAGE_FILE_KEY));
            Dictionary<string, IFilter> dic = new Dictionary<string, IFilter>();
            dic.Add("Debugger", new NotNullFilter());
            List<RegistryKey> kList = rReader.GetSubKeys(dic);
            foreach (RegistryKey k in kList)
            {
                string name = k.GetValue("Debugger").ToString();
                BaseModel model = new FileVersionHelper(name).GetFileInfoModel(new RegistryReader(k).GetEntryName());
                list.Add(model);
            }
            return list;
        }
        public const string KNOWN_DLLS = "System\\CurrentControlSet\\Control\\Session Manager\\KnownDlls";
        public static List<BaseModel> GetKnownDllsList()
        {
            List<BaseModel> list = new List<BaseModel>();
            RegistryReader rReader = new RegistryReader(Registry.LocalMachine.OpenSubKey(KNOWN_DLLS));
            List<string> ls = rReader.GetValues(new EndWithFileter(".dll"));
            foreach (string s in ls)
            {
                BaseModel model = new FileVersionHelper("C:\\Windows\\System32\\" + s).GetFileInfoModel();
                list.Add(model);
            }
            return list;
        }
        public const string PROTOCOL_CATAOG = "System\\CurrentControlSet\\Services\\WinSock2\\Parameters\\Protocol_Catalog9\\Catalog_Entries";
        public static List<BaseModel> GetWinsockProviderList()
        {
            List<BaseModel> list = new List<BaseModel>();
            RegistryReader rReader = new RegistryReader(Registry.LocalMachine.OpenSubKey(PROTOCOL_CATAOG));
            List<RegistryKey> kl = rReader.GetSubKeys(null);
            ContainFileter cf = new ContainFileter(".dll");
            foreach (RegistryKey key in kl)
            {
                string value = System.Text.Encoding.Default.GetString((byte[])key.GetValue("PackedCatalogItem"));
                value = StringUtils.RemoveTailByTag(value, "\0");
                string name = key.GetValue("ProtocolName").ToString();
                if (cf.Filter(name))
                {
                    name = RegistryReader.GetPureValueName(name);
                    FileVersionInfo info = FileVersionInfo.GetVersionInfo("C:\\Windows\\" + name.Substring(14));
                    name = info.FileDescription;
                }
                BaseModel model = new FileVersionHelper(value.Replace("%SystemRoot%", "C:\\Windows")).GetFileInfoModel(name);
                list.Add(model);
            }
            return list;
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
