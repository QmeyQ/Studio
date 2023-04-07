using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.AccessControl;

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
            foreach (Process p in processes)
            {
                if (p.Id != Process.GetCurrentProcess().Id)
                {
                    try
                    {
                        p.Kill();
                    }catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    if(!p.HasExited)
                    {
                        Process.Start("cmd.exe"," /c taskkill /im \"PhinoS.exe\" & exit");
                    }
                }
            }

            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
