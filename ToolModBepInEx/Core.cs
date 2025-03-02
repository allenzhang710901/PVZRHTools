using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using ToolModData;
using UnityEngine;
using static ToolModBepInEx.PatchMgr;

namespace ToolModBepInEx
{
    [BepInPlugin("inf75.toolmod", "ToolMod", "3.12")]
    public class Core : BasePlugin
    {
        public void CustomPlantPlayground()
        {
        }

        public override void Load()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            ClassInjector.RegisterTypeInIl2Cpp<PatchMgr>();
            ClassInjector.RegisterTypeInIl2Cpp<DataProcessor>();
            ClassInjector.RegisterTypeInIl2Cpp<CardUIReplacer>();
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            Instance = new(this);
            if (Time.timeScale == 0) Time.timeScale = 1;
            SyncSpeed = Time.timeScale;
            Port = new(Config.Bind("PVZRHTools", "Port", 13531, "修改窗口无法出现时可尝试修改此数值，范围10000~60000"));
            AlmanacZombieMindCtrl = new(Config.Bind("PVZRHTools", nameof(AlmanacZombieMindCtrl), false));
            KeyTimeStop = new(Config.Bind("PVZRHTools", nameof(KeyTimeStop), KeyCode.Alpha5));
            KeyShowGameInfo = new(Config.Bind("PVZRHTools", nameof(KeyShowGameInfo), KeyCode.BackQuote));
            KeyAlmanacCreatePlant = new(Config.Bind("PVZRHTools", nameof(KeyAlmanacCreatePlant), KeyCode.B));
            KeyAlmanacCreateZombie = new(Config.Bind("PVZRHTools", nameof(KeyAlmanacCreateZombie), KeyCode.N));
            KeyAlmanacZombieMindCtrl = new(Config.Bind("PVZRHTools", nameof(KeyAlmanacZombieMindCtrl), KeyCode.LeftControl));
            KeyTopMostCardBank = new(Config.Bind("PVZRHTools", nameof(KeyTopMostCardBank), KeyCode.Tab));
            KeyBindings = new
            ([
                KeyTimeStop.Value,KeyTopMostCardBank.Value,KeyShowGameInfo.Value,
                KeyAlmanacCreatePlant.Value,KeyAlmanacCreateZombie.Value,KeyAlmanacZombieMindCtrl.Value,
            ]);
            Config.Save();
            inited = true;
            try
            {
                if (Port.Value.Value < 10000 || Port.Value.Value > 60000)
                {
                    MessageBox(0, "Port值无效，已使用默认值13531", "修改器警告", 0);
                    Port.Value.Value = 13531;
                }
                _ = DataSync.Instance.Value;
            }
            catch (Exception ex)
            {
                LoggerInstance.Error(ex);
            }
            CustomPlantPlayground();
        }

        public override bool Unload()
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
            return true;
        }

        public void Update()
        {
            //PatchMgr.Update();
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr MessageBox(int hWnd, string text, string caption, uint type);

        public static Lazy<ConfigEntry<bool>> AlmanacZombieMindCtrl { get; set; } = new();
        public static Lazy<Core> Instance { get; set; } = new();
        public static Lazy<ConfigEntry<KeyCode>> KeyAlmanacCreatePlant { get; set; } = new();
        public static Lazy<ConfigEntry<KeyCode>> KeyAlmanacCreateZombie { get; set; } = new();
        public static Lazy<ConfigEntry<KeyCode>> KeyAlmanacZombieMindCtrl { get; set; } = new();
        public static Lazy<List<ConfigEntry<KeyCode>>> KeyBindings { get; set; } = new();
        public static Lazy<ConfigEntry<KeyCode>> KeyShowGameInfo { get; set; } = new();
        public static Lazy<ConfigEntry<KeyCode>> KeyTimeStop { get; set; } = new();
        public static Lazy<ConfigEntry<KeyCode>> KeyTopMostCardBank { get; set; } = new();
        public static Lazy<ConfigEntry<int>> Port { get; set; } = new();
        public ManualLogSource LoggerInstance => BepInEx.Logging.Logger.CreateLogSource("ToolMod");
        public static bool inited = false;
    }
}