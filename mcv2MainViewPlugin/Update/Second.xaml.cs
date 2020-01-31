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

namespace mcv2MainViewPlugin.Update
{
    /// <summary>
    /// Interaction logic for Second.xaml
    /// </summary>
    public partial class Second : UserControl
    {
        public Second()
        {
            InitializeComponent();
        }
        public string CurrentVersion
        {
            get { return (string)GetValue(CurrentVersionProperty); }
            set { SetValue(CurrentVersionProperty, value); }
        }

        public static readonly DependencyProperty CurrentVersionProperty =
            DependencyProperty.Register(nameof(CurrentVersion), typeof(string), typeof(Second), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string LatestVersion
        {
            get { return (string)GetValue(LatestVersionProperty); }
            set { SetValue(LatestVersionProperty, value); }
        }

        public static readonly DependencyProperty LatestVersionProperty =
            DependencyProperty.Register(nameof(LatestVersion), typeof(string), typeof(Second), new PropertyMetadata(""));

    }
}
