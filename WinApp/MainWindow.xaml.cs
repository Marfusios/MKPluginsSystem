using System.Windows;
using WinApp.ViewModels;

namespace WinApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            _mainPanel.Content = new MainPanelViewModel();
        }
    }
}
