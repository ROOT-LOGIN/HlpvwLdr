using HlpvwFake.Proxies;
using Microsoft.Win32;
using mshtml;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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
    /// Interaction logic for SaveUI.xaml
    /// </summary>
    public partial class SaveUI : Window
    {
        public SaveUI()
        {
            InitializeComponent();

            Loaded += SaveUI_Loaded;
        }

        IHelpViewerBrowser helpBrowse;

        private void SaveUI_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var provider = CoreImpl.applicationServiceProvider;
                var _UO = provider.GetService(CoreImpl.tyIBrowserManager);
                if (_UO == null) return;
                var browserManager = new IBrowserManager(_UO);

                var contentBrowser = browserManager.ActiveBrowser;
                if (contentBrowser == null) return;

                /*var obj = contentBrowser.DocumentUrl;
                MessageBox.Show($"DocumentUrl: {obj}", contentBrowser.DocumentTitle);

                obj = contentBrowser.DocumentPageUrl;
                MessageBox.Show($"DocumentPageUrl: {obj}", contentBrowser.DocumentTitle);

                contentBrowser.PrintPreview();*/

                helpBrowse = new IHelpViewerBrowser(contentBrowser._UO);
                var meta = helpBrowse.TopicMetadata;
                this.Title = meta.HelpId;
                var ie = helpBrowse.WebBrowser;

                location.Text = LoaderImpl.SaveLocation;
                if (string.IsNullOrEmpty(location.Text))
                {
                    location.Text = Environment.CurrentDirectory;
                }
                var id = meta.HelpId;
                var fname = EncodeAsFileName(id);
                location.Text = System.IO.Path.Combine(location.Text, fname + ".html");


                var doc = (HTMLDocument)ie.Document;
                dynamic item = doc.getElementsByTagName("html");
                var html = (string)item[0].outerHTML;
                browser.NavigateToString(html);

                //MessageBox.Show($"DocumentPageUrl: {item[0].outerHTML}", contentBrowser.DocumentTitle);
                int a = 0;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static string EncodeAsFileName(string id)
        {
            id = id.Replace('/', '\\');
            int i = id.LastIndexOf('\\');
            if (i >= 0)
            {
                id = id.Substring(i + 1);
            }
            id = System.IO.Path.GetFileNameWithoutExtension(id);
            id = HttpUtility.UrlEncode(id);
            return DateTime.Now.ToString("yyyyMMddHHmmss") + "$" + id;
        }

        public static string DecodeFileName(string name, out int order)
        {
            order = 1;
            name = HttpUtility.UrlDecode(name);
            int k = name.IndexOf('#');
            if (k > 0)
            {
                int.TryParse(name.Substring(0, k), out order);
            }
            k = name.IndexOf('$');            
            return name.Substring(k + 1);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == cancel) { this.Close(); return; }

            if(sender == save)
            {
                if (string.IsNullOrEmpty(location.Text)) return;

                try
                {
                    var meta = helpBrowse.TopicMetadata;
                    var id = meta.HelpId;
                    var ie = helpBrowse.WebBrowser;

                    var doc = (HTMLDocument)ie.Document;
                    dynamic item = doc.getElementsByTagName("html");
                    var html = (string)item[0].outerHTML;

                    var file = location.Text;
                    System.IO.File.WriteAllText(file, html, Encoding.UTF8);
                    this.Close();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return;
            }

            if (sender == dir)
            {
                bool usedef;
                var loc = LoaderImpl.SelectDirectory(out usedef);
                if (!string.IsNullOrEmpty(loc)) location.Text = loc;
                return;
            }
        }

        public bool SaveDirect()
        {
            try
            {
                var provider = CoreImpl.applicationServiceProvider;
                var _UO = provider.GetService(CoreImpl.tyIBrowserManager);
                if (_UO == null) return false;
                var browserManager = new IBrowserManager(_UO);

                var contentBrowser = browserManager.ActiveBrowser;
                if (contentBrowser == null) return false;


                helpBrowse = new IHelpViewerBrowser(contentBrowser._UO);
                var meta = helpBrowse.TopicMetadata;
                this.Title = meta.HelpId;
                var ie = helpBrowse.WebBrowser;

                var location_Text = LoaderImpl.SaveLocation;
                if (string.IsNullOrEmpty(location_Text))
                {
                    location_Text = Environment.CurrentDirectory;
                }
                var id = meta.HelpId;
                var fname = EncodeAsFileName(id);
                location_Text = System.IO.Path.Combine(location_Text, fname + ".html");


                var doc = (HTMLDocument)ie.Document;
                dynamic item = doc.getElementsByTagName("html");
                var html = (string)item[0].outerHTML;

                System.IO.File.WriteAllText(location_Text, html, Encoding.UTF8);
                return true;
            }
            catch(Exception ex)
            {
                LoaderImpl.ShowException(ex);
                return false;
            }
        }
    }
}
