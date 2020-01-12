using HlpvwFake.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HlpvwFake
{
    class CoreImpl
    {
        internal static Assembly HlpViewer;
        Dictionary<string, object> serviceMap = new Dictionary<string, object>();
        Dictionary<string, Type> srvtypeMap = new Dictionary<string, Type>();

        public const string nameIContentBrowser = "Microsoft.VisualStudio.Help.Viewer.Application.IContentBrowser";
        public const string nameIBrowserManager = "Microsoft.VisualStudio.Help.Viewer.DockViews.IBrowserManager";

        public static ApplicationServiceProvider applicationServiceProvider;

        public static Type tyIContentBrowser;
        public static Type tyIBrowserManager;
        public static Type tyIDockViewManager;
        public static Type tyBrowserDockView;


        public object GetService(string nameService, bool useCache = true)
        {
            if(HlpViewer == null)
            {
                HlpViewer = Application.Current.GetType().Assembly;
            }

            object service = null;
            if (useCache && serviceMap.TryGetValue(nameService, out service))
            {
                return service;
            }

            var tyServiceInterface = HlpViewer.GetType(nameService);
            if (tyServiceInterface == null) return null;

            var tyApplicationServiceProvider = HlpViewer.GetType("Microsoft.VisualStudio.Help.Viewer.Application.ApplicationServiceProvider", false);
            if (tyApplicationServiceProvider == null) return null;
            var piInstance = tyApplicationServiceProvider.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
            if (piInstance == null) return null;

            var ApplicationServiceProvider = piInstance.GetValue(null, null);
            var gmiGetService = tyApplicationServiceProvider.GetMethod("GetService");
            if (gmiGetService == null) return null;

            var miGetService = gmiGetService.MakeGenericMethod(tyServiceInterface);
            try
            {
                service = miGetService.Invoke(ApplicationServiceProvider, new object[] { null });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            if (service != null)
            {
                serviceMap[nameService] = service;
                srvtypeMap[nameService] = service.GetType();
            }
            return service;
        }

        public object InvokeMethod(object _this, string method, object[] argv)
        {
            if (_this == null) return null;

            if (_this is Type)
            {
                var mi = ((Type)_this).GetMethod(method, BindingFlags.Public | BindingFlags.Static);
                if (mi == null) return null;
                var ret = mi.Invoke(null, argv);
                return ret;
            }
            else
            {
                var mi = _this.GetType().GetMethod(method);
                if (mi == null) return null;
                var ret = mi.Invoke(_this, argv);
                return ret;
            }
        }

        public object GetProperty(object _this, string property, object[] index)
        {
            if (_this == null) return null;

            if (_this is Type)
            {
                var pi = ((Type)_this).GetProperty(property, BindingFlags.Public | BindingFlags.Static);
                if (pi == null) return null;
                var ret = pi.GetValue(null, index);
                return ret;
            }
            else
            {
                var pi = _this.GetType().GetProperty(property);
                if (pi == null) return null;
                var ret = pi.GetValue(_this, index);
                return ret;
            }
        }

        public void GetProperty(object _this, string property, object value, object[] index)
        {
            if (_this == null) return;

            if (_this is Type)
            {
                var pi = ((Type)_this).GetProperty(property, BindingFlags.Public | BindingFlags.Static);
                if (pi == null) return;
                pi.SetValue(null, value, index);
            }
            else
            {
                var pi = _this.GetType().GetProperty(property);
                if (pi == null) return;
                pi.SetValue(_this, value, index);
            }
        }

        public object GetIBrowserManager()
        {
            return GetService(nameIBrowserManager);
        }
    }
}
