using HlpvwFake.Proxies;
using mshtml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HlpvwFake
{
    /// <summary>
    /// Interaction logic for NaviTool.xaml
    /// </summary>
    public partial class NaviTool : UserControl
    {
        public NaviTool()
        {
            InitializeComponent();

            addr.KeyDown += Addr_KeyDown;

            _impl = new CoreImpl();
        }

        private void Addr_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Button_Click(go, null);
            }
        }

        CoreImpl _impl;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == go)
            {
                var aloc = addr.Text;
                if (aloc == null) return;
                aloc.Trim();
                if (string.IsNullOrEmpty(aloc))
                    return;

                var source = new Uri(aloc, UriKind.RelativeOrAbsolute);
                if (!source.IsAbsoluteUri)
                {
                    aloc = "http://" + aloc;
                }
                Navigate(aloc);
                return;
            }
        }

        public void Navigate(string source)
        {
            try
            {
                var provider = CoreImpl.applicationServiceProvider;
                var _UO = provider.GetService(CoreImpl.tyIBrowserManager);
                if (_UO == null) return;
                var browserManager = new IBrowserManager(_UO);

                var contentBrowser = browserManager.ActiveBrowser;
                if (contentBrowser == null) return;

                var pi = contentBrowser._TY.GetField("webBrowserKeyDownHandler", BindingFlags.Instance | BindingFlags.NonPublic);
                if(pi!= null)
                {
                    var webBrowserKeyDownHandler = pi.GetValue(contentBrowser._UO);
                    if (webBrowserKeyDownHandler != null)
                    {
                        ((IDisposable)webBrowserKeyDownHandler).Dispose();
                    }
                }

                var helpBrowser = new IHelpViewerBrowser(contentBrowser._UO);
                var brower = helpBrowser.WebBrowser;
                if (brower != null)
                {
                }

                contentBrowser.Navigate(source);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public object StubContent
        {
            get { return stub.Content; }
            set { stub.Content = value; }
        }
        public void ManageSavedContent(string path)
        {
            var ui = new ManageUI() { Owner = Application.Current.MainWindow, ShowInTaskbar = false, WindowStartupLocation = WindowStartupLocation.CenterScreen };
            ui.ShowDialog();
        }
    }
}
