using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Windows;
using System;
using System.Windows.Threading;
using ToolModData;
using CommunityToolkit.Mvvm.DependencyInjection;
using System.IO;

namespace PVZRHTools
{
    public class DataSync
    {
        public DataSync()
        {
            buffer = new byte[1024 * 64];
            modifierSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            modifierSocket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 13531));
            modifierSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new(Receive), modifierSocket);
        }

        ~DataSync()
        {
            if (!modifierSocket.Poll(100, SelectMode.SelectRead))
            {
                modifierSocket.Shutdown(SocketShutdown.Both);
                modifierSocket.Close();
            }
        }

        public void Receive(IAsyncResult ar)
        {
            Socket? socket = ar.AsyncState as Socket;
            if (socket is not null)
            {
                int bytes = socket.EndReceive(ar);
                ar.AsyncWaitHandle.Close();
                ProcessData(Encoding.UTF8.GetString(buffer, 0, bytes));
                buffer = new byte[1024 * 64];
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new(Receive), socket);
            }
        }

        public void ProcessData(string data)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data) || string.IsNullOrEmpty(data)) return;
                JsonObject json = JsonNode.Parse(data)!.AsObject();
                switch ((int)json["ID"]!)
                {
                    case 4:
                        {
                            SyncTravelBuff s = JsonSerializer.Deserialize<SyncTravelBuff>(json);
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                if (s.AdvInGame is not null && s.UltiInGame is not null)
                                {
                                    //垃圾Ioc.Default.GetService<ModifierViewModel>()把词条修改的进度卡了整整两周
                                    //把ViewModel基本改了个遍，结果是这里把数据sync到****里去了
                                    for (int i = 0; i < s.AdvInGame.Count; i++)
                                    {
                                        MainWindow.Instance!.ViewModel.InGameBuffs[i].Enabled = s.AdvInGame[i];
                                    }
                                    for (int i = 0; i < s.UltiInGame.Count; i++)
                                    {
                                        MainWindow.Instance!.ViewModel.InGameBuffs[i + s.AdvInGame.Count].Enabled = s.UltiInGame[i];
                                    }
                                }
                            });
                            break;
                        }
                    case 6:
                        {
                            InGameActions iga = JsonSerializer.Deserialize<InGameActions>(json);
                            if (iga.WriteField is not null)
                            {
                                Application.Current.Dispatcher.Invoke(() => Clipboard.SetDataObject(iga.WriteField));
                            }
                            if (iga.WriteZombies is not null)
                            {
                                Application.Current.Dispatcher.Invoke(() => Clipboard.SetDataObject(iga.WriteZombies));
                            }
                            break;
                        }
                    case 16:
                        {
                            Application.Current.Dispatcher.Invoke(new Action(delegate { Application.Current.Shutdown(); }));
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText("./ModifierError.txt", ex.Message + ex.StackTrace);
                MessageBox.Show(ex.ToString());
                Application.Current.Dispatcher.Invoke(new Action(delegate { Application.Current.Shutdown(); }));
            }
        }

        public void SendData<T>(T data)
        {
            modifierSocket.Send(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data)));
            Thread.Sleep(5);
        }

        public static Lazy<DataSync> Instance { get; } = new();
        public byte[] buffer;
        public Socket modifierSocket;
    }
}