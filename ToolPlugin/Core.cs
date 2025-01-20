using Il2Cpp;
using Il2CppTMPro;
using MelonLoader;
using MelonLoader.InternalUtils;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using ToolModData;
using UnityEngine;
using static ToolModData.Modifier;

[assembly: MelonInfo(typeof(ToolPlugin.Core), "ToolPlugin", "3.10", "Infinite75", null)]
[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]

namespace ToolPlugin
{
    public class Core : MelonPlugin
    {
        public override void OnInitializeMelon()
        {
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
                //TODO:Delete in 2.1.7/2.2
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

                InitData = new()
                {
                    Plants = plants,
                    Zombies = zombies,
                    AdvBuffs = [.. advBuffs],
                    UltiBuffs = [.. ultiBuffs],
                    Bullets = bullets,
                    FirstArmors = firsts,
                    SecondArmors = seconds,
                };
                File.WriteAllText("./PVZRHTools/InitData.json", JsonSerializer.Serialize(InitData));
            }
        }

        public override void OnPreInitialization()
        {
            Console.OutputEncoding = Encoding.UTF8;
            if (UnityInformationHandler.GameVersion != "2.1.6")
            {
                string caption = "修改器启动错误";
                MessageBox(0, "游戏版本错误，修改器仅支持2.2版本。你需要自行更换游戏版本。请勿向修改器作者反馈此问题，看到也不会回复。", caption, 0U);
                Environment.Exit(0);
            }
            LoggerInstance.Msg("游戏版本2.2：Version Check OK");

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
            LoggerInstance.Msg("PVZRHTools文件夹：Files Check OK");
        }

        public override void OnUpdate()
        {
            if (GameAPP.theGameStatus is 0 or 2 or 3)
            {
                try
                {
                    var slow = GameObject.Find("InGameUIFHD").GetComponent<InGameUIMgr>().SlowTrigger.transform;
                    slow.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = $"时停(x{Time.timeScale})";
                    slow.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = $"时停(x{Time.timeScale})";
                }
                catch { }

                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    if (GameAPP.canvas.GetComponent<Canvas>().sortingLayerName == "Default")
                    {
                        GameAPP.canvas.GetComponent<Canvas>().sortingLayerName = "UI";
                    }
                    else
                    {
                        GameAPP.canvas.GetComponent<Canvas>().sortingLayerName = "Default";
                    }
                }
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr MessageBox(int hWnd, string text, string caption, uint type);

        public static InitData InitData { get; set; }
    }
}