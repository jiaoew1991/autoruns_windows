using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoruns.utils
{
    class FileUtils
    {
        public static bool DirContainFile(DirectoryInfo dir, string fileName)
        {
            foreach (FileInfo file in dir.GetFiles())
            {
                if (file.Name == fileName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
