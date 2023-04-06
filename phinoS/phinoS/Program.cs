using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace phinoS
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Process[] processes;
            processes = Process.GetProcessesByName(Application.ProductName);
            string str = "";
            foreach (Process p in processes)
            {
                if (p.Id != Process.GetCurrentProcess().Id)
                {
                    p.Kill();
                }
                else
                    Console.WriteLine(Application.ProductName);
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
