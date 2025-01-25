using System.Diagnostics.Metrics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using System.Windows;
using System.Windows.Threading;
using ToolModData;

namespace PVZRHTools
{
    public class DataSync
    {
        public DataSync(int port)
        {
            buffer = new byte[1024 * 64];
            modifierSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            modifierSocket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), port));
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
                            SyncTravelBuff s = JsonSerializer.Deserialize(json, SyncTravelBuffSGC.Default.SyncTravelBuff);
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                if (s.AdvInGame is not null && s.UltiInGame is not null)
                                {
                                    Enabled = false;
                                    for (int i = 0; i < s.AdvInGame.Count; i++)
                                    {
                                        MainWindow.Instance!.ViewModel.InGameBuffs[i].Enabled = s.AdvInGame[i];
                                    }
                                    for (int i = 0; i < s.UltiInGame.Count; i++)
                                    {
                                        MainWindow.Instance!.ViewModel.InGameBuffs[i + s.AdvInGame.Count].Enabled = s.UltiInGame[i];
                                    }
                                    Enabled = true;
                                }
                                if (s.DebuffsInGame is not null)
                                {
                                    Enabled = false;
                                    for (int i = 0; i < s.DebuffsInGame.Count; i++)
                                    {
                                        MainWindow.Instance!.ViewModel.InGameDebuffs[i].Enabled = s.DebuffsInGame[i];
                                    }

                                    Enabled = true;
                                }
                            });
                            break;
                        }
                    case 6:
                        {
                            InGameActions iga = JsonSerializer.Deserialize(json, InGameActionsSGC.Default.InGameActions);
                            if (iga.WriteField is not null)
                            {
                                Application.Current.Dispatcher.Invoke(() => MainWindow.Instance!.ViewModel.FieldString = iga.WriteField);
                            }
                            if (iga.WriteZombies is not null)
                            {
                                Application.Current.Dispatcher.Invoke(() => MainWindow.Instance!.ViewModel.ZombieFieldString = iga.WriteZombies);
                            }
                            break;
                        }
                    case 15:
                        {
                            Application.Current.Dispatcher.Invoke(() => MainWindow.Instance!.ViewModel.SyncAll());
                            break;
                        }
                    case 16:
                        {
                            Application.Current.Dispatcher.Invoke(new Action(Application.Current.Shutdown));
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText("./ModifierError.txt", ex.Message + ex.StackTrace);
                MessageBox.Show(ex.ToString());
                Application.Current.Dispatcher.Invoke(new Action(Application.Current.Shutdown));
            }
        }

        public void Receive(IAsyncResult ar)
        {
            try
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
            catch (SocketException)
            {
                Application.Current.Shutdown();
            }
            catch (ObjectDisposedException)
            {
                Application.Current.Shutdown();
            }
        }

        public void SendData<T>(T data) where T : ISyncData
        {
            if (!App.inited) return;
            if (!Enabled) return;
            JsonTypeInfo jti = data.ID switch
            {
                6 => InGameActionsSGC.Default.InGameActions,
                2 => BasicPropertiesSGC.Default.BasicProperties,
                5 => CardPropertiesSGC.Default.CardProperties,
                16 => ExitSGC.Default.Exit,
                7 => GameModesSGC.Default.GameModes,
                15 => SyncAllSGC.Default.SyncAll,
                4 => SyncTravelBuffSGC.Default.SyncTravelBuff,
                1 => ValuePropertiesSGC.Default.ValueProperties,
                _ => throw new InvalidOperationException()
            };
            modifierSocket.Send(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data, jti)));
            Thread.Sleep(5);
        }

        public static bool Enabled { get; set; } = true;
        public static Lazy<DataSync> Instance { get; } = new();
        public byte[] buffer;
        public Socket modifierSocket;
    }
}