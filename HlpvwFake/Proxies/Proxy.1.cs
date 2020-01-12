using Microsoft.VisualStudio.Help.Viewer.DockViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace HlpvwFake.Proxies
{
    class ApplicationServiceProvider : _ProxyImpl
    {
        public const string _FullName = "Microsoft.VisualStudio.Help.Viewer.Application.ApplicationServiceProvider";
        public override string FullName => _FullName;

        public ApplicationServiceProvider(object uo) : base(uo)
        {

        }

        public object GetService(Type type, object context = null)
        {
            var gmi = _TY.GetMethod("GetService", BindingFlags.Public | BindingFlags.Instance);
            var mi = gmi.MakeGenericMethod(type);
            try
            {
                return mi.Invoke(_UO, new[] { context });
            }
            catch
            {
                return null;
            }
        }

        public bool HasServiceProvider(Type type)
        {
            var gmi = _TY.GetMethod("HasServiceProvider", BindingFlags.Public | BindingFlags.Instance);
            var mi = gmi.MakeGenericMethod(type);
            return (bool)mi.Invoke(_UO, null);
        }
    }

    class IContentBrowser : _ProxyImpl
    {
        public const string _FullName = "Microsoft.VisualStudio.Help.Viewer.Application.IContentBrowser";
        public override string FullName => _FullName;

        public IContentBrowser(object uo) : base(uo)
        {

        }

        public Uri DocumentPageUrl
        {
            get { return (Uri)GetProperty("DocumentPageUrl"); }
        }

        public string DocumentTitle
        {
            get { return (string)GetProperty("DocumentTitle"); }
        }

        public Uri DocumentUrl
        {
            get { return (Uri)GetProperty("DocumentUrl"); }
        }

        public bool HasDocument
        {
            get { return (bool)GetProperty("HasDocument"); }
        }

        public bool HasErrorPage
        {
            get { return (bool)GetProperty("HasErrorPage"); }
        }

        public void Navigate(string url)
        {
            Invoke("Navigate", new object[] { url }, new Type[] { typeof(string) });
        }

        public void NavigateToContent(string content)
        {
            Invoke("NavigateToContent", new object[] { content }, new Type[] { typeof(string) });
        }

        public void Print()
        {
            Invoke("Print", null);
        }

        public void PrintPreview()
        {
            Invoke("PrintPreview", null);
        }

        public void Refresh()
        {
            Invoke("Refresh", null);
        }

        public void Stop()
        {
            Invoke("Stop", null);
        }

    }

    class TopicMetadata : _ProxyImpl
    {
        public const string _FullName = "Microsoft.VisualStudio.Help.Viewer.Domain.TopicMetadata";
        public override string FullName => _FullName;

        public TopicMetadata(object uo) : base(uo)
        {
        }


        public string HelpId
        {
            get { return (string)GetProperty("HelpId"); }
        }

        public bool IsError
        {
            get { return (bool)GetProperty("IsError"); }
        }

        public string TopicLocale
        {
            get { return (string)GetProperty("TopicLocale"); }
        }

        public string TopicVendor
        {
            get { return (string)GetProperty("TopicVendor"); }
        }

        public string TopicVersion
        {
            get { return (string)GetProperty("TopicVersion"); }
        }
    }

    class IHelpViewerBrowser : IContentBrowser
    {
        public new const string _FullName = "Microsoft.VisualStudio.Help.Viewer.Application.IHelpViewerBrowser";
        public override string FullName => _FullName;

        public IHelpViewerBrowser(object uo) : base(uo)
        {

        }

        public string F1SearchTerm
        {
            get { return (string)GetProperty("F1SearchTerm"); }
        }


        public TopicMetadata TopicMetadata
        {
            get
            {
                var uo = GetProperty("TopicMetadata");
                if (uo != null) return new TopicMetadata(uo);
                return null;
            }
        }

        public WebBrowser WebBrowser
        {
            get { return (WebBrowser)GetProperty("WebBrowser"); }
        }

        public void MoveFind(ExecutedRoutedEventArgs e, bool next)
        {
            Invoke("MoveFind", new object[] { e, next }, new Type[] { typeof(ExecutedRoutedEventArgs), typeof(bool) });
        }

        public void ShowTextFinder()
        {
            Invoke("ShowTextFinder", null);
        }
    }

    class IBrowserManager : _ProxyImpl
    {
        public const string _FullName = "Microsoft.VisualStudio.Help.Viewer.DockViews.IBrowserManager";
        public override string FullName => _FullName;

        public IBrowserManager(object uo) : base(uo)
        {

        }

        public IContentBrowser ActiveBrowser
        {
            get 
            {
                var uo = GetProperty("ActiveBrowser");
                if(uo != null) return new IContentBrowser(uo);
                return null;
            }
        }

        public BrowserDockView ActiveBrowserView
        {
            get { return (BrowserDockView)GetProperty("ActiveBrowserView"); }
        }

        bool CanCreateBrowser
        {
            get { return (bool)GetProperty("CanCreateBrowser"); }
        }

        object DataContext
        {
            get { return (bool)GetProperty("DataContext"); }
        }

        bool HasBrowser
        {
            get { return (bool)GetProperty("HasBrowser"); }
        }

        public IEnumerable<BrowserDockView> VisibleBrowserViews
        {
            get { return (IEnumerable<BrowserDockView>)GetProperty("VisibleBrowserViews"); }
        }

        public bool CreateBrowser(string url = null, bool noFocus = false)
        {
            return (bool)Invoke("CreateBrowserr", new object[] { url, noFocus }, new Type[] { typeof(string), typeof(bool) });
        }

        public void InitializeBrowser(BrowserDockView browserView, string launchUrl = null)
        {
            Invoke("InitializeBrowser", new object[] { browserView, launchUrl }, new Type[] { typeof(BrowserDockView), typeof(string) });
        }

        public void NavigateBrowser(string url, bool noFocus = false)
        {
            Invoke("NavigateBrowser", new object[] { url, noFocus }, new Type[] { typeof(string), typeof(bool) });
        }

        public void RefreshBrowsers()
        {
            Invoke("RefreshBrowsers", null);
        }
    }

    class IDockViewManager : _ProxyImpl
    {
        public const string _FullName = "Microsoft.VisualStudio.Help.Viewer.DockViews.IDockViewManager";
        public override string FullName => _FullName;

        public IDockViewManager(object uo) : base(uo) { }

        public DockView ActiveView
        {
            get { return (DockView)GetProperty("ActiveView"); }
        }

        public bool Activate(DockViewType viewType)
        {
            return (bool)Invoke("Activate", new object[] { viewType }, new Type[] { typeof(DockViewType) });
        }

        public DockView Get(DockViewType viewType)
        {
            return (DockView)Invoke("Get", new object[] { viewType }, new Type[] { typeof(DockViewType) });
        }

        public bool Has(DockViewType viewType)
        {
            return (bool)Invoke("Has", new object[] { viewType }, new Type[] { typeof(DockViewType) });
        }

        // public void Initialize(WindowProfile profile, params DockViewState[] viewStates);

        public bool IsActive(DockViewType viewType)
        {
            return (bool)Invoke("IsActive", new object[] { viewType }, new Type[] { typeof(DockViewType) });
        }

        public void Reset()
        {
            Invoke("Reset", null);
        }

        public void StopTracking(DockView view)
        {
            Invoke("StopTracking", new object[] { view}, new Type[] { typeof(DockView ) });
        }

        public void Track(DockView view)
        {
            Invoke("Track", new object[] { view }, new Type[] { typeof(DockView) });
        }

        public void Use(DockView view)
        {
            Invoke("Use", new object[] { view }, new Type[] { typeof(DockView) });
        }

        /*static Type tyEventArgDockView;

        public static Delegate CreateEventHandler(Delegate handler)
        {
            if(tyEventArgDockView == null)
            {
                var ty = CoreImpl.HlpViewer.GetType("Microsoft.VisualStudio.Help.Viewer.Additions.EventArg<>");
                if (ty == null) return null;

                tyEventArgDockView = ty.MakeGenericType(typeof(DockView));
            }
            //EventArg<DockView>
            var dm = new DynamicMethod("", typeof(void), new Type[] { typeof(object), tyEventArgDockView });
            Delegate.CreateDelegate()
            dm.CreateDelegate()
        }*/

        public event EventHandler<EventArgs> Activated
        {
            add
            {
                AddEventHandler("Activated", value);
            }
            remove
            {
                RemoveEventHandler("Activated", value);
            }
        }

        //event EventHandler<EventArg<DockView>> ContentShown;

        //event EventHandler<EventArg<DockView>> Deactivated;

        event EventHandler ViewsReset
        {
            add { AddEventHandler("ViewsReset", value); }
            remove  { RemoveEventHandler("ViewsReset", value); }
        }
    }
}
