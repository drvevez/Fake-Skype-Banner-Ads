using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
namespace NoSkypeBannerAds
{

    public enum HookType : int
    {
        WH_JOURNALRECORD = 0,
        WH_JOURNALPLAYBACK = 1,
        WH_KEYBOARD = 2,
        WH_GETMESSAGE = 3,
        WH_CALLWNDPROC = 4,
        WH_CBT = 5,
        WH_SYSMSGFILTER = 6,
        WH_MOUSE = 7,
        WH_HARDWARE = 8,
        WH_DEBUG = 9,
        WH_SHELL = 10,
        WH_FOREGROUNDIDLE = 11,
        WH_CALLWNDPROCRET = 12, // only this one is used.
        WH_KEYBOARD_LL = 13,
        WH_MOUSE_LL = 14
    }

    [StructLayout(LayoutKind.Sequential)]
    struct COPYDATASTRUCT
    {
        public IntPtr dwData;
        public int cbData;
        public IntPtr lpData;
    }

    public partial class Form1 : Form
    {
        public const Int32 WM_COPYDATA = 0x4A;
        public const Int32 WM_USER = 0x0400;


        // win32 API all definitons are from www.pinvoke.net

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, IntPtr lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool UnhookWindowsHookEx(IntPtr hhk);


        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, uint nSize, string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool WritePrivateProfileString(string lpAppName,
           string lpKeyName, string lpString, string lpFileName);



        enum MessageToSkypeProcess
        {
            Hello = 0,
            UrlBannerTop = 1,
            UrlBannerRight = 2,
            close = 3
        }

        static int hWinHookWndProcRet = 0;
        static IntPtr dllInstance = IntPtr.Zero;

        static nopub n;
        static Boolean IsConnected = false;
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;
        private Boolean ForceToClose = false;

        private String IniFile = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var thedll = "DllFakeSkypeBannerAds.dll";   // the dll to hook Skype....
            dllInstance = LoadLibrary(thedll);
            IniFile = Process.GetCurrentProcess().MainModule.FileName;
            IniFile = IniFile.Substring(0, IniFile.Length - ".exe".Length) + ".ini";
            StringBuilder strb = new StringBuilder(200);


            // get default url....

            var rt = GetPrivateProfileString("url", "top", "http://hervemarchal.free.fr/skypenewbanner/blob.html", strb, (uint)strb.Capacity, IniFile);
            urlh.Text = strb.ToString();
            
            rt = GetPrivateProfileString("url", "right", "http://hervemarchal.free.fr/skypenewbanner/blob.html", strb, (uint)strb.Capacity, IniFile);
            urlr.Text = strb.ToString();
            


            DoHook();
        }

        private void DoHook()
        {
            var hookProcWndProcRet = GetProcAddress(dllInstance, "captainhookwndprocret");
            n = new nopub();
            n.findskype();
            if (n.ThreadId > 0)
            {
                hWinHookWndProcRet = SetWindowsHookEx((int)HookType.WH_CALLWNDPROCRET, hookProcWndProcRet, dllInstance, (int)n.ThreadId);
                timerConnection.Start(); // we wait to send the 1srt message
                TimerWatch.Stop();

            }
            else
            {
                newLog("Skype is not running retry in 10 secondes");
                TimerWatch.Interval = 10000;    //  10 secondes.
                TimerWatch.Start();
            }
        }

        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            SendMsg("", MessageToSkypeProcess.close); // 3 to close the fake webbrowser in skype
            if (hWinHookWndProcRet != 0) UnhookWindowsHookEx((IntPtr)hWinHookWndProcRet);
            if (dllInstance != IntPtr.Zero) FreeLibrary(dllInstance);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            if (!ForceToClose)  // if not exit by tray menu
                e.Cancel = true;    // don't close me
        }


        private void button1_Click(object sender, EventArgs e)
        {
            SendMsg(urlh.Text, MessageToSkypeProcess.UrlBannerTop);
            SendMsg(urlr.Text, MessageToSkypeProcess.UrlBannerRight);
            
            WritePrivateProfileString("url", "top", urlh.Text, IniFile);
            WritePrivateProfileString("url", "right", urlr.Text, IniFile);

        }

        /// <summary>
        /// Send a message to the hooked Skype process 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="no"></param>
        static void SendMsg(String msg, MessageToSkypeProcess no)
        {
            var c = new COPYDATASTRUCT();
            c.lpData = Marshal.StringToHGlobalAnsi(msg);
            c.cbData = msg.Length + 1; // +1 for ASCII 0 string terminator 
            c.dwData = (IntPtr)(WM_USER + no);
            IntPtr retval = Marshal.AllocHGlobal(Marshal.SizeOf(c));
            Marshal.StructureToPtr(c, retval, false);
            SendMessage(n.HwndSkype, WM_COPYDATA, IntPtr.Zero, retval);
            Marshal.FreeHGlobal(c.lpData);
            Marshal.FreeHGlobal(retval);
        }


        private void Form1_Shown(object sender, EventArgs e)
        {
            SysTrayApp();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            SendMsg(this.Handle.ToString(), MessageToSkypeProcess.Hello);// we send the handle of this window tp skype to establish the communication

        }

        // context menu Config item
        private void OnConfig(object sender, EventArgs e)
        {
            this.Show();
        }
        // context menu Exit item
        private void OnExit(object sender, EventArgs e)
        {
            ForceToClose = true;
            this.Close();
        }
        private void SysTrayApp()
        {
            // Create a simple tray menu with only one item.
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Config", OnConfig);
            trayMenu.MenuItems.Add("Exit", OnExit);

            // Create a tray icon. In this example we use a
            // standard system icon for simplicity, but you
            // can of course use your own custom icon too.
            trayIcon = new NotifyIcon();
            trayIcon.Text = "No Skype BannerAds";
            trayIcon.Icon = new Icon(SystemIcons.Shield, 40, 40);

            // Add menu to tray icon and show it.
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;
        }
        private void newLog(string msg)
        {
            Message.Text += msg + Environment.NewLine;
            Message.SelectionStart = Message.Text.Length;
            Message.ScrollToCaret();
        }
        /// <summary>
        /// override this window proc to get messages from hooked Skype process
        /// </summary>
        /// <param name="m">message received</param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_COPYDATA)
            {
                var msg = (COPYDATASTRUCT)Marshal.PtrToStructure(m.LParam, typeof(COPYDATASTRUCT));
                // a message arrives 
                if (msg.dwData == (IntPtr)(WM_USER + 2))
                {
                    if (!IsConnected)   // not connected ? ... now we are !
                    {
                        IsConnected = true;
                        timerConnection.Stop();
                        // and we send new urls to Skype ...
                        SendMsg(urlh.Text, MessageToSkypeProcess.UrlBannerTop);
                        SendMsg(urlr.Text, MessageToSkypeProcess.UrlBannerRight);
                    }
                    newLog(Marshal.PtrToStringAnsi(msg.lpData));
                }

            }
            base.WndProc(ref m);
        }

        private void TimerWatch_Tick(object sender, EventArgs e)
        {
            DoHook();
        }
    }
}
