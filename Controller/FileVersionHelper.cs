using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autoruns.Model;
using Autoruns.utils;

namespace Autoruns.Controller
{
    class FileVersionHelper
    {
        string exeName;
        FileVersionInfo fileInfo = null;

        public FileVersionHelper(string name)
        {
            exeName = name;
            try
            {
                fileInfo = FileVersionInfo.GetVersionInfo(exeName);
            }
            catch (Exception e)
            {
                fileInfo = null;
            }
        }
        public BaseModel GetFileInfoModel()
        {
            BaseModel model = new BaseModel();
            if (fileInfo != null)
            {
                model.Description = fileInfo.FileDescription;
                model.ImagePath = exeName;
                model.Publisher = fileInfo.CompanyName;
                model.Name = fileInfo.OriginalFilename;
            }
            else
            {
                model.ImagePath = "File not Found: " + exeName;
            }
            return model;
        }
        public BaseModel GetFileInfoModel(string name)
        {
            BaseModel model = GetFileInfoModel();
            model.Name = name;
            return model;
        }
    }
}
