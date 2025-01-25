using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
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
    /// SelectPathPage.xaml 的交互逻辑
    /// </summary>
    public partial class SelectPathPage : Page
    {
        public SelectPathPage()
        {
            InitializeComponent();
        }

        [GeneratedRegex(@"[\u4e00-\u9fa5]+")]
        private static partial Regex CheckChinese();

        private void Close_Click(object sender, RoutedEventArgs e) => App.Current.Shutdown();

        private void FilePath_TextChanged(object sender, TextChangedEventArgs e)
        {
            string path = ((TextBox)sender).Text;
            if (!File.Exists(path))
            {
                if (!string.IsNullOrEmpty(path))
                {
                    ErrorMessage.Content = "错误：文件不存在！";
                }
                else
                {
                    ErrorMessage.Content = "";
                }
                NextStep.IsEnabled = false;
                return;
            }
            if (!path.Contains("\\PlantsVsZombiesRH.exe"))
            {
                ErrorMessage.Content = "错误：融合版游戏本体名称错误！";
                NextStep.IsEnabled = false;
                return;
            }
            string folder = path.Replace("\\PlantsVsZombiesRH.exe", "");
            if (!File.Exists(folder + "\\GameAssembly.dll"))
            {
                ErrorMessage.Content = "错误：游戏本体不完整！";
                NextStep.IsEnabled = false;
                return;
            }
            SHA256 sha256 = SHA256.Create();
            var stream = new FileStream(folder + "\\GameAssembly.dll", FileMode.Open, FileAccess.Read);
            byte[] hashByte = sha256.ComputeHash(stream);
            stream.Close();
            var hashCode = BitConverter.ToString(hashByte).Replace("-", "");
            if (CheckChinese().IsMatch(path))
            {
                NextStep.IsEnabled = false;
                ErrorMessage.Content = "错误：运行目录含有中文！";
            }
            else if (Directory.Exists(folder + "\\MelonLoader") || Directory.Exists(folder + "\\BepInEx"))
            {
                NextStep.IsEnabled = false;
                ErrorMessage.Content = "错误：不要选取安装过修改器/BepInEx/MelonLoader的游戏！";
            }
            else if (hashCode != "90CA592E94E8528D200C8ABDB803A1B58CDE0CF02C985568B75885C036BF664E")
            {
                NextStep.IsEnabled = false;
                ErrorMessage.Content = "错误：游戏版本错误！请使用融2.2版本的游戏！";
            }
            else
            {
                NextStep.IsEnabled = true;
                ErrorMessage.Content = "";
            }
        }

        private void NextStep_Click(object sender, RoutedEventArgs e)
        {
            CheckInstallation.InstallationPath = FilePath.Text;
            NavigationService.Navigate(new CheckInstallation());
        }

        private void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "融合版游戏本体|PlantsVsZombiesRH.exe"
            };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                FilePath.Text = openFileDialog.FileName;
            }
        }
    }
}