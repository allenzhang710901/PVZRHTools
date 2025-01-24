using SevenZipExtractor;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
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
    /// CheckInstallation.xaml 的交互逻辑
    /// </summary>
    public partial class CheckInstallation : Page
    {
        public CheckInstallation()
        {
            InitializeComponent();
            CheckPath.Content = InstallationPath;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void LastStep_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void NextStep_Click(object sender, RoutedEventArgs e)
        {
            InfoText.Content = "解压中，可能会卡顿...";
            MainWindow.Lock = true;
            LastStep.IsEnabled = false;
            NextStep.IsEnabled = false;
            Close.IsEnabled = false;
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PVZRHToolsInstaller.PVZRHTools.7z")!)
            {
                new ArchiveFile(stream).Extract(InstallationPath.Replace("\\PlantsVsZombiesRH.exe", ""), true);
            }
            InfoText.Content = "解压完成，感谢您对本项目的支持！";
            MainWindow.Lock = false;
            Close.IsEnabled = true;
        }

        public static string InstallationPath { get; set; } = "";
    }
}