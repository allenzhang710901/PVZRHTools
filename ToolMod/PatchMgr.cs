using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppTMPro;
using MelonLoader;
using ToolModData;
using Unity.VisualScripting;
using UnityEngine;
using static ToolMod.PatchConfig;

namespace ToolMod
{
    [RegisterTypeInIl2Cpp]
    public class PatchConfig : MonoBehaviour
    {
        public PatchConfig() : base(ClassInjector.DerivedConstructorPointer<PatchConfig>()) => ClassInjector.DerivedConstructorBody(this);

        public PatchConfig(IntPtr i) : base(i)
        {
        }

        public static bool GloveNoCD { get; set; } = false;
        public static bool HammerNoCD { get; set; } = false;
        public static bool FreePlanting { get; set; } = false;
        public static bool FreeCD { get; set; } = false;
        public static bool UnlockAllFusions { get; set; } = false;
        public static bool SuperPresent { get; set; } = false;
        public static bool UltimateRamdomZombie { get; set; } = false;
        public static bool PresentFastOpen { get; set; } = false;
        public static int LockPresent { get; set; } = -1;
        public static bool FastShooting { get; set; } = false;
        public static bool HardPlant { get; set; } = false;
        public static bool NoHole { get; set; } = false;
        public static bool MineNoCD { get; set; } = false;
        public static bool ChomperNoCD { get; set; } = false;
        public static bool HyponoEmperorNoCD { get; set; } = false;
        public static bool CobCannonNoCD { get; set; } = false;
        public static bool NoIceRoad { get; set; } = false;
        public static bool ItemExistForever { get; set; } = false;
        public static bool CardNoInit { get; set; } = false;
        public static bool JackboxNotExplode { get; set; } = false;
        public static float SyncSpeed { get; set; } = -1;
        public static bool LockSun { get; set; } = false;
        public static int LockSunCount { get; set; } = 500;
        public static bool LockMoney { get; set; } = false;
        public static bool TimeStop { get; set; } = false;
        public static bool TimeSlow { get; set; } = false;
        public static int LockMoneyCount { get; set; } = 3000;
        public static bool StopSummon { get; set; } = false;
        public static bool ZombieSea { get; set; } = false;
        public static int ZombieSeaCD { get; set; } = 40;
        public static int LockBullet { get; set; } = -2;
        public static bool ShowNextWaveTime { get; set; } = false;
        public static bool UndeadBullet { get; set; } = false;
        public static bool GarlicDay { get; set; } = false;
        public static GameModes GameModes { get; set; }
        public static List<ToolModData.Card> CardReplaces { get; set; } = [];
        public static Dictionary<MixData.PlantType, int> HealthPlants { get; set; } = [];
        public static Dictionary<ZombietType, int> HealthZombies { get; set; } = [];
        public static Dictionary<Zombie.FirstArmorType, int> Health1st { get; set; } = [];
        public static Dictionary<Zombie.SecondArmorType, int> Health2nd { get; set; } = [];
        public static Dictionary<CreateBullet.BulletType, int> BulletDamage { get; set; } = [];
        public static bool[] AdvBuffs { get; set; } = new bool[34];
        public static bool[] UltiBuffs { get; set; } = new bool[20];
        public static bool[] InGameAdvBuffs { get; set; } = new bool[34];
        public static bool[] InGameUltiBuffs { get; set; } = new bool[20];

        public static int seaTime = 0;
        public static int garlicDayTime = 0;
        public static int originalLevel;
        public static float originalSpeed;
        public static List<int> SeaTypes { get; set; } = [];

        public static void ChangeCard()
        {
            if (!InGame()) return;
            foreach (var c in CardUIReplacer.Replacers)
            {
                if (c is not null)
                {
                    foreach (var r in CardReplaces)
                    {
                        if (r.ID == c)
                        {
                            if (r.Enabled)
                            {
                                c.ChangeCard(r.NewID, r.Sun, r.CD);
                            }
                            else
                            {
                                c.Resume();
                            }
                        }
                    }
                }
            }
        }

        public static void Update()
        {
            if (GameAPP.theGameStatus is 0 or 2 or 3)
            {
                try
                {
                    if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.R))
                    {
                        TimeStop = !TimeStop;
                        TimeSlow = false;
                    }
                    if (Input.GetKeyDown(KeyCode.Alpha3))
                    {
                        TimeStop = false;
                        TimeSlow = !TimeSlow;
                    }
                    if (!TimeStop && !TimeSlow)
                    {
                        Time.timeScale = SyncSpeed;
                    }
                    if (TimeStop && !TimeSlow)
                    {
                        Time.timeScale = 0;
                    }
                    if (!TimeStop && TimeSlow)
                    {
                        Time.timeScale = 0.2f;
                    }
                    if (Input.GetKeyDown(KeyCode.BackQuote))
                    {
                        ShowNextWaveTime = !ShowNextWaveTime;
                    }
                    var slow = GameObject.Find("InGameUIFHD").GetComponent<InGameUIMgr>().SlowTrigger.transform;
                    slow.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = $"时停(x{Time.timeScale})";
                    slow.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = $"时停(x{Time.timeScale})";
                    if (GameModes.Shooting1)
                    {
                        GameAPP.theBoardLevel = 40;
                    }
                    if (GameModes.Shooting2)
                    {
                        GameAPP.theBoardLevel = 72;
                    }
                    if (GameModes.Shooting3)
                    {
                        GameAPP.theBoardLevel = 84;
                    }
                    if (GameModes.Shooting4)
                    {
                        GameAPP.theBoardLevel = 88;
                    }
                }
                catch (NullReferenceException) { }
            }
            if (!InGame()) return;
            if (GameAPP.theGameStatus is 1)
            {
                GameAPP.theBoardLevel = originalLevel;
            }
            if (LockSun)
            {
                Board.Instance.theSun = LockSunCount;
            }
            if (LockMoney)
            {
                Board.Instance.theMoney = LockMoneyCount;
            }
            if (StopSummon)
            {
                Board.Instance.iceDoomFreezeTime = 1;
            }

            if (ZombieSea)
            {
                if (++seaTime >= ZombieSeaCD &&
                    Board.Instance.theWave is not 0 && Board.Instance.theWave < Board.Instance.theMaxWave &&
                    GameAPP.theGameStatus == (int)GameStatus.InGame)
                {
                    foreach (var j in SeaTypes)
                    {
                        if (j < 0) continue;
                        for (int i = 0; i < Board.Instance.rowNum; i++)
                        {
                            CreateZombie.Instance.SetZombie(i, j);
                        }
                        seaTime = 0;
                    }
                }
            }
            if (GarlicDay && ++garlicDayTime >= 500 && GameAPP.theGameStatus == (int)GameStatus.InGame)
            {
                garlicDayTime = 0;
                _ = FindObjectsOfTypeAll(Il2CppType.Of<Zombie>()).All(b =>
                {
                    b?.TryCast<Zombie>()?.StartCoroutine_Auto(b?.TryCast<Zombie>()?.DeLayGarliced(0.1f, false, false));
                    return true;
                });
            }
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

        public static void UpdateInGameBuffs()
        {
            for (int i = 0; i < GameAPP.gameAPP.GetOrAddComponent<TravelMgr>().advancedUpgrades.Count; i++)
            {
                GameAPP.gameAPP.GetOrAddComponent<TravelMgr>().advancedUpgrades![i] = InGameAdvBuffs[i];
            }
            for (int i = 0; i < GameAPP.gameAPP.GetOrAddComponent<TravelMgr>().ultimateUpgrades.Count; i++)
            {
                GameAPP.gameAPP.GetOrAddComponent<TravelMgr>().ultimateUpgrades![i] = InGameUltiBuffs[i];
            }
        }

        public static void SyncInGameBuffs()
        {
            if (!InGame()) return;
            DataSync.Instance.Value.SendData(new SyncTravelBuff()
            {
                AdvInGame = GameAPP.gameAPP.GetOrAddComponent<TravelMgr>().advancedUpgrades!.ToList(),
                UltiInGame = GameAPP.gameAPP.GetOrAddComponent<TravelMgr>().ultimateUpgrades!.ToList()
            });
        }

        public static bool InGame() => Board.Instance is not null && GameAPP.theGameStatus != -2 && GameAPP.theGameStatus != -1 && GameAPP.theGameStatus != 4;

        public static MelonLogger.Instance MLogger => Core.Instance.Value.LoggerInstance;

        public static GameObject? SeedGroup
        {
            get
            {
                try
                {
                    return InGame() ? GameObject.Find("SeedGroup") : null;
                }
                catch
                {
                    return null;
                }
            }
        }

        static PatchConfig()
        {
            foreach (var p in Enum.GetValues<MixData.PlantType>())
            {
                HealthPlants.Add(p, -1);
            }
            HealthPlants.Add((MixData.PlantType)257, -1);
            foreach (var z in Enum.GetValues<ZombietType>())
            {
                HealthZombies.Add(z, -1);
            }
            foreach (var f in Enum.GetValues<Zombie.FirstArmorType>())
            {
                Health1st.Add(f, -1);
            }
            foreach (var s in Enum.GetValues<Zombie.SecondArmorType>())
            {
                Health2nd.Add(s, -1);
            }
            foreach (var b in Enum.GetValues<CreateBullet.BulletType>())
            {
                BulletDamage.Add(b, -1);
            }
        }
    }

    [HarmonyPatch(typeof(HammerMgr), "Start")]
    public static class HammerMgrPatchB
    {
        public static void Postfix(HammerMgr __instance)
        {
            GameObject obj = new("ModifierHammerCD");
            var text = obj.AddComponent<TextMeshProUGUI>();
            text.font = Resources.Load<TMP_FontAsset>("Fonts/ContinuumBold SDF");
            text.color = new(228f / 256f, 155f / 256f, 38f / 256f);
            obj.transform.SetParent(__instance.GameObject().transform);
            obj.transform.localScale = new(2f, 2f, 2f);
            obj.transform.localPosition = new(107, 0, 0);
        }
    }

    [HarmonyPatch(typeof(GloveMgr), "Start")]
    public static class GloveMgrPatchB
    {
        public static void Postfix(GloveMgr __instance)
        {
            GameObject obj = new("ModifierGloveCD");
            var text = obj.AddComponent<TextMeshProUGUI>();
            text.font = Resources.Load<TMP_FontAsset>("Fonts/ContinuumBold SDF");
            text.color = new(228f / 256f, 155f / 256f, 38f / 256f);
            obj.transform.SetParent(__instance.GameObject().transform);
            obj.transform.localScale = new(0.4f, 0.4f, 0.4f);
            obj.transform.localPosition = new(27.653f, 0, 0);
        }
    }

    [HarmonyPatch(typeof(UIMgr), "EnterMainMenu")]
    public static class UIMgrPatch
    {
        public static void Postfix()
        {
            GameObject obj = new("ModifierInfo");
            var text = obj.AddComponent<TextMeshProUGUI>();
            text.font = Resources.Load<TMP_FontAsset>("Fonts/ContinuumBold SDF");
            text.color = new(1, 1, 0, 1);
            text.text = "修改器作者为b站@Infinite75\n若存在任何付费/要求三连+关注/私信发链接的情况\n说明你被盗版骗了，请注意隐私和财产安全！！！\n此信息仅在游戏主菜单和修改窗口显示";
            obj.transform.SetParent(GameObject.Find("Leaves").transform);
            obj.transform.localScale = new(0.5f, 0.5f, 0.5f);
            obj.GetComponent<RectTransform>().sizeDelta = new(800, 50);
            var t = obj.transform.position;
            t.x = -5.5f;
            t.y = -0.7f;
            obj.transform.position = t;
        }
    }

    [HarmonyPatch(typeof(RandomZombie), "SetRandomZombie")]
    public static class RamdomZombiePatch
    {
        public static bool Prefix(RandomZombie __instance, ref GameObject __result)
        {
            if (!UltimateRamdomZombie) return true;
            if (Board.Instance is not null && Board.Instance.isEveStarted) return true;
            int id = UnityEngine.Random.RandomRangeInt(200, 216);
            if (UnityEngine.Random.RandomRangeInt(0, 6) == 1)
            {
                if (!__instance.isMindControlled)
                {
                    __result = CreateZombie.Instance.SetZombie(__instance.theZombieRow, id, __instance.GameObject().transform.position.x);
                }
                else
                {
                    __result = CreateZombie.Instance.SetZombieWithMindControl(__instance.theZombieRow, id, __instance.GameObject().transform.position.x);
                }
                return false;
            }
            else { return true; }
        }
    }

    [HarmonyPatch(typeof(GameAPP), "Start")]
    public static class GameAppPatch
    {
        public static void Postfix(GameAPP __instance)
        {
            GameObject obj = new("Modifier");
            UnityEngine.Object.DontDestroyOnLoad(obj);

            obj.AddComponent<DataProcessor>();
            obj.AddComponent<PatchConfig>();
        }
    }

    [HarmonyPatch(typeof(InGameText), "EnableText")]
    public static class InGameTextPatch
    {
        public static void Postfix()
        {
            //游戏->修改窗口
            for (int i = 0; i < InGameAdvBuffs.Length; i++)
            {
                if (InGameAdvBuffs[i] != GameAPP.gameAPP.GetOrAddComponent<TravelMgr>().advancedUpgrades[i])
                {
                    SyncInGameBuffs();
                    return;
                }
            }
            for (int i = 0; i < InGameUltiBuffs.Length; i++)
            {
                if (InGameUltiBuffs[i] != GameAPP.gameAPP.GetOrAddComponent<TravelMgr>().ultimateUpgrades[i])
                {
                    SyncInGameBuffs();
                    return;
                }
            }
        }
    }

    [HarmonyPatch(typeof(Bullet), "Die")]
    public static class BulletPatchB
    {
        public static bool Prefix(Bullet __instance)
        {
            if (UndeadBullet)
            {
                __instance.hit = false;
                __instance.penetrationTimes = int.MaxValue;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(InitBoard), "ReadySetPlant")]
    public static class InitBoardPatchA
    {
        public static void Prefix()
        {
            GameAPP.gameAPP.GetOrAddComponent<TravelMgr>();
            if (PatchConfig.GameModes.IsShooting())
            {
                var t = Board.Instance.boardTag;
                t.isShooting = true;
                Board.Instance.boardTag = t;
            }

            if (CardNoInit)
            {
                if (!InGame()) return;
                if (SeedGroup is not null)
                {
                    for (int i = SeedGroup!.transform.childCount - 1; i >= 0; i--)
                    {
                        try
                        {
                            var card = SeedGroup.transform.GetChild(i);
                            if (card is null || card.childCount is 0) continue;
                            card.GetChild(0).gameObject.GetComponent<CardUI>().CD = card.GetChild(0).gameObject.GetComponent<CardUI>().fullCD;
                        }
                        catch (Exception e) { Core.Instance.Value.LoggerInstance.Msg(e); }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Board), "Awake")]
    public static class BoardPatchA
    {
        public static void Prefix()
        {
            originalLevel = GameAPP.theBoardLevel;
        }
    }

    [HarmonyPatch(typeof(InitZombieList), "InitZombie")]
    public static class InitZombieListPatch
    {
        public static void Prefix(ref int theLevelNumber)
        {
            originalLevel = GameAPP.theBoardLevel * 1;
            if (PatchConfig.GameModes.IsShooting())
            {
                var t = Board.Instance.boardTag;
                t.isShooting = true;
                Board.Instance.boardTag = t;
            }
            if (PatchConfig.GameModes.Shooting1)
            {
                GameAPP.theBoardLevel = 40;
                theLevelNumber = 40;
            }
            if (PatchConfig.GameModes.Shooting2)
            {
                GameAPP.theBoardLevel = 72;
                theLevelNumber = 72;
            }
            if (PatchConfig.GameModes.Shooting3)
            {
                GameAPP.theBoardLevel = 84;
                theLevelNumber = 84;
            }
            if (PatchConfig.GameModes.Shooting4)
            {
                GameAPP.theBoardLevel = 88;
                theLevelNumber = 88;
            }
        }
    }

    [HarmonyPatch(typeof(Board), "Awake")]
    public static class BoardPatchB
    {
        public static void Postfix()
        {
            var t = Board.Instance.boardTag;
            t.isScaredyDream |= PatchConfig.GameModes.ScaredyDream;
            t.isColumn |= PatchConfig.GameModes.ColumnPlanting;
            t.isSeedRain |= PatchConfig.GameModes.SeedRain;
            t.isShooting |= PatchConfig.GameModes.IsShooting();
            t.isExchange |= PatchConfig.GameModes.Exchange;
            Board.Instance.boardTag = t;
        }
    }

    [HarmonyPatch(typeof(InitBoard), "RightMoveCamera")]
    public static class InitBoardPatchB
    {
        public static void Postfix()
        {
            GameAPP.gameAPP.GetOrAddComponent<TravelMgr>();
            InGameAdvBuffs = new bool[34];
            InGameUltiBuffs = new bool[20];
            Board.Instance.freeCD = FreeCD;
            var advs = GameAPP.gameAPP.GetOrAddComponent<TravelMgr>().advancedUpgrades;

            for (int i = 0; i < advs.Count; i++)
            {
                if (GameAPP.theBoardType == 3 && Board.Instance.theCurrentSurvivalRound != 1) break;
                advs[i] = AdvBuffs[i] || advs[i];
            }
            var ultis = GameAPP.gameAPP.GetOrAddComponent<TravelMgr>().ultimateUpgrades;
            for (int i = 0; i < ultis.Count; i++)
            {
                if (GameAPP.theBoardType == 3 && Board.Instance.theCurrentSurvivalRound != 1) break;
                ultis[i] = UltiBuffs[i] || ultis[i];
            }
            InGameAdvBuffs = GameAPP.gameAPP.GetOrAddComponent<TravelMgr>().advancedUpgrades;
            InGameUltiBuffs = GameAPP.gameAPP.GetOrAddComponent<TravelMgr>().ultimateUpgrades;
            SyncInGameBuffs();
            ChangeCard();
        }

        //UIPropertyModifier.Instance.appliers.ForEach(t => t.Item2(t.Item1.isOn));
    }

    [HarmonyPatch(typeof(DroppedCard), "Update")]
    public static class DroppedCardPatch
    {
        public static void Postfix(DroppedCard __instance)
        {
            if (ItemExistForever) __instance.existTime = 0;
        }
    }

    [HarmonyPatch(typeof(GridItem), "CreateGridItem")]
    public class GridItemPatch
    {
        public static bool Prefix(ref int theType)
        {
            if (theType >= 3) return true;
            return !NoHole;
        }
    }

    [HarmonyPatch(typeof(InGameUIMgr), "Start")]
    public static class InGameUIMgrPatch
    {
        public static void Prefix()
        {
            GameAPP.theBoardLevel = originalLevel;
        }

        public static void Postfix()
        {
            if (PatchConfig.GameModes.Shooting1)
            {
                GameAPP.theBoardLevel = 40;
            }
            if (PatchConfig.GameModes.Shooting2)
            {
                GameAPP.theBoardLevel = 72;
            }
            if (PatchConfig.GameModes.Shooting3)
            {
                GameAPP.theBoardLevel = 84;
            }
            if (PatchConfig.GameModes.Shooting4)
            {
                GameAPP.theBoardLevel = 88;
            }
        }
    }

    [HarmonyPatch(typeof(Fertilize), "Update")]
    public static class FertilizePatch
    {
        public static void Postfix(Fertilize __instance)
        {
            if (ItemExistForever) __instance.existTime = 0.1f;
        }
    }

    [HarmonyPatch(typeof(CreateBullet), "SetBullet")]
    public class CreateBulletPatch
    {
        public static void Prefix(ref int theBulletType)
        {
            if (LockBullet == -1)
            {
                theBulletType = (int)Enum.GetValues<CreateBullet.BulletType>()[UnityEngine.Random.Range(0, Enum.GetValues<CreateBullet.BulletType>().Length)];
            }
            if (LockBullet >= 0)
            {
                theBulletType = LockBullet;
            }
        }
    }

    [HarmonyPatch(typeof(Bullet), "Update")]
    public static class BulletPatchA
    {
        public static void Postfix(Bullet __instance)
        {
            if ((CreateBullet.BulletType)__instance.theBulletType is CreateBullet.BulletType.FirePea_yellow or CreateBullet.BulletType.FirePea_orange or CreateBullet.BulletType.FirePea_red) return;
            if (BulletDamage[(CreateBullet.BulletType)__instance.theBulletType] >= 0)
                __instance.theBulletDamage = BulletDamage[(CreateBullet.BulletType)__instance.theBulletType];
        }
    }

    [RegisterTypeInIl2Cpp]
    public class CardUIReplacer : MonoBehaviour
    {
        public CardUIReplacer() : base(ClassInjector.DerivedConstructorPointer<CardUIReplacer>()) => ClassInjector.DerivedConstructorBody(this);

        public CardUIReplacer(IntPtr i) : base(i)
        {
        }

        public void Start()
        {
            originalID = card.theSeedType * 1;
            originalCost = card.theSeedCost * 1;
            originalCD = card.fullCD * 1;
            Replacers.Add(this);
            GameObject obj = new("ModifierCardCD");
            var text = obj.AddComponent<TextMeshProUGUI>();
            text.font = Resources.Load<TMP_FontAsset>("Fonts/ContinuumBold SDF");
            text.color = new(228f / 256f, 155f / 256f, 38f / 256f);
            obj.transform.SetParent(gameObject.transform);
            obj.transform.localScale = new(0.7f, 0.7f, 0.7f);
            obj.transform.localPosition = new(39f, 0, 0);
        }

        public void Update()
        {
            if (gameObject.GetComponent<CardUI>().isAvailable)
            {
                gameObject.transform.FindChild("ModifierCardCD").GameObject().active = false;
            }
            else
            {
                gameObject.transform.FindChild("ModifierCardCD").GameObject().active = true;
                gameObject.transform.FindChild("ModifierCardCD").GameObject().GetComponent<TextMeshProUGUI>().text = $"{gameObject.GetComponent<CardUI>().CD:N1}/{gameObject.GetComponent<CardUI>().fullCD}";
            }
        }

        public void ChangeCard(int id, int cost, float cd)
        {
            card.theSeedType = id >= 0 ? id : originalID;
            card.theSeedCost = cost >= 0 ? cost : originalCost;
            if (cd >= 0.01)
            {
                card.fullCD = cd;
            }
            else if (cd < 0.01 && cd >= 0)
            {
                card.fullCD = 0.01f;
            }
            else
            {
                card.fullCD = originalCD;
            }
            Lawnf.ChangeCardSprite(card.theSeedType, card.gameObject);
        }

        public void Resume() => ChangeCard(-1, -1, -1);

        public static implicit operator int(CardUIReplacer r) => r.originalID;

        public CardUI card => gameObject.GetComponent<CardUI>();
        public int originalID { get; private set; }
        public int originalCost { get; private set; }
        public float originalCD { get; private set; }
        public static List<CardUIReplacer> Replacers { get; set; } = [];
    }

    [HarmonyPatch(typeof(CardUI), "Awake")]
    public static class CardUIPatch
    {
        public static void Postfix(CardUI __instance) => __instance.gameObject.AddComponent<CardUIReplacer>();
    }

    [HarmonyPatch(typeof(Bucket), "Update")]
    public static class BucketPatch
    {
        public static void Postfix(Bucket __instance)
        {
            if (ItemExistForever) __instance.existTime = 0.1f;
        }
    }

    [HarmonyPatch(typeof(HammerMgr), "Update")]
    public static class HammerMgrPatchA
    {
        public static void Postfix(HammerMgr __instance)
        {
            __instance.gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(!HammerNoCD);
            if (HammerNoCD == true)
            {
                __instance.CD = __instance.fullCD;
            }
            if (__instance.avaliable)
            {
                __instance.transform.FindChild("ModifierHammerCD").GameObject().active = false;
            }
            else
            {
                __instance.transform.FindChild("ModifierHammerCD").GameObject().active = true;
                __instance.transform.FindChild("ModifierHammerCD").GameObject().GetComponent<TextMeshProUGUI>().text = $"{__instance.CD:N1}/{__instance.fullCD}";
            }
        }
    }

    [HarmonyPatch(typeof(GloveMgr), "Update")]
    public static class GloveMgrPatchA
    {
        public static void Postfix(GloveMgr __instance)
        {
            __instance.gameObject.transform.GetChild(0).gameObject.SetActive(!GloveNoCD);
            if (GloveNoCD == true)
            {
                __instance.CD = __instance.fullCD;
            }
            if (__instance.avaliable)
            {
                __instance.transform.FindChild("ModifierGloveCD").GameObject().active = false;
            }
            else
            {
                __instance.transform.FindChild("ModifierGloveCD").GameObject().active = true;
                __instance.transform.FindChild("ModifierGloveCD").GameObject().GetComponent<TextMeshProUGUI>().text = $"{__instance.CD:N1}/{__instance.fullCD}";
            }
        }
    }

    [HarmonyPatch(typeof(InitBoard), "StartInit")]
    public static class InitBoardPatchC
    {
        public static void Prefix()
        {
            CardUIReplacer.Replacers = [];
        }
    }

    [HarmonyPatch(typeof(CreatePlant), "SetPlant")]
    public class CreatePlantPatchC
    {
        public static void Prefix(ref bool isFreeSet)
        {
            isFreeSet = FreePlanting || isFreeSet;
        }
    }

    [HarmonyPatch(typeof(CreatePlant), "Lim")]
    public static class CreatePlantPatchA
    {
        public static void Postfix(ref bool __result)
        {
            if (UnlockAllFusions)
            {
                __result = false;
            }
        }
    }

    [HarmonyPatch(typeof(Plant), "Awake")]
    public static class PlantPatchD
    {
        public static void Postfix(Plant __instance)
        {
            __instance.gameObject.GetOrAddComponent<PlantModify>();
            try
            {
                if (HealthPlants[(MixData.PlantType)__instance.thePlantType] >= 0)
                {
                    __instance.thePlantMaxHealth = HealthPlants[(MixData.PlantType)__instance.thePlantType];
                    __instance.thePlantHealth = HealthPlants[(MixData.PlantType)__instance.thePlantType];
                }
            }
            catch { }
        }
    }

    [HarmonyPatch(typeof(CreatePlant), "LimTravel")]
    public static class CreatePlantPatchB
    {
        public static void Postfix(ref bool __result)
        {
            if (UnlockAllFusions)
            {
                __result = false;
            }
        }
    }

    [HarmonyPatch(typeof(Present), "RandomPlant")]
    public static class PresentPatchA
    {
        public static bool Prefix(Present __instance)
        {
            foreach (var plant in __instance.board.plantArray)
            {
                try
                {
                    if (plant.thePlantRow == __instance.thePlantRow && plant.thePlantColumn == __instance.thePlantColumn && plant.thePlantType != __instance.thePlantType)
                    {
                        return true;
                    }
                }
                catch { }
            }

            if (LockPresent >= 0)
            {
                CreatePlant.Instance.SetPlant(__instance.thePlantColumn, __instance.thePlantRow, LockPresent);
                if (CreatePlant.Instance.IsPuff(LockPresent))
                {
                    CreatePlant.Instance.SetPlant(__instance.thePlantColumn, __instance.thePlantRow, LockPresent);
                    CreatePlant.Instance.SetPlant(__instance.thePlantColumn, __instance.thePlantRow, LockPresent);
                }

                return false;
            }
            if (SuperPresent)
            {
                __instance.SuperRandomPlant();
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Present), "Start")]
    public static class PresentPatchB
    {
        public static void Postfix(Present __instance)
        {
            if (PresentFastOpen && __instance.thePlantType != 245) __instance.AnimEvent();
        }
    }

    [RegisterTypeInIl2Cpp]
    public class PlantModify : MonoBehaviour
    {
        public PlantModify() : base(ClassInjector.DerivedConstructorPointer<PlantModify>()) => ClassInjector.DerivedConstructorBody(this);

        public PlantModify(IntPtr i) : base(i)
        {
        }

        private Plant PlantObj => gameObject.GetComponent<Plant>();

        public void Update()
        {
            if (PlantObj is not null)
            {
                if (((PlantObj.TryCast<CobCannon>() && CobCannonNoCD))
                    && PlantObj.attributeCountdown > 0.1f)
                {
                    PlantObj.attributeCountdown = 0.1f;
                }
                if (HealthPlants[(MixData.PlantType)PlantObj.thePlantType] >= 0 && PlantObj.thePlantMaxHealth != HealthPlants[(MixData.PlantType)PlantObj.thePlantType])
                {
                    if (PlantObj.thePlantHealth == PlantObj.thePlantMaxHealth)
                    {
                        PlantObj.thePlantHealth = HealthPlants[(MixData.PlantType)PlantObj.thePlantType];
                    }

                    PlantObj.thePlantMaxHealth = HealthPlants[(MixData.PlantType)PlantObj.thePlantType];
                    PlantObj.UpdateHealthText();
                }
            }
            else
            {
                DestroyImmediate(this, false);
            }
        }
    }

    [HarmonyPatch(typeof(Plant), "Update")]
    public static class PlantPatchB
    {
        public static void Postfix(Plant __instance)
        {
            if (FastShooting && __instance.thePlantAttackCountDown > 0.05f)
            {
                switch (__instance.thePlantType)
                {
                    case 914:
                        {
                            if (__instance.thePlantAttackCountDown > 0.6f)
                            {
                                __instance.thePlantAttackCountDown = 0.6f;
                            }

                            break;
                        }
                    case 28:
                        {
                            if (__instance.thePlantAttackCountDown > 0.3f)
                            {
                                __instance.thePlantAttackCountDown = 0.3f;
                            }
                            break;
                        }
                    default:
                        {
                            __instance.thePlantAttackCountDown = 0.05f;

                            break;
                        }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Plant), "Die")]
    public static class PlantPatchC
    {
        public static bool Prefix(Plant __instance)
        {
            if (HardPlant && __instance.thePlantHealth <= 0)
            {
                __instance.thePlantHealth = __instance.thePlantMaxHealth;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(HyponoEmperor), "Update")]
    public static class HyponoEmperorPatch
    {
        public static void Postfix(HyponoEmperor __instance)
        {
            if (HyponoEmperorNoCD && __instance.summonZombieTime > 2f)
            {
                __instance.summonZombieTime = 2f;
            }
        }
    }

    [HarmonyPatch(typeof(PotatoMine), "Update")]
    public static class PotatoMinePatch
    {
        public static void Prefix(PotatoMine __instance)
        {
            if (MineNoCD && __instance.attributeCountdown > 0.1f)
            {
                __instance.attributeCountdown = 0.1f;
            }
        }
    }

    [HarmonyPatch(typeof(Chomper), "Update")]
    public static class ChomperPatch
    {
        public static void Prefix(Chomper __instance)
        {
            if (ChomperNoCD && __instance.attributeCountdown > 0.1f)
            {
                __instance.attributeCountdown = 0.1f;
            }
        }
    }

    [HarmonyPatch(typeof(Zombie), "Awake")]
    public static class ZombiePatchA
    {
        public static void Postfix(Zombie __instance)
        {
            __instance.gameObject.GetOrAddComponent<ZombieModify>();
        }
    }

    [HarmonyPatch(typeof(DriverZombie), "PositionUpdate")]
    public static class DriverZombiePatch
    {
        public static void Postfix(DriverZombie __instance)
        {
            if (NoIceRoad)
            {
                Board.Instance.iceRoadX[__instance.theZombieRow] = 35f;
            }
        }
    }

    [HarmonyPatch(typeof(JackboxZombie), "Update")]
    public static class JackboxZombiePatch
    {
        public static void Postfix(JackboxZombie __instance)
        {
            if (JackboxNotExplode)
            {
                __instance.popCountDown = __instance.originalCountDown;
            }
        }
    }

    [RegisterTypeInIl2Cpp]
    public class ZombieModify : MonoBehaviour

    {
        public ZombieModify() : base(ClassInjector.DerivedConstructorPointer<ZombieModify>()) => ClassInjector.DerivedConstructorBody(this);

        public ZombieModify(IntPtr i) : base(i)
        {
        }

        public void Update()
        {
            try
            {
                if (ZombieObj is not null)
                {
                    if (HealthZombies[(ZombietType)ZombieObj.theZombieType] >= 0 && ZombieObj.theMaxHealth != HealthZombies[(ZombietType)ZombieObj.theZombieType])
                    {
                        if (ZombieObj.theHealth == ZombieObj.theMaxHealth)
                        {
                            ZombieObj.theHealth = HealthZombies[(ZombietType)ZombieObj.theZombieType];
                        }

                        ZombieObj.theMaxHealth = HealthZombies[(ZombietType)ZombieObj.theZombieType];
                    }
                    if (Health1st[ZombieObj.theFirstArmorType] >= 0 && ZombieObj.theMaxHealth != Health1st[ZombieObj.theFirstArmorType])
                    {
                        if (ZombieObj.theFirstArmorHealth == ZombieObj.theFirstArmorMaxHealth)
                        {
                            ZombieObj.theFirstArmorHealth = Health1st[ZombieObj.theFirstArmorType];
                        }

                        ZombieObj.theFirstArmorMaxHealth = Health1st[ZombieObj.theFirstArmorType];
                    }
                    if (Health2nd[ZombieObj.theSecondArmorType] >= 0 && ZombieObj.theMaxHealth != Health2nd[ZombieObj.theSecondArmorType])
                    {
                        if (ZombieObj.theSecondArmorHealth == ZombieObj.theSecondArmorMaxHealth)
                        {
                            ZombieObj.theSecondArmorHealth = Health2nd[ZombieObj.theSecondArmorType];
                        }

                        ZombieObj.theSecondArmorMaxHealth = Health2nd[ZombieObj.theSecondArmorType];
                    }
                    ZombieObj.UpdateHealthText();
                }
                else
                {
                    DestroyImmediate(this, false);
                }
            }
            catch (Exception e) { Core.Instance.Value.LoggerInstance.Msg(e); }
        }

        public Zombie ZombieObj => gameObject.GetComponent<Zombie>();
    }
}