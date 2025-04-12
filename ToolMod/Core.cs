using Harmony;
using Il2Cpp;
using Il2CppTMPro;
using MelonLoader;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text.Json;
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
            KeyAlmanacCreatePlantVase = new(Config.Value.CreateEntry(nameof(KeyAlmanacCreatePlantVase), KeyCode.J));
            KeyAlmanacCreateZombieVase = new(Config.Value.CreateEntry(nameof(KeyAlmanacCreateZombieVase), KeyCode.K));

            KeyBindings = new
            ([
                KeyTimeStop.Value,KeyTopMostCardBank.Value,KeyShowGameInfo.Value,
                KeyAlmanacCreatePlant.Value,KeyAlmanacCreateZombie.Value,KeyAlmanacZombieMindCtrl.Value,
                KeyAlmanacCreatePlantVase.Value,KeyAlmanacCreateZombieVase.Value,
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
                MLogger.Warning("以下id信息为动态生成，仅适用于当前游戏实例！！！");
                MLogger.Warning("以下id信息为动态生成，仅适用于当前游戏实例！！！");
                MLogger.Warning("以下id信息为动态生成，仅适用于当前游戏实例！！！");

                Dictionary<int, string> plants = [];
                Dictionary<int, string> zombies = [];

                //GameObject gameObject = new();
                //GameObject back = new();
                //back.transform.SetParent(gameObject.transform);
                //GameObject name = new("Name");
                //GameObject shadow = new("Shadow");
                //shadow.AddComponent<TextMeshPro>();
                //shadow.transform.SetParent(name.transform);
                //var nameText = name.AddComponent<TextMeshPro>();
                //name.transform.SetParent(gameObject.transform);
                //GameObject info = new("Info");
                //info.AddComponent<TextMeshPro>();
                //info.transform.SetParent(gameObject.transform);
                //GameObject cost = new("Cost");
                //cost.AddComponent<TextMeshPro>();
                //cost.transform.SetParent(gameObject.transform);
                //var almz = gameObject.AddComponent<AlmanacMgrZombie>();
                //gameObject.AddComponent<TravelMgr>();
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
                    MLogger.Msg($"Dumping Plant String: {item}");
                    plants.Add(i, item);
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
                UnityEngine.Object.Destroy(gameObject2);

                List<string> advBuffs = [];
                for (int i = 0; i < TravelMgr.advancedBuffs.Count; i++)
                {
                    if (TravelMgr.advancedBuffs[i] is not null)
                    {
                        MLogger.Msg($"Dumping Advanced Buff String:#{i} {TravelMgr.advancedBuffs[i]}");
                        advBuffs.Add($"#{i} {TravelMgr.advancedBuffs[i]}");
                    }
                }
                List<string> ultiBuffs = [];
                for (int i = 0; i < TravelMgr.ultimateBuffs.Count; i++)
                {
                    if (TravelMgr.ultimateBuffs[i] is not null)
                    {
                        MLogger.Msg($"Dumping Ultimate Buff String:#{i} {TravelMgr.ultimateBuffs[i]}");
                        ultiBuffs.Add($"#{i} {TravelMgr.ultimateBuffs[i]}");
                    }
                }
                List<string> debuffs = [];
                for (int i = 0; i < TravelMgr.debuffs.Count; i++)
                {
                    if (TravelMgr.debuffs[i] is not null)
                    {
                        MLogger.Msg($"Dumping Debuff String:#{i} {TravelMgr.debuffs[i]}");
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
                        MLogger.Msg($"Dumping Bullet String: {text}");
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
                MLogger.Warning("以上id信息为动态生成，仅适用于当前游戏实例！！！");
                MLogger.Warning("以上id信息为动态生成，仅适用于当前游戏实例！！！");
                MLogger.Warning("以上id信息为动态生成，仅适用于当前游戏实例！！！");

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
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr MessageBox(int hWnd, string text, string caption, uint type);

        public static Lazy<MelonPreferences_Entry<bool>> AlmanacZombieMindCtrl { get; set; } = new();
        public static Lazy<MelonPreferences_Category> Config { get; set; } = new();
        public static Lazy<Core> Instance { get; set; } = new();
        public static Lazy<MelonPreferences_Entry<KeyCode>> KeyAlmanacCreatePlant { get; set; } = new();
        public static Lazy<MelonPreferences_Entry<KeyCode>> KeyAlmanacCreatePlantVase { get; set; } = new();

        public static Lazy<MelonPreferences_Entry<KeyCode>> KeyAlmanacCreateZombie { get; set; } = new();
        public static Lazy<MelonPreferences_Entry<KeyCode>> KeyAlmanacCreateZombieVase { get; set; } = new();

        public static Lazy<MelonPreferences_Entry<KeyCode>> KeyAlmanacZombieMindCtrl { get; set; } = new();
        public static Lazy<List<MelonPreferences_Entry<KeyCode>>> KeyBindings { get; set; } = new();
        public static Lazy<MelonPreferences_Entry<KeyCode>> KeyShowGameInfo { get; set; } = new();
        public static Lazy<MelonPreferences_Entry<KeyCode>> KeyTimeStop { get; set; } = new();
        public static Lazy<MelonPreferences_Entry<KeyCode>> KeyTopMostCardBank { get; set; } = new();
        public static Lazy<MelonPreferences_Entry<int>> Port { get; set; } = new();
        public static bool inited = false;
    }
}