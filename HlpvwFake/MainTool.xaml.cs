using HlpvwFake.Proxies;
using mshtml;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for MainTool.xaml
    /// </summary>
    public partial class MainTool : UserControl
    {
        public MainTool()
        {
            InitializeComponent();

            _impl = new CoreImpl();
        }

        CoreImpl _impl;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == save)
            {
                SaveCurrentContent(directSave.IsChecked.GetValueOrDefault());
                return;
            }

            if (sender == manage)
            {
                ManageSavedContent(null);
                return;
            }
        }

        public void SaveCurrentContent(bool? isDirect)
        {
            if (isDirect == null) isDirect = directSave.IsChecked.GetValueOrDefault();

            var ui = new SaveUI() { Owner = Application.Current.MainWindow, ShowInTaskbar = false, WindowStartupLocation = WindowStartupLocation.CenterScreen };
            if ((bool)isDirect)
            {
                if (ui.SaveDirect()) return;
            }
            ui.ShowDialog();
        }

        public void ManageSavedContent(string path)
        {
            var ui = new ManageUI() { Owner = Application.Current.MainWindow, ShowInTaskbar = false, WindowStartupLocation = WindowStartupLocation.CenterScreen };
            ui.ShowDialog();
        }
    }
}


// https://blog.csdn.net/haibindev/article/details/84061641
// browserEvent_NewWindow
