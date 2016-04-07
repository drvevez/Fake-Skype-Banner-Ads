using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using RGiesecke.DllExport;

namespace DllFakeSkypeBannerAds
{
    public struct POINT
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CWPRETSTRUCT
    {
        public IntPtr lResult;
        public IntPtr lParam;
        public IntPtr wParam;
        public uint message;
        public IntPtr hwnd;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;        // x position of upper-left corner
        public int Top;         // y position of upper-left corner
        public int Right;       // x position of lower-right corner
        public int Bottom;      // y position of lower-right corner
    }

    [StructLayout(LayoutKind.Sequential)]
    struct COPYDATASTRUCT
    {
        public IntPtr dwData;
        public int cbData;
        public IntPtr lpData;
    }
    public class NoPub
    {

        private const UInt32 WM_CLOSE = 0x0010;
        private const UInt32 WM_PAINT = 0xF;
        private const UInt32 WM_NCPAINT = 0x85;
        private const UInt32 WM_TIMER = 0x113;
        private const UInt32 WM_SIZE = 0x0005;
        private const UInt32 WM_MOVE = 0x0003;
        private const UInt32 WM_DESTROY = 0x0002;

        private const int GWL_HWNDPARENT = (-8);

        private const int GWL_ID = (-12);
        private const int GWL_STYLE = (-16);
        private const int GWL_EXSTYLE = (-20);

        // Window Styles 
        private const UInt32 WS_OVERLAPPED = 0;


        private const UInt32 WS_POPUP = 0x80000000;
        private const UInt32 WS_CHILD = 0x40000000;
        private const UInt32 WS_MINIMIZE = 0x20000000;
        private const UInt32 WS_VISIBLE = 0x10000000;
        private const UInt32 WS_DISABLED = 0x8000000;
        private const UInt32 WS_CLIPSIBLINGS = 0x4000000;
        private const UInt32 WS_CLIPCHILDREN = 0x2000000;
        private const UInt32 WS_MAXIMIZE = 0x1000000;
        private const UInt32 WS_CAPTION = 0xC00000;      // WS_BORDER or WS_DLGFRAME  
        private const UInt32 WS_BORDER = 0x800000;
        private const UInt32 WS_DLGFRAME = 0x400000;
        private const UInt32 WS_VSCROLL = 0x200000;
        private const UInt32 WS_HSCROLL = 0x100000;
        private const UInt32 WS_SYSMENU = 0x80000;
        private const UInt32 WS_THICKFRAME = 0x40000;
        private const UInt32 WS_GROUP = 0x20000;
        private const UInt32 WS_TABSTOP = 0x10000;
        private const UInt32 WS_MINIMIZEBOX = 0x20000;
        private const UInt32 WS_MAXIMIZEBOX = 0x10000;
        private const UInt32 WS_TILED = WS_OVERLAPPED;
        private const UInt32 WS_ICONIC = WS_MINIMIZE;
        private const UInt32 WS_SIZEBOX = WS_THICKFRAME;


        private static IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        private static IntPtr HWND_TOP = new IntPtr(0);
        private static IntPtr HWND_BOTTOM = new IntPtr(1);

        public enum SetWindowPosFlags : uint
        {
            SWP_ASYNCWINDOWPOS = 0x4000,
            SWP_DEFERERASE = 0x2000,
            SWP_DRAWFRAME = 0x0020,
            SWP_FRAMECHANGED = 0x0020,
            SWP_HIDEWINDOW = 0x0080,
            SWP_NOACTIVATE = 0x0010,
            SWP_NOCOPYBITS = 0x0100,
            SWP_NOMOVE = 0x0002,
            SWP_NOOWNERZORDER = 0x0200,
            SWP_NOREDRAW = 0x0008,
            SWP_NOREPOSITION = 0x0200,
            SWP_NOSENDCHANGING = 0x0400,
            SWP_NOSIZE = 0x0001,
            SWP_NOZORDER = 0x0004,
            SWP_SHOWWINDOW = 0x0040,

            // ReSharper restore InconsistentNaming
        }

        public enum ShowWindowCommands : uint
        {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9
        }
        enum GetWindow_Cmd : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }

        static List<IntPtr> hwndclosing = null;
        //static  ChromiumWebBrowser FakeBanner = null;
        static WebBrowser FakeBanner = null;
        static IntPtr HwndBanner = IntPtr.Zero;

        public static uint LoWord(uint dwValue)
        {
            return dwValue & 0xFFFF;
        }

        public static uint HiWord(uint dwValue)
        {
            return (dwValue >> 16) & 0xFFFF;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct PAINTSTRUCT
        {
            public IntPtr hdc;
            public bool fErase;
            public RECT rcPaint;
            public bool fRestore;
            public bool fIncUpdate;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] rgbReserved;
        }



        /// <summary>
        /// HOOK ENTRY POINT 
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllExport("captainhookwndprocret", CallingConvention = CallingConvention.Winapi)]
        public static IntPtr captainwndprocret(int nCode, IntPtr wParam, IntPtr lParam)
        {


            if (lParam != IntPtr.Zero)
            {
                try
                {

                    CWPRETSTRUCT cwret;
                    InitDll();
                    cwret = (CWPRETSTRUCT)Marshal.PtrToStructure(lParam, typeof(CWPRETSTRUCT));
                    //SendMsg(String.Format("MSG  hwnd = {0} message={1:X} wParam = {2}  lParam = {3} ", cwret.hwnd, cwret.message, cwret.wParam, cwret.lParam));
                    if (HwndBanner == IntPtr.Zero) HwndBanner = GetHwndBanner();
                    if (HwndBanner != IntPtr.Zero && FakeBanner == null)
                    {
                        FakeBanner = new WebBrowser();
                        SetParent(FakeBanner.Handle, HwndBanner);
                        FakeBanner.Show();
                        AjustWebBrowser();
                    }
                    if (cwret.message == (uint)cmsg.allmsg.WM_COPYDATA)  // a message is receving from the controller process ..
                        ComputeMessage(cwret.lParam);       // we compute it
                    if (HwndBanner != IntPtr.Zero)
                    {
                        var h = FindWindowEx(HwndBanner, IntPtr.Zero, "Shell Embedding", null);
                        if (h != IntPtr.Zero)
                        {
                            ShowWindow(h, ShowWindowCommands.SW_HIDE);
                            if (h != IntPtr.Zero)
                            {
                                h = FindWindowEx(h, IntPtr.Zero, "Shell DocObject View", null);
                            }
                            if (h != IntPtr.Zero)
                            {
                                if (!hwndclosing.Contains(h))
                                {
                                    hwndclosing.Add(h);
                                    DestroyWindow(h);
                                    SendMsg(String.Format("closing Shell DocObject View {0:X}", h));
                                }
                            }
                        }
                    }

                    if (cwret.message == (uint)cmsg.allmsg.WM_NCDESTROY && hwndclosing.Contains(cwret.hwnd))
                    {
                        hwndclosing.Remove(cwret.hwnd);
                        SendMsg("remove Shell DocObject View");

                        //form.Show(nativeWindow);

                    }
                    if (cwret.hwnd == HwndBanner && (cwret.message == (uint)cmsg.allmsg.WM_MOVE || cwret.message == (uint)cmsg.allmsg.WM_SIZE) && FakeBanner != null)
                    {
                        AjustWebBrowser();
                    }

                }

                catch (Exception e)
                {
                    SendMsg(String.Format("Erreur {0}", e.Message));
                }
            }
            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }


        static Boolean IsInit = false;
        static IntPtr hWndControl = IntPtr.Zero;
        static String Urlh = "http://hervemarchal.free.fr";
        static String Urld = "http://hervemarchal.free.fr";
        static String CurrentUrl;

        static void ComputeMessage(IntPtr data)
        {
            
            var msg = (COPYDATASTRUCT)Marshal.PtrToStructure(data, typeof(COPYDATASTRUCT));
            String t = Marshal.PtrToStringAnsi(msg.lpData);
            switch ((cmsg.allmsg)msg.dwData)
            {
                case cmsg.allmsg.WM_USER:
                    hWndControl = (IntPtr)UInt32.Parse(t);
                    break;
                case cmsg.allmsg.WM_USER + 1:
                    Urlh = t;
                    break;
                case cmsg.allmsg.WM_USER + 2:
                    Urld = t;
                    break;
                case cmsg.allmsg.WM_USER + 3:  // close the fake webbrowser
                    if (FakeBanner != null)
                    {
                        FakeBanner.Dispose();
                        FakeBanner = null;
                    }
                    return; // end...
            }
            SendMsg("Message received t = " + t);
            
            if (FakeBanner != null)
            {
                RECT r;
                GetWindowRect(HwndBanner, out r);
                var width = r.Right - r.Left;
                var height = r.Bottom - r.Top;
                CurrentUrl = (height > 0 && ((float)width / (float)height) > 1) ? Urlh : Urld; // landscape or portrait ??

                var rnd = new Random();
                
                FakeBanner.Navigate(new Uri(CurrentUrl + (CurrentUrl.Contains("?") ? "&":"?")  + rnd.Next(1, 1000).ToString()));
                //FakeBanner.Load(CurrentUrl);
                
            }
        }

        static void InitDll()
        {
            if (!IsInit)
            {
                hwndclosing = new List<IntPtr>();
                IsInit = true;
            }
        }

        static void SendMsg(String msg)
        {


            
            if (hWndControl != IntPtr.Zero)
            {
                SendMessage(hWndControl, (uint)cmsg.allmsg.WM_USER + 10, IntPtr.Zero, IntPtr.Zero);

                var c = new COPYDATASTRUCT();
                c.lpData = Marshal.StringToHGlobalAnsi(msg);
                c.cbData = msg.Length + 1;
                c.dwData = (IntPtr)(cmsg.allmsg.WM_USER + 2);
                IntPtr retval = Marshal.AllocHGlobal(Marshal.SizeOf(c));
                Marshal.StructureToPtr(c, retval, false);
                
                var r = SendMessage(hWndControl, (uint)cmsg.allmsg.WM_COPYDATA, IntPtr.Zero, retval);
                

                Marshal.FreeHGlobal(c.lpData);
                Marshal.FreeHGlobal(retval);
            }
        }
        static void AjustWebBrowser()
        {
            if (HwndBanner == IntPtr.Zero) return;
            RECT r;
            GetWindowRect(HwndBanner, out r);
            
            
            var width = r.Right - r.Left;
            var height = r.Bottom - r.Top;

            MoveWindow(FakeBanner.Handle, 0, 0, width, height, true);   // window form generated by c# to containing the "Shell Embedding"
            var h = GetWindow(FakeBanner.Handle, (uint)GetWindow_Cmd.GW_CHILD); // only one children window "Shell Embedding" class

            var Url = (height > 0 && (float)width / (float)height > 1) ? Urlh : Urld;
            MoveWindow(h, 0, 0, width, height, true);
            if (Url != CurrentUrl)
            {
                CurrentUrl = Url;
                FakeBanner.Navigate(CurrentUrl);
            }
            SendMsg("Resizing Web Browser");
        }
        static IntPtr GetHwndBanner()
        {
            var h = FindWindowEx(Process.GetCurrentProcess().MainWindowHandle, IntPtr.Zero, "TChatBanner", null);
            if (h != IntPtr.Zero)
            {
                if (h != IntPtr.Zero) SendMsg(String.Format("TChatBanner {0:X}", h));
                return h;
            }
            return IntPtr.Zero;

        }

        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);


        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        static extern IntPtr PostMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern IntPtr GetWindowDC(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);


        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        static extern IntPtr BeginPaint(IntPtr hwnd, out PAINTSTRUCT lpPaint);
        [DllImport("user32.dll")]
        static extern bool EndPaint(IntPtr hWnd, [In] ref PAINTSTRUCT lpPaint);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommands nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool DestroyWindow(IntPtr hwnd);

        [DllImport("kernel32.dll", SetLastError = true)]
        [PreserveSig]
        public static extern uint GetModuleFileName
        ([In]  IntPtr hModule, [Out]  StringBuilder lpFilename, [In]    [MarshalAs(UnmanagedType.U4)]    int nSize);

    }

}
