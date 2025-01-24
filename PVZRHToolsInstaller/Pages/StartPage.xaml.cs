using Microsoft.Win32;
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

namespace PVZRHToolsInstaller.Pages
{
    /// <summary>
    /// Start.xaml 的交互逻辑
    /// </summary>
    public partial class StartPage : Page
    {
        public StartPage()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void NextStep_Click(object sender, RoutedEventArgs e)
        {
            var r = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\VisualStudio\\14.0\\VC\\Runtimes\\X64", "Installed", null);
            if (r is null || (int)r != 1)
            {
                NeedLibraryPage.NeedVCRedist = true;
            }
            Process pro = new();
            pro.StartInfo.FileName = "cmd.exe";
            pro.StartInfo.UseShellExecute = false;
            pro.StartInfo.RedirectStandardError = true;
            pro.StartInfo.RedirectStandardInput = true;
            pro.StartInfo.RedirectStandardOutput = true;
            pro.StartInfo.CreateNoWindow = true;
            pro.Start();
            pro.StandardInput.WriteLine("dotnet --list-runtimes");
            pro.StandardInput.WriteLine("exit");
            pro.StandardInput.AutoFlush = true;
            string output = pro.StandardOutput.ReadToEnd();
            pro.WaitForExit();
            pro.Close();
            NeedLibraryPage.NeedDotnet = !output.Contains("Microsoft.NETCore.App 6.0");
            if (NeedLibraryPage.NeedVCRedist || NeedLibraryPage.NeedDotnet)
            {
                NavigationService.Navigate(new NeedLibraryPage());
            }
            else
            {
                NavigationService.Navigate(new SelectPathPage());
            }
        }
    }
}