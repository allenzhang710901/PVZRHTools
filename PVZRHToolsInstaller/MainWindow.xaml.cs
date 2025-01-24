using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PVZRHToolsInstaller.Pages;

namespace PVZRHToolsInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CurrentPage.Navigate(new StartPage());
        }

        private void OnCurrentPageNavigated(object sender, NavigationEventArgs e)
        {
        }

        public static bool Lock { get; set; } = false;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = Lock;
        }
    }
}