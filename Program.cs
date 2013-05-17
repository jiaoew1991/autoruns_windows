using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autoruns.Controller;
using Autoruns.Model;

namespace Autoruns
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            //List<BaseModel> models = MainController.GetLogonList();
            MainController.GetIEList();
            Console.WriteLine("abc");
        }
    }
}
