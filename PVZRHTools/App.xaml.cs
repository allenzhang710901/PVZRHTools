using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;
using ToolModData;

namespace PVZRHTools
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            DataSync = new();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                if (e.Args[0] == "PVZRHTools")
                {
                    _ = DataSync.Value;

                    InitData = JsonSerializer.Deserialize<InitData>(File.ReadAllText("./PVZRHTools/InitData.json"));
                    /*
                    Ioc.Default.ConfigureServices(new ServiceCollection()
                        .AddTransient<ModifierViewModel>()
                        .BuildServiceProvider());
                    */
                    inited = true;
                }
                else
                {
                    Shutdown();
                }
            }
            catch (IndexOutOfRangeException)
            {
                MessageBox.Show("请直接启动游戏本体，修改窗口不允许单独启动。");
                Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (inited)
            {
                try
                {
                    DataSync.Value.SendData(new Exit());
                    Thread.Sleep(100);
                    DataSync.Value.modifierSocket.Shutdown(SocketShutdown.Both);
                    DataSync.Value.modifierSocket.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    File.WriteAllText("./ModifierError.txt", ex.Message + ex.StackTrace);
                }
            }
            inited = false;
        }

        public static Lazy<DataSync> DataSync { get; set; }
        public bool inited = false;
        public static InitData? InitData { get; set; }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message + e.Exception.StackTrace);
            File.WriteAllText("./ModifierError.txt", e.Exception.Message + e.Exception.StackTrace);
            e.Handled = true;
            Shutdown();
        }
    }
}