using HlpvwFake.Proxies;
using Microsoft.Win32;
using mshtml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
    /// Interaction logic for ManageUI.xaml
    /// </summary>
    public partial class ManageUI : Window
    {
        public ManageUI()
        {
            InitializeComponent();

            Title = $"Saved Document Management - {LoaderImpl.DAT_FILE}";
            Loaded += ManageUI_Loaded;

            dirtree.SelectedItemChanged += Dirtree_SelectedItemChanged;
            filelst.SelectionChanged += Filelst_SelectionChanged;
            filelst.SelectionMode = DataGridSelectionMode.Single;
        }

        private void ManageUI_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var loc = LoaderImpl.SaveLocation;
                if (string.IsNullOrEmpty(loc) || !System.IO.Directory.Exists(loc))
                {
                    Button_Click(dir, e);
                }
                else
                {
                    location.Text = loc;
                }

                loc = location.Text;

                if (!string.IsNullOrEmpty(loc) && System.IO.Directory.Exists(loc))
                {
                    LoadRootDirectory();
                }

                return;

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

                var helpBrowse = new IHelpViewerBrowser(contentBrowser._UO);
                var meta = helpBrowse.TopicMetadata;
                this.Title = meta.HelpId;
                var ie = helpBrowse.WebBrowser;

                var doc = (HTMLDocument)ie.Document;
                dynamic item = doc.getElementsByTagName("html");
                var html = (string)item[0].outerHTML;

                //MessageBox.Show($"DocumentPageUrl: {item[0].outerHTML}", contentBrowser.DocumentTitle);
                int a = 0;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == build)
            {
                if (filelst.ItemsSource == null) return;
                IList<FileModel> selist = ((IList<FileModel>)(filelst.ItemsSource)).Where(f => f.ISCHECKED).ToList();
                if(selist.Count == 0)
                {
                    if (MessageBox.Show("No file is checked, process all?", "CONFIRM", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                        return;

                    selist = (IList<FileModel>)(filelst.ItemsSource);
                }
                var ui = new BuildUI() { Owner = this, ShowInTaskbar = false };
                ui.FileList = selist;
                ui.ShowDialog();
                return;
            }

            if (sender == exit)
            {
                this.Close();
                return;
            }

            if (sender == dir)
            {
                bool usedef;
                var loc = LoaderImpl.SelectDirectory(out usedef, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                if(usedef)
                {
                    if(string.IsNullOrEmpty(LoaderImpl.SaveLocation)) LoaderImpl.SaveLocation = loc;
                }
                else
                {
                    LoaderImpl.SaveLocation = loc;
                }
                location.Text = LoaderImpl.SaveLocation;
                LoaderImpl.SaveSettings();
                LoadRootDirectory();
                return;
            }
        }


        void LoadRootDirectory()
        {
            var rootlst = new[] { new DirModel()
            {
                Name = System.IO.Path.GetFileName(location.Text),
                Path = location.Text
            } };
            LoadSubDirectory(rootlst);

            dirtree.ItemsSource = rootlst;
        }

        void LoadSubDirectory(IList<DirModel> rootlst)
        {
            foreach(var root in rootlst)
            {
                var sublst = new List<DirModel>();
                foreach (var subdir in Directory.EnumerateDirectories(root.Path))
                {
                    sublst.Add(new DirModel()
                    {
                        Name = System.IO.Path.GetFileName(subdir),
                        Path = subdir
                    });
                }
                LoadSubDirectory(sublst);
                root.Children = sublst;
            }            
        }

        private void Dirtree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (dirtree.SelectedItem == null)
            {
                filelst.ItemsSource = null;
                return;
            }

            var dm = (DirModel)dirtree.SelectedItem;
            filelst.ItemsSource = System.IO.Directory.EnumerateFiles(dm.Path, "*.htm?").Select(f =>
            {
                var fm = new FileModel()
                {
                    FILE = System.IO.Path.GetFileName(f),
                    PATH = f
                };
                fm.NAME = SaveUI.DecodeFileName(System.IO.Path.GetFileNameWithoutExtension(f), out fm.ORDER);
                return fm;
            }).OrderBy(f => f.ORDER).ToList();
        }

        private void Filelst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var fm = filelst.SelectedItem as FileModel;
            try
            {
                if (fm != null && System.IO.Path.GetExtension(fm.PATH).StartsWith(".htm", StringComparison.InvariantCultureIgnoreCase))
                {
                    browser.Navigate(fm.PATH);
                }
                else
                {
                    browser.Navigate("about:blank");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var chk = sender as CheckBox;
            if (chk == null) return;

            var items = filelst.ItemsSource as IList<FileModel>;
            if (items == null) return;

            var ischk = chk.IsChecked.GetValueOrDefault();
            foreach (var item in items) item.ISCHECKED = ischk;
        }
    }

    class _Model : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void Notify(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        bool _ISCHECKED;
        public bool ISCHECKED
        {
            get { return _ISCHECKED; }
            set { if (_ISCHECKED != value) { _ISCHECKED = value; Notify("ISCHECKED"); } }
        }
    }

    class DirModel : _Model
    {
        public string Name { get; set; }
        public IList<DirModel> Children { get; set; }
        public string Path { get; set; }
    }

    class FileModel : _Model
    {
        public string NAME { get; set; }
        public string FILE { get; set; }
        public string PATH { get; set; }
        public int ORDER;
    }

}
