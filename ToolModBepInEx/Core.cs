using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using TMPro;
using ToolModData;
using UnityEngine;
using static ToolModBepInEx.PatchMgr;

namespace ToolModBepInEx
{
    [HarmonyPatch(typeof(Help_), "Awake")]
    public static class Help_Patch
    {
        public static void Postfix() => Core.Instance.Value.LateInit();
    }

    [BepInPlugin("inf75.toolmod", "ToolMod", "3.17")]
    public class Core : BasePlugin
    {
        public void LateInit()
        {
            try
            {
                if (Port.Value.Value < 10000 || Port.Value.Value > 60000)
                {
                    MessageBox(0, "Port值无效，已使用默认值13531", "修改器警告", 0);
                    Port.Value.Value = 13531;
                }
                MLogger.LogWarning("以下id信息为动态生成，仅适用于当前游戏实例！！！");
                MLogger.LogWarning("以下id信息为动态生成，仅适用于当前游戏实例！！！");
                MLogger.LogWarning("以下id信息为动态生成，仅适用于当前游戏实例！！！");
                Dictionary<int, string> plants = [];
                Dictionary<int, string> zombies = [];
                GameObject gameObject = new();
                GameObject back = new();
                back.transform.SetParent(gameObject.transform);
                GameObject name = new("Name");
                GameObject shadow = new("Shadow");
                shadow.AddComponent<TextMeshPro>();
                shadow.transform.SetParent(name.transform);
                var nameText = name.AddComponent<TextMeshPro>();
                name.transform.SetParent(gameObject.transform);
                GameObject info = new("Info");
                info.AddComponent<TextMeshPro>();
                info.transform.SetParent(gameObject.transform);
                GameObject cost = new("Cost");
                cost.AddComponent<TextMeshPro>();
                cost.transform.SetParent(gameObject.transform);
                var alm = gameObject.AddComponent<AlmanacMgr>();
                var almz = gameObject.AddComponent<AlmanacMgrZombie>();
                gameObject.AddComponent<TravelMgr>();
                alm.plantName = name;
                for (int i = 0; i < GameAPP.plantPrefab.Count; i++)
                {
                    if (GameAPP.plantPrefab[i] is not null)
                    {
                        alm.theSeedType = i;
                        alm.InitNameAndInfoFromJson();
                        string item = $"{i} : {alm.plantName.GetComponent<TextMeshPro>().text}";
                        MLogger.Msg($"Dumping Plant String: {item}");
                        plants.Add(i, item);
                        HealthPlants.Add((PlantType)i, -1);
                        alm.plantName.GetComponent<TextMeshPro>().text = "";
                    }
                }
                for (int i = 0; i < GameAPP.zombiePrefab.Count; i++)
                {
                    if (GameAPP.zombiePrefab[i] is not null)
                    {
                        almz.theZombieType = (ZombieType)i;
                        almz.InitNameAndInfoFromJson();
                        HealthZombies.Add((ZombieType)i, -1);

                        if (!string.IsNullOrEmpty(almz.zombieName.GetComponent<TextMeshPro>().text))
                        {
                            string item = $"{i} : {almz.zombieName.GetComponent<TextMeshPro>().text}";
                            MLogger.Msg($"Dumping Zombie String: {item}");
                            zombies.Add(i, item);
                            almz.zombieName.GetComponent<TextMeshPro>().text = "";
                        }
                    }
                }
                zombies.Add(44, "44 : 僵王博士");
                zombies.Add(46, "46 : 僵王博士(二阶段)");
                MLogger.Msg($"Dumping Zombie String: 44 : 僵王博士");
                MLogger.Msg($"Dumping Zombie String: 46 : 僵王博士(二阶段)");

                UnityEngine.Object.Destroy(gameObject);
                List<string> advBuffs = [];
                for (int i = 0; i < TravelMgr.advancedBuffs.Count; i++)
                {
                    if (TravelMgr.advancedBuffs[i] is not null)
                    {
                        MLogger.LogInfo($"Dumping Advanced Buff String:#{i} {TravelMgr.advancedBuffs[i]}");
                        advBuffs.Add($"#{i} {TravelMgr.advancedBuffs[i]}");
                    }
                }
                List<string> ultiBuffs = [];
                for (int i = 0; i < TravelMgr.ultimateBuffs.Count; i++)
                {
                    if (TravelMgr.ultimateBuffs[i] is not null)
                    {
                        MLogger.LogInfo($"Dumping Ultimate Buff String:#{i} {TravelMgr.ultimateBuffs[i]}");
                        ultiBuffs.Add($"#{i} {TravelMgr.ultimateBuffs[i]}");
                    }
                }
                List<string> debuffs = [];
                for (int i = 0; i < TravelMgr.debuffs.Count; i++)
                {
                    if (TravelMgr.debuffs[i] is not null)
                    {
                        MLogger.LogInfo($"Dumping Debuff String:#{i} {TravelMgr.debuffs[i]}");
                        debuffs.Add(TravelMgr.debuffs[i]);
                    }
                }
                AdvBuffs = new bool[TravelMgr.advancedBuffs.Count];
                UltiBuffs = new bool[TravelMgr.ultimateBuffs.Count];
                Debuffs = new bool[TravelMgr.debuffs.Count];

                Dictionary<int, string> bullets = [];

                for (int i = 0; i < GameAPP.bulletPrefab.Count; i++)
                {
                    if (GameAPP.bulletPrefab[i] is not null)
                    {
                        string text = $"{i} : {GameAPP.bulletPrefab[i].name}";
                        MLogger.LogInfo($"Dumping Bullet String: {text}");
                        bullets.Add(i, text);
                        BulletDamage.Add((BulletType)i, -1);
                    }
                }
                Dictionary<int, string> firsts = [];
                foreach (var first in Enum.GetValues(typeof(Zombie.FirstArmorType)))
                {
                    firsts.Add((int)first, $"{first}");
                }
                Dictionary<int, string> seconds = [];
                foreach (var second in Enum.GetValues(typeof(Zombie.SecondArmorType)))
                {
                    seconds.Add((int)second, $"{second}");
                }
                MLogger.LogWarning("以上id信息为动态生成，仅适用于当前游戏实例！！！");
                MLogger.LogWarning("以上id信息为动态生成，仅适用于当前游戏实例！！！");
                MLogger.LogWarning("以上id信息为动态生成，仅适用于当前游戏实例！！！");

                InitData data = new()
                {
                    Plants = plants,
                    Zombies = zombies,
                    AdvBuffs = [.. advBuffs],
                    UltiBuffs = [.. ultiBuffs],
                    Bullets = bullets,
                    FirstArmors = firsts,
                    SecondArmors = seconds,
                    Debuffs = [.. debuffs],
                };
                File.WriteAllText("./PVZRHTools/InitData.json", JsonSerializer.Serialize(data));

                _ = DataSync.Instance.Value;
            }
            catch (Exception ex)
            {
                LoggerInstance.Error(ex);
            }
            inited = true;
        }

        public override void Load()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            ClassInjector.RegisterTypeInIl2Cpp<PatchMgr>();
            ClassInjector.RegisterTypeInIl2Cpp<DataProcessor>();
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
            KeyAlmanacCreatePlantVase = new(Config.Bind("PVZRHTools", nameof(KeyAlmanacCreatePlantVase), KeyCode.J));
            KeyAlmanacCreateZombieVase = new(Config.Bind("PVZRHTools", nameof(KeyAlmanacCreateZombieVase), KeyCode.K));

            KeyBindings = new
            ([
                KeyTimeStop.Value,KeyTopMostCardBank.Value,KeyShowGameInfo.Value,
                KeyAlmanacCreatePlant.Value,KeyAlmanacCreateZombie.Value,KeyAlmanacZombieMindCtrl.Value,
                KeyAlmanacCreatePlantVase.Value,KeyAlmanacCreateZombieVase.Value
            ]);
            Config.Save();
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

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr MessageBox(int hWnd, string text, string caption, uint type);

        public static Lazy<ConfigEntry<bool>> AlmanacZombieMindCtrl { get; set; } = new();
        public static Lazy<Core> Instance { get; set; } = new();
        public static Lazy<ConfigEntry<KeyCode>> KeyAlmanacCreatePlant { get; set; } = new();
        public static Lazy<ConfigEntry<KeyCode>> KeyAlmanacCreatePlantVase { get; set; } = new();
        public static Lazy<ConfigEntry<KeyCode>> KeyAlmanacCreateZombie { get; set; } = new();
        public static Lazy<ConfigEntry<KeyCode>> KeyAlmanacCreateZombieVase { get; set; } = new();
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