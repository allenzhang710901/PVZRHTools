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
    [HarmonyPatch(typeof(NoticeMenu), "Awake")]
    public static class Help_Patch
    {
        public static void Postfix() => Core.Instance.Value.LateInit();
    }

    [BepInPlugin("inf75.toolmod", "ToolMod", "3.20")]
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
                GameObject back1 = new();
                back1.transform.SetParent(gameObject.transform);
                GameObject name1 = new("Name");
                GameObject shadow1 = new("Shadow");
                shadow1.transform.SetParent(name1.transform);
                var nameText1 = name1.AddComponent<TextMeshPro>();
                name1.transform.SetParent(gameObject.transform);
                GameObject info1 = new("Info");
                info1.transform.SetParent(gameObject.transform);
                GameObject cost1 = new("Cost");
                cost1.transform.SetParent(gameObject.transform);
                var alm = gameObject.AddComponent<AlmanacPlantBank>();
                alm.cost = cost1.AddComponent<TextMeshPro>();
                alm.plantName_shadow = shadow1.AddComponent<TextMeshPro>();
                alm.plantName = name1.GetComponent<TextMeshPro>();
                alm.introduce = info1.AddComponent<TextMeshPro>(); ;

                for (int i = 0; i < GameAPP.resourcesManager.allPlants.Count; i++)
                {
                    alm.theSeedType = (int)GameAPP.resourcesManager.allPlants[i];
                    alm.InitNameAndInfoFromJson();
                    string item = $"{(int)GameAPP.resourcesManager.allPlants[i]} : {alm.plantName.GetComponent<TextMeshPro>().text}";
                    MLogger.LogInfo($"Dumping Plant String: {item}");
                    plants.Add((int)GameAPP.resourcesManager.allPlants[i], item);
                    HealthPlants.Add(GameAPP.resourcesManager.allPlants[i], -1);
                    alm.plantName.GetComponent<TextMeshPro>().text = "";
                }
                UnityEngine.Object.Destroy(gameObject);

                GameObject gameObject2 = new();
                GameObject back2 = new();
                back2.transform.SetParent(gameObject2.transform);
                back2.AddComponent<SpriteRenderer>();
                GameObject name2 = new("Name");
                GameObject shadow2 = new("Name_1");
                shadow2.transform.SetParent(name2.transform);
                shadow2.AddComponent<TextMeshPro>();
                name2.AddComponent<TextMeshPro>();
                name2.transform.SetParent(gameObject2.transform);
                name2.AddComponent<TextMeshPro>();
                GameObject info2 = new("Info");
                info2.transform.SetParent(gameObject2.transform);
                var almz = gameObject2.AddComponent<AlmanacMgrZombie>();
                almz.info = info2;
                almz.zombieName = name2;
                almz.introduce = info2.AddComponent<TextMeshPro>(); ;

                for (int i = 0; i < GameAPP.resourcesManager.allZombieTypes.Count; i++)
                {
                    almz.theZombieType = GameAPP.resourcesManager.allZombieTypes[i];
                    almz.InitNameAndInfoFromJson();
                    HealthZombies.Add(GameAPP.resourcesManager.allZombieTypes[i], -1);

                    if (!string.IsNullOrEmpty(almz.zombieName.GetComponent<TextMeshPro>().text))
                    {
                        string item = $"{(int)GameAPP.resourcesManager.allZombieTypes[i]} : {almz.zombieName.GetComponent<TextMeshPro>().text}";
                        MLogger.LogInfo($"Dumping Zombie String: {item}");
                        zombies.Add((int)GameAPP.resourcesManager.allZombieTypes[i], item);
                        almz.zombieName.GetComponent<TextMeshPro>().text = "";
                    }
                }
                UnityEngine.Object.Destroy(gameObject2);

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
                PatchMgr.UltiBuffs = new bool[TravelMgr.ultimateBuffs.Count];
                Debuffs = new bool[TravelMgr.debuffs.Count];

                Dictionary<int, string> bullets = [];

                for (int i = 0; i < GameAPP.resourcesManager.allBullets.Count; i++)
                {
                    if (GameAPP.resourcesManager.bulletPrefabs[GameAPP.resourcesManager.allBullets[i]] is not null)
                    {
                        string text = $"{(int)GameAPP.resourcesManager.allBullets[i]} : {GameAPP.resourcesManager.bulletPrefabs[GameAPP.resourcesManager.allBullets[i]].name}";
                        MLogger.LogInfo($"Dumping Bullet String: {text}");
                        bullets.Add((int)GameAPP.resourcesManager.allBullets[i], text);
                        BulletDamage.Add(GameAPP.resourcesManager.allBullets[i], -1);
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
                LoggerInstance.LogError(ex);
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