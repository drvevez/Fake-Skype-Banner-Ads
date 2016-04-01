using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NoSkypeBannerAds
{
    class nopub
    {


        public String Error;
        public IntPtr HwndSkype;
        public UInt32 ThreadId;
        public UInt32 SkypeProcessId;
        public Boolean MainWindowSkypeFound;

        public Boolean findskype()
        {

            ThreadId = 0;
            Error = "";
            
            MainWindowSkypeFound = false;
            var p = Process.GetProcessesByName("skype");
            if (p.Length == 0)
            {
                Error = "Skype is not running";
                return false;
            }
            var s = new StringBuilder(200);

            this.SkypeProcessId = (UInt32) p[0].Id;
            this.ThreadId = (uint) p[0].Threads[0].Id;
            HwndSkype = p[0].MainWindowHandle;
            return this.ThreadId != 0;
        }


    }

}



