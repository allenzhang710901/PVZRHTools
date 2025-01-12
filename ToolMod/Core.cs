using Il2Cpp;
using Il2CppTMPro;
using MelonLoader;
using MelonLoader.InternalUtils;
using Microsoft.Diagnostics.Runtime.Utilities;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using ToolModData;
using UnityEngine;
using static ToolMod.PatchConfig;

namespace ToolMod
{
    public class Core : MelonMod
    {
        public static Lazy<Core> Instance { get; set; } = new();
        public static bool inited = false;
        public static bool Dev => false;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr MessageBox(int hWnd, string text, string caption, uint type);

        public override void OnInitializeMelon()
        {
            Console.OutputEncoding = Encoding.UTF8;
            if (UnityInformationHandler.GameVersion != "2.1.6")
            {
                string caption = "修改器启动错误";
                MessageBox(0, "游戏版本错误，修改器仅支持2.1.6版本。你需要自行更换游戏版本。请勿向修改器作者反馈此问题，看到也不会回复。", caption, 0U);
                Environment.Exit(0);
            }
            Instance = new(this);
            if (Time.timeScale == 0) Time.timeScale = 1;
            SyncSpeed = Time.timeScale;
            inited = true;
        }

        public override void OnLateInitializeMelon()
        {
            GameAPP.plantPrefab[257] = Resources.Load<GameObject>("Plants/Peashooter/Electricpea/Electricpeaprefab");
            GameAPP.prePlantPrefab[257] = Resources.Load<GameObject>("Plants/Peashooter/Electricpea/Electricpeapreview");
            if (Dev)
            {
                string plantsPath = Application.dataPath + "/LawnStrings.json";
                string plantsJson;
                if (!File.Exists(plantsPath))
                {
                    plantsJson = Resources.Load<TextAsset>("LawnStrings").text;
                }
                else
                {
                    plantsJson = File.ReadAllText(plantsPath);
                }
                Dictionary<int, string> plants = [];
                foreach (var plant in JsonNode.Parse(plantsJson)!["plants"]!.AsArray())
                {
                    if (plant is not null) plants.Add((int)plant["seedType"]!, (int)plant["seedType"]! + " : " + (string)plant["name"]!);
                }
                plants.Add(257, "257：闪电豌豆");
                string zombiesPath = Application.dataPath + "/ZombieStrings.json";
                string zombiesJson;
                if (!File.Exists(zombiesPath))
                {
                    zombiesJson = Resources.Load<TextAsset>("ZombieStrings").text;
                }
                else
                {
                    zombiesJson = File.ReadAllText(zombiesPath);
                }
                Dictionary<int, string> zombies = [];
                foreach (var zombie in JsonNode.Parse(zombiesJson)!["zombies"]!.AsArray())
                {
                    if (zombie is not null) zombies.Add((int)zombie["theZombieType"]!, (int)zombie["theZombieType"]! + " : " + (string)zombie["name"]!);
                }
                zombies.Add(44, "44：僵王博士");
                zombies.Add(114, "114：铁桶铁门铁豌豆僵尸");
                List<string> advBuffs = [];
                foreach (var adv in TravelMgr.advancedBuffs)
                {
                    advBuffs.Add(adv.Value);
                }
                List<string> ultiBuffs = [];
                foreach (var ulti in TravelMgr.ultimateBuffs)
                {
                    ultiBuffs.Add(ulti.Value);
                }
                Dictionary<int, string> bullets = [];
                foreach (var type in Enum.GetValues(typeof(CreateBullet.BulletType)))
                {
                    if ((int)type is 25 or 26 or 27) continue;
                    bullets.Add((int)type, $"{type}");
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

                initData = new()
                {
                    Plants = plants,
                    Zombies = zombies,
                    AdvBuffs = [.. advBuffs],
                    UltiBuffs = [.. ultiBuffs],
                    Bullets = bullets,
                    FirstArmors = firsts,
                    SecondArmors = seconds,
                };
                File.WriteAllText("./PVZRHTools/InitData.json", JsonSerializer.Serialize(initData));
            }
            if (!Directory.Exists("./PVZRHTools"))
            {
                string caption = "修改器启动错误";
                MessageBox(0, "PVZRHTools文件夹丢失或名称错误或解压位置错误，请仔细检查解压时是否存在纰漏。", caption, 0U);
                Environment.Exit(0);
            }
            else if (!File.Exists("./PVZRHTools/InitData.json") || !File.Exists("./PVZRHTools/PVZRHTools.exe"))
            {
                string caption = "修改器启动错误";
                MessageBox(0, "PVZRHTools文件夹中核心文件丢失，请仔细检查解压时是否存在纰漏。", caption, 0U);
                Environment.Exit(0);
            }
            else
            {
                try
                {
                    _ = DataSync.Instance.Value;
                }
                catch (Exception ex)
                {
                    LoggerInstance.Error(ex);
                }
            }
        }

        public static InitData initData;

        public override void OnGUI()
        {
            if (InGame() && ShowNextWaveTime)
            {
                GUI.Label(new(950, 670, 150, 40), $"波数：{Board.Instance.theWave}/{Board.Instance.theMaxWave}    刷怪CD：{Board.Instance.newZombieWaveCountDown:N1}", new GUIStyle()
                {
                    fontSize = 16,
                    normal = { textColor = Color.cyan },
                    fontStyle = FontStyle.Italic,
                });
            }
        }

        public override void OnUpdate()
        {
            Update();
        }

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

        public static int SpeedToTick(float scale) => scale switch
        {
            0 => 0,
            0.2f => 1,
            0.5f => 2,
            2 => 4,
            3 => 5,
            4 => 6,
            6 => 7,
            8 => 8,
            16 => 9,
            _ => 3
        };
    }
}