using System.Windows;
using GalaSoft.MvvmLight.Messaging;

namespace mcv2.MainViewPlugin
{
    /// <summary>
    /// Interaction logic for OptionsView.xaml
    /// </summary>
    public partial class OptionsView : Window
    {
        public OptionsView()
        {
            InitializeComponent();

            Messenger.Default.Register<CloseOptionsViewMessage>(this, message =>
            {
                this.Close();
            });
        }
    }
}
