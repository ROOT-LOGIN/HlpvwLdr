using HlpvwFake.Proxies;
using Microsoft.Win32;
using mshtml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Windows.Threading;

namespace HlpvwFake
{
    /// <summary>
    /// Interaction logic for BuildUI.xaml
    /// </summary>
    public partial class BuildUI : Window
    {
        public BuildUI()
        {
            InitializeComponent();

            browser.Navigated += Browser_Navigated;
            browser.LoadCompleted += Browser_LoadCompleted;

            filelstbox.MouseDoubleClick += Filelstbox_MouseDoubleClick;

            Title = "Build Merged Document";
            Loaded += ManageUI_Loaded;
        }

        private void ManageUI_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                sAlso.IsChecked = !LoaderImpl.NoSeeAlso;
                sFooter.IsChecked = !LoaderImpl.NoFooter;
                sRelated.IsChecked = !LoaderImpl.NoRelated;
                pAuto.IsChecked = LoaderImpl.AutoBeautify;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        internal IList<FileModel> FileList
        {
            get { return (IList<FileModel>)filelstbox.ItemsSource; }
            set
            {
                if (value == null) filelstbox.ItemsSource = null;
                else if (value is ObservableCollection<FileModel>) filelstbox.ItemsSource = value;
                else filelstbox.ItemsSource = new ObservableCollection<FileModel>(value);
            }
        }

        Stream m_stream = null;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == build1 || sender == build2)
            {
                var filelst = FileList;
                if (filelst == null) return;
                IList<FileModel> selist = null;
                if (sender == build1)
                {
                    selist = filelst.Where(f => f.ISCHECKED).ToList();
                    if (selist.Count == 0)
                    {
                        if (MessageBox.Show($"No file is checked, process all ({filelst.Count} Items)?", "CONFIRM", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                            return;

                        selist = filelst;
                    }
                }
                else if(filelstbox.SelectedItem != null)
                {
                    selist = new List<FileModel>() { (FileModel)filelstbox.SelectedItem };
                }

                if (selist == null || selist.Count == 0)
                {
                    browser.Navigate("about:blank");
                }
                else
                {
                    LoaderImpl.Mask(this, null, () =>
                    {
                        var timer = new System.Timers.Timer();
                        timer.Interval = 600;
                        timer.Elapsed += (aa, bb) =>
                        {
                            timer.Stop();
                            Stream stream = null;
                            try
                            {
                                stream = File.Create("$__tmp.mhtml");
                                foreach (var fm in selist)
                                {
                                    var bytes = System.IO.File.ReadAllBytes(fm.PATH);
                                    stream.Write(bytes, 0, bytes.Length);
                                }
                                stream.Flush();

                                stream.Seek(0, SeekOrigin.Begin);
                                Dispatcher.Invoke(() => browser.NavigateToStream(stream));
                                m_stream = stream;
                            }
                            catch
                            {
                                LoaderImpl.Unmask();
                                if (stream != null) stream.Close();
                            }

                        };
                        timer.Start();
                        //System.Windows.Application.Current.Dispatcher.BeginInvoke(new System.Action(), DispatcherPriority.ContextIdle);
                    });                    
                }
                return;
            }

            if(sender == beautify)
            {
                Browser_LoadCompleted(browser, null);
                return;
            }

            if (sender == exit)
            {
                LoaderImpl.NoSeeAlso = !sAlso.IsChecked.GetValueOrDefault();
                LoaderImpl.NoFooter = !sFooter.IsChecked.GetValueOrDefault();
                LoaderImpl.NoRelated = !sRelated.IsChecked.GetValueOrDefault();
                LoaderImpl.AutoBeautify = pAuto.IsChecked.GetValueOrDefault();

                LoaderImpl.SaveSettings();

                this.Close();
                return;
            }

            if(sender == top)
            {
                var sel = (FileModel)filelstbox.SelectedItem;
                if (sel == null) return;
                FileList.Remove(sel);
                FileList.Insert(0, sel);
                filelstbox.SelectedItem = sel;
                filelstbox.ScrollIntoView(sel);            
                return;
            }

            if (sender == btm)
            {
                var sel = (FileModel)filelstbox.SelectedItem;
                if (sel == null) return;
                FileList.Remove(sel);
                FileList.Add(sel);
                filelstbox.SelectedItem = sel;
                filelstbox.ScrollIntoView(sel);
                return;
            }

            if (sender == up1)
            {
                var sel = (FileModel)filelstbox.SelectedItem;
                var seli = filelstbox.SelectedIndex;
                if (sel == null) return;
                FileList.RemoveAt(seli);
                FileList.Insert(Math.Max(0, seli - 1), sel);
                filelstbox.SelectedItem = sel;
                filelstbox.ScrollIntoView(sel);
                return;
            }

            if (sender == dw1)
            {
                var sel = (FileModel)filelstbox.SelectedItem;
                var seli = filelstbox.SelectedIndex;
                if (sel == null) return;
                FileList.RemoveAt(seli);
                FileList.Insert(Math.Min(FileList.Count, seli + 1), sel);
                filelstbox.SelectedItem = sel;
                filelstbox.ScrollIntoView(sel);
                return;
            }

            if (sender == sav)
            {
                if (FileList == null) return;

                if (MessageBox.Show("Do you want to save this order?", "CONFIRM", MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK)
                    return;

                var sel = (FileModel)filelstbox.SelectedItem;
                int i = 0;
                foreach(var f in FileList)
                {
                    var of = f.PATH;
                    i++;
                    int k = f.FILE.IndexOf('#');
                    f.FILE = i.ToString("0000") + "#" + f.FILE.Substring(k + 1);
                    f.PATH = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(f.PATH), f.FILE);
                    System.IO.File.Move(of, f.PATH);
                }
                var lst = FileList;
                FileList = null;
                FileList = lst;
                filelstbox.SelectedItem = sel;
                return;
            }
        }

        private void Browser_Navigated(object sender, NavigationEventArgs e)
        {
            if (m_stream != null)
            {
                m_stream.Close();
                m_stream = null;
            }
        }

        private void Browser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            LoaderImpl.Mask(this, null, () =>
            {
                var timer = new System.Timers.Timer();
                timer.Interval = 600;
                timer.Elapsed += (aa, bb) =>
                {
                    timer.Stop();
                    Dispatcher.Invoke(() => Real_Browser_LoadCompleted(sender, e));
                    
                };
                timer.Start();
                //System.Windows.Application.Current.Dispatcher.BeginInvoke(new System.Action(), DispatcherPriority.ContextIdle);
            });
        }

        private void Real_Browser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            try
            {
                // remove logo,copyright,annoy note
                dynamic doc = browser.Document;
                for ( ; ; )
                {
                    dynamic item = doc.getElementsByClassName("OH_feedbacklink");
                    for (int i = 0; i < item.length; i++)
                    {
                        var _this = item[i];
                        var pr = _this.parentNode;
                        if (pr == null) continue;
                        pr.removeChild(_this);
                    }
                    if (item.length == 0) break;
                }
                for ( ; ; )
                {
                    dynamic item = doc.getElementsByClassName("OH_disclaimersSection");
                    for (int i = 0; i < item.length; i++)
                    {
                        var _this = item[i];
                        var pr = _this.parentNode;
                        if (pr == null) continue;
                        pr.removeChild(_this);
                    }
                    if (item.length == 0) break;
                }
                for (; ; )
                {
                    dynamic item = doc.getElementsByClassName("OH_CodeSnippetToolBarText");
                    if (item.length == 0) break;

                    bool bk = true;
                    for (int i = 0; i < item.length; i++)
                    {
                        var _this = item[i];
                        if(_this.style.display != "none")
                        {
                            bk = false;
                            _this.style.display = "none";
                        }
                        /*var pr = _this.parentNode;
                        if (pr == null) continue;
                        pr.removeChild(_this);*/
                    }
                    if (bk) break;
                }

                if (e != null && !pAuto.IsChecked.GetValueOrDefault())
                {
                    LoaderImpl.Unmask();
                    return;
                }

                for (; !sFooter.IsChecked.GetValueOrDefault();)
                {
                    dynamic item = doc.getElementsByClassName("OH_footer");
                    for (int i = 0; i < item.length; i++)
                    {
                        var _this = item[i];
                        var pr = _this.parentNode;
                        if (pr == null) continue;
                        pr.removeChild(_this);
                    }
                    if (item.length == 0) break;
                }
                for ( ; !sAlso.IsChecked.GetValueOrDefault() ;)
                {
                    dynamic item = doc.getElementById("see_also");
                    if (item == null) break;
                    var pr = item.parentNode;
                    if(pr != null)
                    {
                        var sb = pr.nextSibling;
                        if (sb != null && sb.tagName.ToLower() == "dl")
                        {
                            if (sb.parentNode != null) sb.parentNode.removeChild(sb);
                        }

                        if (pr.tagName.ToLower().StartsWith("h"))
                        {
                            if (pr.parentNode != null) pr.parentNode.removeChild(pr);
                        }
                    }
                }
                for ( ; !sRelated.IsChecked.GetValueOrDefault() ;)
                {
                    dynamic item = doc.getElementById("related_topics");
                    if (item == null) break;
                    var pr = item.parentNode;
                    if (pr != null)
                    {
                        var sb = pr.nextSibling;
                        if (sb != null && sb.tagName.ToLower() == "dl")
                        {
                            if (sb.parentNode != null) sb.parentNode.removeChild(sb);
                        }

                        if (pr.tagName.ToLower().StartsWith("h"))
                        {
                            if (pr.parentNode != null) pr.parentNode.removeChild(pr);
                        }
                    }
                }
                LoaderImpl.Unmask();            
            }
            catch (Exception ex)
            {
                LoaderImpl.Unmask();
                LoaderImpl.ShowException(ex);
            }
        }


        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var chk = sender as CheckBox;
            if (chk == null) return;

            if(FileList != null && chk == this.chk)
            {
                foreach(var item in FileList)
                {
                    item.ISCHECKED = chk.IsChecked.GetValueOrDefault();
                }
            }

            if (FileList != null)
            {                
                chktip.Text = $"{(FileList.Count(d => d.ISCHECKED))} of {FileList.Count} Items Checked";
            }
        }

        private void Filelstbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Button_Click(build2, e);
        }
    }

}
