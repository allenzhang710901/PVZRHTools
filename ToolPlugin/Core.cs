using HarmonyLib;
using Il2Cpp;
using Il2CppTMPro;
using MelonLoader;
using MelonLoader.InternalUtils;
using MelonLoader.Utils;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using ToolModData;
using UnityEngine;
using static ToolModData.Modifier;

[assembly: MelonInfo(typeof(ToolPlugin.Core), "ToolPlugin", "2.3.1-3.14", "Infinite75", null)]
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
                //我****************************
                plantsJson = plantsJson.Replace("\"cost\": \"\",\r\n            \"cost\": \"\"", "\"cost\": \"\"");
                //我****************************
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
                zombies.Add(44, "44 : 僵王博士");
                zombies.Add(46, "46 : 僵王博士(二阶段)");
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
                List<string> debuffs = [];
                foreach (var de in TravelMgr.debuffs)
                {
                    debuffs.Add(de.Value);
                }
                Dictionary<int, string> bullets = [];
                foreach (var type in Enum.GetValues(typeof(BulletType)))
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
                    Debuffs = [.. debuffs],
                };
                File.WriteAllText("./PVZRHTools/InitData.json", JsonSerializer.Serialize(InitData));
            }
        }

        public override void OnPreInitialization()
        {
            Console.OutputEncoding = Encoding.UTF8;
            if (UnityInformationHandler.GameVersion != "2.3.1")
            {
                string caption = "修改器启动错误";
                MessageBox(0, "游戏版本错误，修改器仅支持2.3.1版本。你需要自行更换游戏版本。请勿向修改器作者反馈此问题，看到也不会回复。", caption, 0U);
                Environment.Exit(0);
            }
            LoggerInstance.Msg("游戏版本2.3.1：Version Check OK");

            if (!Directory.Exists("./PVZRHTools"))
            {
                string caption = "修改器启动错误";
                MessageBox(0, "PVZRHTools文件夹丢失或名称错误或解压位置错误，请检查是否受杀毒软件影响。", caption, 0U);
                Environment.Exit(0);
            }
            else if (!File.Exists("./PVZRHTools/InitData.json") || !File.Exists("./PVZRHTools/PVZRHTools.exe"))
            {
                string caption = "修改器启动错误";
                MessageBox(0, "PVZRHTools文件夹中核心文件丢失，请检查是否受杀毒软件影响。", caption, 0U);
                Environment.Exit(0);
            }
            LoggerInstance.Msg("PVZRHTools文件夹：Files Check OK");
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr MessageBox(int hWnd, string text, string caption, uint type);

        public static InitData InitData { get; set; }
    }
}