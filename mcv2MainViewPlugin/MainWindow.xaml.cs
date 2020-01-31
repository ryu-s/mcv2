using GalaSoft.MvvmLight.Messaging;
using mcv2MainViewPlugin;
using mcv2MainViewPlugin.Update;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace mcv2.MainViewPlugin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Messenger.Default.Register<ShowOptionsViewMessage>(this, message =>
            {
                try
                {
                    var optionsView = new OptionsView
                    {
                        Owner = this,
                        DataContext = message.OptionsViewModel
                    };
                    optionsView.ShowDialog();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });
            Messenger.Default.Register<ShowUserViewMessage>(this, message =>
            {
                try
                {
                    var uvm = message.Uvm;
                    var userView = new UserView
                    {
                        DataContext = uvm
                    };
                    userView.Show();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });
            Messenger.Default.Register<ShowUpdateViewMessage>(this, message =>
            {
                var updateView = new UpdateView
                {
                    DataContext = message.Vm,
                };
                updateView.Show();
            });
        }

    }
    public static class DataGridBehavior
    {
        public static ScrollViewer GetScrollViewer(this DataGrid dataGrid)
        {
            return dataGrid.Template.FindName("DG_ScrollViewer", dataGrid) as ScrollViewer;
        }
    }
    public static class ScrollViewerBehavior
    {
        public static bool IsBottom(this ScrollViewer sv)
        {
            //var b = (sv.VerticalOffset * 1.01) > sv.ScrollableHeight;
            var b = (sv.VerticalOffset >= sv.ScrollableHeight
                || sv.ExtentHeight < sv.ViewportHeight);
            return b;
        }
    }
}
