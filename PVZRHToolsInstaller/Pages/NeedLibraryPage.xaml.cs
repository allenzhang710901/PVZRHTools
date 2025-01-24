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

namespace PVZRHToolsInstaller.Pages
{
    /// <summary>
    /// NeedLibraryPage.xaml 的交互逻辑
    /// </summary>
    public partial class NeedLibraryPage : Page
    {
        public NeedLibraryPage()
        {
            InitializeComponent();
            ErrorMessage.Content = "您的系统中缺少：" + (NeedDotnet ? "dotnet6 " : "") + (NeedDotnet && NeedVCRedist ? "和" : "") + (NeedVCRedist ? "vc_redist" : "");
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        public static bool NeedDotnet { get; set; } = false;
        public static bool NeedVCRedist { get; set; } = false;
    }
}