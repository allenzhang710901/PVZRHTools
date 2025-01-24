using Microsoft.Win32;
using System.Configuration;
using System.Data;
using System.Windows;

namespace PVZRHToolsInstaller
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (Environment.OSVersion.Version.Major < 10)
            {
                MessageBox.Show("抱歉，系统版本过低，至少需要win10才能运行此程序。\n不要用此弹窗问修改器作者，看到也不会回复。");
            }
        }
    }
}