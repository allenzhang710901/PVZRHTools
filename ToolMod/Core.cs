using Il2Cpp;
using MelonLoader;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using ToolModData;
using UnityEngine;
using static ToolMod.PatchMgr;

namespace ToolMod
{
    public class Core : MelonMod
    {
        public override void OnApplicationQuit()
        {
            if (inited)
            {
                if (GameAPP.gameSpeed == 0) GameAPP.gameSpeed = 1;
                DataSync.Instance.Value.SendData(new Exit());
                Thread.Sleep(100);
                DataSync.Instance.Value.modifierSocket.Shutdown(SocketShutdown.Both);
                DataSync.Instance.Value.modifierSocket.Close();
            }
            inited = false;
        }

        public override void OnInitializeMelon()
        {
            Instance = new(this);
            if (Time.timeScale == 0) Time.timeScale = 1;
            SyncSpeed = Time.timeScale;
            Config = new(MelonPreferences.CreateCategory("PVZRHTools"));
            Port = new(Config.Value.CreateEntry("Port", 13531));
            AlmanacZombieMindCtrl = new(Config.Value.CreateEntry(nameof(AlmanacZombieMindCtrl), false));
            KeyTimeStop = new(Config.Value.CreateEntry(nameof(KeyTimeStop), KeyCode.Alpha5));
            KeyShowGameInfo = new(Config.Value.CreateEntry(nameof(KeyShowGameInfo), KeyCode.BackQuote));
            KeyAlmanacCreatePlant = new(Config.Value.CreateEntry(nameof(KeyAlmanacCreatePlant), KeyCode.B));
            KeyAlmanacCreateZombie = new(Config.Value.CreateEntry(nameof(KeyAlmanacCreateZombie), KeyCode.N));
            KeyAlmanacZombieMindCtrl = new(Config.Value.CreateEntry(nameof(KeyAlmanacZombieMindCtrl), KeyCode.LeftControl));
            KeyTopMostCardBank = new(Config.Value.CreateEntry(nameof(KeyTopMostCardBank), KeyCode.Tab));
            KeyBindings = new
            ([
                KeyTimeStop.Value,KeyTopMostCardBank.Value,KeyShowGameInfo.Value,
                KeyAlmanacCreatePlant.Value,KeyAlmanacCreateZombie.Value,KeyAlmanacZombieMindCtrl.Value,
            ]);
            Port.Value.Description = "修改窗口无法出现时可尝试修改此数值，范围10000~60000";
            Config.Value.SaveToFile();
            inited = true;
        }

        public override void OnLateInitializeMelon()
        {
            try
            {
                if (Port.Value.Value < 10000 || Port.Value.Value > 60000)
                {
                    MessageBox(0, "Port值无效，已使用默认值13531", "修改器警告", 0);
                    Port.Value.Value = 13531;
                }
                _ = DataSync.Instance.Value;
                Task.Run(() =>
                {
                    Thread.Sleep(5000);
                });
            }
            catch (Exception ex)
            {
                LoggerInstance.Error(ex);
            }
        }

        public override void OnUpdate()
        {
            Update();
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr MessageBox(int hWnd, string text, string caption, uint type);

        public static Lazy<MelonPreferences_Entry<bool>> AlmanacZombieMindCtrl { get; set; } = new();
        public static Lazy<MelonPreferences_Category> Config { get; set; } = new();
        public static Lazy<Core> Instance { get; set; } = new();
        public static Lazy<MelonPreferences_Entry<KeyCode>> KeyAlmanacCreatePlant { get; set; } = new();
        public static Lazy<MelonPreferences_Entry<KeyCode>> KeyAlmanacCreateZombie { get; set; } = new();
        public static Lazy<MelonPreferences_Entry<KeyCode>> KeyAlmanacZombieMindCtrl { get; set; } = new();
        public static Lazy<List<MelonPreferences_Entry<KeyCode>>> KeyBindings { get; set; } = new();
        public static Lazy<MelonPreferences_Entry<KeyCode>> KeyShowGameInfo { get; set; } = new();
        public static Lazy<MelonPreferences_Entry<KeyCode>> KeyTimeStop { get; set; } = new();
        public static Lazy<MelonPreferences_Entry<KeyCode>> KeyTopMostCardBank { get; set; } = new();
        public static Lazy<MelonPreferences_Entry<int>> Port { get; set; } = new();
        public static bool inited = false;
    }
}