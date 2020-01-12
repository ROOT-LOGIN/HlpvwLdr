using HlpvwFake;
using HlpvwFake.Proxies;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

public class LoaderImpl
{
    static MainTool myTools;
    static object myTag;

    public static void Worker(string args)
    {
        System.Windows.MessageBox.Show(args);

        if (myTools != null) return;
        myTag = new object();

        try
        {
            System.Windows.Forms.Application.EnableVisualStyles();

            LoadSettings();

            var appAssembly = Application.Current.GetType().Assembly;
            // init types
            CoreImpl.tyBrowserDockView = typeof(Microsoft.VisualStudio.Help.Viewer.DockViews.BrowserDockView);
            CoreImpl.tyIDockViewManager = appAssembly.GetType(IDockViewManager._FullName);
            CoreImpl.tyIBrowserManager = appAssembly.GetType(IBrowserManager._FullName);
            CoreImpl.tyIContentBrowser = appAssembly.GetType(IContentBrowser._FullName);
            // get ApplicationServiceProvider
            var pi = appAssembly.GetType(ApplicationServiceProvider._FullName).GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
            var _UO = pi.GetValue(null, null);
            CoreImpl.applicationServiceProvider = new ApplicationServiceProvider(_UO);


            Application.Current.MainWindow.PreviewKeyDown += MainWindow_KeyDown;

            var serviceUO = CoreImpl.applicationServiceProvider.GetService(CoreImpl.tyIDockViewManager);
            if(serviceUO != null)
            {
                try
                {
                    var dockMgr = new IDockViewManager(serviceUO);
                    AdjustHlpViwContextMenu(dockMgr.ActiveView);

                    dockMgr.Activated += (aa, bb) =>
                    {
                        AdjustHlpViwContextMenu(aa);
                    };
                }
                catch (Exception ex)
                {
                    LoaderImpl.ShowException(ex);
                }
            }

            var ty = Application.Current.MainWindow.GetType();
            var ftb = ty.GetField("viewerToolBar", BindingFlags.Instance | BindingFlags.NonPublic);
            if (ftb == null) { System.Windows.MessageBox.Show("viewerToolBar field is not found"); return; }

            var tb = ftb.GetValue(Application.Current.MainWindow) as FrameworkElement;
            if (tb == null) { System.Windows.MessageBox.Show("viewerToolBar instance not found"); return; }

            var fbd = tb.GetType().GetField("mainToolBar", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fbd == null) { System.Windows.MessageBox.Show("viewerToolBar does not contain mainToolBar"); return; }

            var bd = fbd.GetValue(tb) as Border;
            if (bd == null) { System.Windows.MessageBox.Show("mainToolBar is not a Border"); return; }

            var wp = bd.Child as WrapPanel;
            if (bd == null) { System.Windows.MessageBox.Show("mainToolBar does not have a WrapPanel child"); return; }

            var mytb = new MainTool() { Margin = new Thickness(16, 0, 0, 0) };
            wp.Children.Add(mytb);

            myTools = mytb;

            var fly = ty.GetField("LayoutRoot", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fly == null) { System.Windows.MessageBox.Show("LayoutRoot field is not found"); return; }

            var layout = fly.GetValue(Application.Current.MainWindow) as Grid;
            if (layout == null) { System.Windows.MessageBox.Show("LayoutRoot is not a Grid"); return; }

            var ViewerStatusBar = layout.Children.OfType<FrameworkElement>().FirstOrDefault(d => d.GetType().Name == "ViewerStatusBar");
            if(ViewerStatusBar != null)
            {
                var navi = new NaviTool();
                navi.Height = ViewerStatusBar.ActualHeight;
                Grid.SetColumn(navi, Grid.GetColumn(ViewerStatusBar));
                Grid.SetRow(navi, Grid.GetRow(ViewerStatusBar));
                Grid.SetRowSpan(navi, Grid.GetRowSpan(ViewerStatusBar));
                Grid.SetColumnSpan(navi, Grid.GetColumnSpan(ViewerStatusBar));
                layout.Children.Remove(ViewerStatusBar);
                navi.StubContent = ViewerStatusBar;
                layout.Children.Add(navi);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }

    private static void AdjustHlpViwContextMenu(object view)
    {
        if (view == null) return;

        var fpi = view.GetType().GetProperty("Browser", BindingFlags.Instance | BindingFlags.NonPublic);
        if (fpi == null) return;
        var broswer = fpi.GetValue(view);
        if (broswer == null) return;
        var ffi = broswer.GetType().GetField("contextMenu", BindingFlags.NonPublic | BindingFlags.Instance);
        if (ffi == null) return;
        var menu = ffi.GetValue(broswer) as ContextMenu;
        if (menu == null) return;
        if (menu.ItemsSource != null)
        {

        }
        else
        {
            if (menu.Items.Count > 0)
            {
                var last = menu.Items[menu.Items.Count - 1] as MenuItem;
                if (last == null) return;
                if (last.Tag == myTag) return;
            }

            menu.Items.Add(new Separator() { Tag = myTag });
            var mi = new MenuItem() { Tag = myTag, Header = "Save ..." };
            mi.Click += (aa, bb) => { myTools.SaveCurrentContent(null); };
            menu.Items.Add(mi);
            mi = new MenuItem() { Tag = myTag, Header = "Direct Save" };
            mi.Click += (aa, bb) => { myTools.SaveCurrentContent(true); };
            menu.Items.Add(mi);
            mi = new MenuItem() { Tag = myTag, Header = "About ..." };
            mi.Click += (aa, bb) => 
            {
                MessageBox.Show
(@"Woo-hoo, Snoopy is here!

Glad to see you!

Power by Snoop, A wpf injection method.", 
                "ABOUT ...", MessageBoxButton.OK, MessageBoxImage.Information);
            };
            menu.Items.Add(mi);
        }
    }

    private static void MainWindow_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.F8)
        {
            myTools.ManageSavedContent(null);
            return;
        }

        if (e.Key == Key.F9)
        {
            myTools.SaveCurrentContent(null);
        }

        /*if (sender != null && e.OriginalSource != null && e.OriginalSource is WebBrowser &&
            sender.GetType().FullName == "Microsoft.VisualStudio.Help.Viewer.Views.ViewerWindow" )
        {
            var _onKeyDown = e.OriginalSource.GetType().GetMethod("OnKeyDown", BindingFlags.NonPublic | BindingFlags.Instance);
            if (_onKeyDown != null)
            {
                _onKeyDown.Invoke(e.OriginalSource, new object[] { e });
            }

            try
            {
                var src = PresentationSource.FromVisual((WebBrowser)e.OriginalSource);
                var ke = new KeyEventArgs(e.KeyboardDevice, src, e.Timestamp, e.Key);
                ke.RoutedEvent = UIElement.KeyDownEvent;
                ke.Source = e.OriginalSource;
                ((WebBrowser)e.OriginalSource).RaiseEvent(ke);
            }
            catch (Exception ex)
            {
                LoaderImpl.ShowException(ex);
            }

            e.Handled = true;
        }*/
    }

    internal static readonly string DAT_FILE = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "hlpvw.dat");

    public static string SettingVersion { get; private set; } = "2019.1022.0.0";
    public static string SaveLocation { get; set; }
    public static bool NoFooter { get; set; }
    public static bool NoSeeAlso { get; set; }
    public static bool NoRelated { get; set; }
    public static bool AutoBeautify { get; set; } = true;


    public static void LoadSettings()
    {
        if (System.IO.File.Exists(DAT_FILE))
        {
            using (var dat = System.IO.File.OpenText(DAT_FILE))
            {
                try
                {
                    SettingVersion = dat.ReadLine();
                    SaveLocation = dat.ReadLine();
                    NoFooter = bool.Parse(dat.ReadLine());
                    NoSeeAlso = bool.Parse(dat.ReadLine());
                    NoRelated = bool.Parse(dat.ReadLine());
                    AutoBeautify = bool.Parse(dat.ReadLine());
                }
                catch { }
            }
        }
    }
    public static void SaveSettings()
    {
        using (var dat = System.IO.File.CreateText(DAT_FILE))
        {
            dat.WriteLine(SettingVersion);
            dat.WriteLine(SaveLocation);
            dat.WriteLine(NoFooter);
            dat.WriteLine(NoSeeAlso);
            dat.WriteLine(NoRelated);
            dat.WriteLine(AutoBeautify);

            dat.Flush();
            dat.Close();
        }
    }
    public static string SelectDirectory(out bool usedef, string defPath = null)
    {
        usedef = true;
        var ofd = new OpenFileDialog();
        ofd.Filter = "*.*|*.*";
        ofd.CheckFileExists = false;
        ofd.CheckPathExists = true;
        if (ofd.ShowDialog() == true)
        {
            usedef = false;
            return System.IO.Path.GetDirectoryName(ofd.FileName);
        }
        return defPath;
    }

    public static void ShowException(Exception ex)
    {
        MessageBox.Show(ex.ToString(), "EXCEPTION", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    class MaskWin : Window
    {
        private MaskWin()
        {
            Closing += MaskWin_Closing;

            this.ShowInTaskbar = false;
            this.ResizeMode = ResizeMode.NoResize;
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            this.Topmost = true;

            this.Title = "LOADING ...";
        }

        bool cancel = true;
        private void MaskWin_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = this.cancel;
        }

        public static object Mask(Window owner, object content, System.Action action)
        {
            var win = new MaskWin();
            if (owner != null) win.Owner = owner;

            win.cancel = true;
            win.Content = content;
            if (action != null) win.Loaded += (aa, bb) => action();
            win.Show();
            return win;
        }

        public static void Unmask(Window mask)
        {
            var win = (MaskWin)mask;
            win.cancel = false;
            win.Close();
        }
    }

    static MaskWin _maskwin;
    public static void Mask(Window owner, object content, System.Action action)
    {
        if (_maskwin != null) Unmask();

        if (content == null)
        {
            content = new Viewbox
            {
                Width = 400,
                Height = 200,
                Stretch = Stretch.Uniform,
                Child = new Border()
                { 
                    Background = Brushes.White,
                    Child = new TextBlock()
                    {
                        VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center,
                        Text = "Processing, please wait ...", Foreground = Brushes.DodgerBlue, FontSize = 18
                    }
                }
            };
        }
        _maskwin = (MaskWin)MaskWin.Mask(owner, content, action);
    }

    public static void Unmask()
    {
        if (_maskwin == null) return;

        MaskWin.Unmask(_maskwin);
        _maskwin = null;
    }

}
