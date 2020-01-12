using SnoopWpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Windows;

namespace SnoopWpf
{    
    public static class ManagedInjector
    {
        public static void xMain(string[] args)
        {
            Injector.LogMessage("Starting the injection process...", false);

            var windowHandle = (IntPtr)long.Parse(args[0]);
            var assemblyName = args[1];
            var className = args[2];
            var methodName = args[3];
            var settingsFile = args[4];

            var injectorData = new InjectorData
            {
                AssemblyName = assemblyName,
                ClassName = className,
                MethodName = methodName,
                SettingsFile = settingsFile
            };

            Injector.Launch(windowHandle, injectorData);

            //check to see that it was injected, and if not, retry with the main window handle.
            var process = GetProcessFromWindowHandle(windowHandle);
            if (process != null && !CheckInjectedStatus(process) && process.MainWindowHandle != windowHandle)
            {
                Injector.LogMessage("Could not inject with current handle... retrying with MainWindowHandle", true);
                Injector.Launch(process.MainWindowHandle, injectorData);
                CheckInjectedStatus(process);
            }
        }

        internal static Process GetProcessFromWindowHandle(IntPtr windowHandle)
        {
            int processId;
            GetWindowThreadProcessId(windowHandle, out processId);
            if (processId == 0)
            {
                Injector.LogMessage(string.Format("could not get process for window handle {0}", windowHandle), true);
                return null;
            }

            var process = Process.GetProcessById(processId);
            if (process == null)
            {
                Injector.LogMessage(string.Format("could not get process for PID = {0}", processId), true);
                return null;
            }
            return process;
        }

        internal static bool CheckInjectedStatus(Process process)
        {
            bool containsFile = false;
            process.Refresh();
            foreach (ProcessModule module in process.Modules)
            {
                if (module.FileName.Contains("ManagedInjector"))
                {
                    containsFile = true;
                }
            }
            if (containsFile)
            {
                Injector.LogMessage(string.Format("Successfully injected Snoop for process {0} (PID = {1})", process.ProcessName, process.Id), true);
            }
            else
            {
                Injector.LogMessage(string.Format("Failed to inject for process {0} (PID = {1})", process.ProcessName, process.Id), true);
            }

            return containsFile;
        }

        [DllImport("user32.dll")]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int processId);
    }

    class WindowHelper
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string className, string windowName);

        delegate bool EnumWindowsWndProc(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr EnumWindows(EnumWindowsWndProc proc, IntPtr lp);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern int GetClassName(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 2)]StringBuilder lpClassName, int nMaxCount);

        bool pattern_wndproc(IntPtr hwnd, IntPtr lp)
        {
            if (GetClassName(hwnd, s_classname, 260) != 0)
            {
                var name = s_classname.ToString();
                if (name.Contains(s_pattern))
                {
                    s_hwnd = hwnd;
                    return false;
                }
            }
            return true;
        }

        StringBuilder s_classname = new StringBuilder(260);
        string s_pattern;
        IntPtr s_hwnd;

        public IntPtr GetHwndWrapper(string pattern)
        {
            this.s_hwnd = IntPtr.Zero;
            this.s_pattern = pattern;

            EnumWindows(pattern_wndproc, IntPtr.Zero);
            return s_hwnd;
        }
    }
}

namespace HlpViewerEx
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                var exefile = @"C:\Program Files (x86)\Microsoft Help Viewer\v2.3\HlpViewer.Ms.exe";
                var winclsname = "HwndWrapper[HlpViewer.Ms.exe;";
                if (!System.IO.File.Exists(exefile))
                {
                    exefile = @"C:\Program Files (x86)\Microsoft Help Viewer\v2.3\HlpViewer.exe";
                    winclsname = "HwndWrapper[HlpViewer.exe;";
                }
                Process.Start(exefile, string.Join(" ", args)); // GetProcessFromWindowHandle(windowHandle);
                var s = new WindowHelper();
                int c = 0;
                while (c < 10)
                {
                    c++;
                    Thread.Sleep(1500);

                    var hwnd = s.GetHwndWrapper(winclsname);
                    if (hwnd == IntPtr.Zero) continue;

                    var process = ManagedInjector.GetProcessFromWindowHandle(hwnd);

                    if (process != null && process.MainWindowHandle != null && !ManagedInjector.CheckInjectedStatus(process))
                    {
                        Thread.Sleep(1500);

                        var loc = typeof(HlpViewerEx.Program).Assembly.Location;
                        loc = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(loc), "HlpvwFake.dll");
                        var injectorData = new InjectorData
                        {
                            AssemblyName = loc,
                            ClassName = "LoaderImpl",
                            MethodName = "Worker",
                            SettingsFile = "Woo-hoo, Snoopy is here!"
                        };

                        Injector.LogMessage("Could not inject with current handle... retrying with MainWindowHandle", true);
                        Injector.Launch(process.MainWindowHandle, injectorData);
                        ManagedInjector.CheckInjectedStatus(process);
                        break;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}


/*
"C:\Program Files (x86)\Microsoft Help Viewer\v2.3\HlpViewer.exe" /catalogName VisualStudio15 /helpQuery method=f1&query=System.Windows.EventManager.RegisterClassHandler%00SolutionItemsProject&LCID=1033&TargetFrameworkMoniker=.NETFramework,Version%3Dv4.0&DevLang=csharp /locale en-US /sku 2000 /launchingApp Microsoft,VisualStudio,15.0

    The satellite assembly for fallback culture "en" either could not be found or could not be loaded.
*/
