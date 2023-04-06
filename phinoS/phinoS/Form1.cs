using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using Microsoft.Win32; //写入注册表时要用到
using System.Diagnostics;

namespace phinoS
{
    public partial class Form1 : Form
    {

       /* [DllImport("shell32.dll")]
        public static extern IntPtr ShellExecute(IntPtr hwnd, //窗口句柄 
            string lpOperation, //指定要进行的操作 
            string lpFile,  //要执行的程序、要浏览的文件夹或者网址 
            string lpParameters, //若lpFile参数是一个可执行程序，则此参数指定命令行参数 
            string lpDirectory, //指定默认目录 
            int nShowCmd   //若lpFile参数是一个可执行程序，则此参数指定程序窗口的初始显示方式(参考如下枚举) 
            );*/

        Process pro = new Process();
        public Form1()
        {
            pro.StartInfo.FileName = "wscript";
            pro.StartInfo.UseShellExecute = false;
            pro.StartInfo.Arguments = "\"" + System.Windows.Forms.Application.StartupPath + "\\key.vbs\"";
            pro.StartInfo.CreateNoWindow = false;
            InitializeComponent();
            //this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            KeyboardHookLib _keyboardHook = new KeyboardHookLib();
            _keyboardHook.InstallHook(this.OnKeyPress); 


        }
        String show;
        public void OnKeyPress(KeyboardHookLib.HookStruct hookStruct, int wParam, IntPtr lParam)
        {
            //是否拦截这个键t
            Keys key = (Keys)hookStruct.vkCode;
            if (key.ToString().ToLower() == textBox1.Text.ToLower() && wParam == 257)
            {
                //ShellExecute(IntPtr.Zero,"Open", "cmd.exe", " /c  \"" + /*System.Windows.Forms.Application.StartupPath*/  "key.vbs", System.Windows.Forms.Application.StartupPath, 0);
                pro.Start();
                pro.Refresh();
                textBox2.Text = pro.StartInfo.Arguments;
                return;
            }
            if (show != hookStruct.ToString())
            {
                textBox2.Text = key.ToString() + ":" + wParam + ":" + lParam ;
                show = hookStruct.ToString();
            }

        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }
    }




    public class KeyboardHookLib
        {
            private const int WH_KEYBOARD_LL = 13; //键盘
                                                   //键盘处理事件委托 ,当捕获键盘输入时调用定义该委托的方法.
            private delegate int HookHandle(int nCode, int wParam, IntPtr lParam);
            //客户端键盘处理事件
            public delegate void ProcessKeyHandle(HookStruct param, int wParam, IntPtr lParam);
            //接收SetWindowsHookEx返回值
            private static int _hHookValue = 0;
            //勾子程序处理事件
            private HookHandle _KeyBoardHookProcedure;
            //Hook结构
            [StructLayout(LayoutKind.Sequential)]
            public class HookStruct
            {
                public int vkCode;
                public int scanCode;
                public int flags;
                public int time;
                public int dwExtraInfo;
            }
            //设置钩子 
            [DllImport("user32.dll")]
            private static extern int SetWindowsHookEx(int idHook, HookHandle lpfn, IntPtr hInstance, int threadId);
            //取消钩子 
            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            private static extern bool UnhookWindowsHookEx(int idHook);
            //调用下一个钩子 
            [DllImport("user32.dll")]
            private static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);
            //获取当前线程ID
            [DllImport("kernel32.dll")]
            private static extern int GetCurrentThreadId();
            //Gets the main module for the associated process.
            [DllImport("kernel32.dll")]
            private static extern IntPtr GetModuleHandle(string name);
            private IntPtr _hookWindowPtr = IntPtr.Zero;
            //构造器
            public KeyboardHookLib() { }
            //外部调用的键盘处理事件
            private static ProcessKeyHandle _clientMethod = null;
            /// <summary>
            /// 安装勾子
            /// </summary>
            /// <param name="hookProcess">外部调用的键盘处理事件</param>
            public void InstallHook(ProcessKeyHandle clientMethod)
            {
                _clientMethod = clientMethod;
                // 安装键盘钩子 
                if (_hHookValue == 0)
                {
                    _KeyBoardHookProcedure = new HookHandle(OnHookProc);
                    _hookWindowPtr = GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);
                    _hHookValue = SetWindowsHookEx(
                    WH_KEYBOARD_LL,
                    _KeyBoardHookProcedure,
                    _hookWindowPtr,
                    0);
                    //如果设置钩子失败. 
                    if (_hHookValue == 0) UninstallHook();
                }
            }
            //取消钩子事件 
            public void UninstallHook()
            {
                if (_hHookValue != 0)
                {
                    bool ret = UnhookWindowsHookEx(_hHookValue);
                    if (ret) _hHookValue = 0;
                }
            }
            //钩子事件内部调用,调用_clientMethod方法转发到客户端应用。
            private static int OnHookProc(int nCode, int wParam, IntPtr lParam)
            {
                if (nCode >= 0)
                {
                    //转换结构
                    HookStruct hookStruct = (HookStruct)Marshal.PtrToStructure(lParam, typeof(HookStruct));
                    if (_clientMethod != null)
                    {
                        bool handle = false;
                        //调用客户提供的事件处理程序。
                        _clientMethod(hookStruct, wParam, lParam);
                        if (handle) return 1; //1:表示拦截键盘,return 退出
                    }
                }
                return CallNextHookEx(_hHookValue, nCode, wParam, lParam);
            }
        }
    }