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
    [HarmonyPatch(typeof(Board), "Awake")]
    public static class BoardPatchA
    {
        public static void Postfix()
        {
            var t = Board.Instance.boardTag;
            originalTravel = t.enableTravelPlant;
            t.isScaredyDream |= PatchConfig.GameModes.ScaredyDream;
            t.isColumn |= PatchConfig.GameModes.ColumnPlanting;
            t.isSeedRain |= PatchConfig.GameModes.SeedRain;
            t.isShooting |= PatchConfig.GameModes.IsShooting();
            t.isExchange |= PatchConfig.GameModes.Exchange;
            t.enableTravelPlant |= UnlockAllFusions;
            Board.Instance.boardTag = t;
        }

        public static void Prefix()
        {
            originalLevel = GameAPP.theBoardLevel;
        }
    }

    [HarmonyPatch(typeof(Board), "NewZombieUpdate")]
    public static class BoardPatchB
    {
        public static void Postfix()
        {
            if (NewZombieUpdateCD > 0 && NewZombieUpdateCD < 30 && Board.Instance.newZombieWaveCountDown > NewZombieUpdateCD)
            {
                Board.Instance.newZombieWaveCountDown = NewZombieUpdateCD;
            }
        }
    }

    [HarmonyPatch(typeof(Board), "OnDestroy")]
    public static class BoardPatchC
    {
        public static void Prefix() => CardUIReplacer.Replacers.Clear();
    }

    [HarmonyPatch(typeof(Bucket), "Update")]
    public static class BucketPatch
    {
        public static void Postfix(Bucket __instance)
        {
            if (ItemExistForever) __instance.existTime = 0.1f;
        }
    }

    [HarmonyPatch(typeof(Bullet), "Die")]
    public static class BulletPatchB
    {
        public static bool Prefix(Bullet __instance)
        {
            if (UndeadBullet && !__instance.isZombieBullet)
            {
                __instance.hit = false;
                __instance.penetrationTimes = int.MaxValue;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(CardUI), "Awake")]
    public static class CardUIPatch
    {
        public static void Postfix(CardUI __instance) => CardUIReplacer.Replacers.Add(__instance.gameObject.GetOrAddComponent<CardUIReplacer>());
    }

    [HarmonyPatch(typeof(Chomper), "Update")]
    public static class ChomperPatch
    {
        public static void Prefix(Chomper __instance)
        {
            if (ChomperNoCD && __instance.attributeCountdown > 0.05f)
            {
                __instance.attributeCountdown = 0.05f;
            }
        }
    }

    [HarmonyPatch(typeof(CreateBullet), "SetBullet")]
    public static class CreateBulletPatch
    {
        public static void Postfix(Bullet __result)
        {
            if (BulletDamage[(BulletType)__result.theBulletType] >= 0)
            {
                __result.theBulletDamage = BulletDamage[(BulletType)__result.theBulletType];
            }
        }

        public static void Prefix(ref int theBulletType)
        {
            if (LockBulletType == -1)
            {
                theBulletType = (int)Enum.GetValues<BulletType>()[UnityEngine.Random.Range(0, Enum.GetValues<BulletType>().Length)];
            }
            if (LockBulletType >= 0)
            {
                theBulletType = LockBulletType;
            }
        }
    }

    /*
    [HarmonyPatch(typeof(CreatePlant), "Lim")]
    public static class CreatePlantPatchA
    {
        public static void Postfix(ref bool __result) => __result = !UnlockAllFusions && __result;
    }

    [HarmonyPatch(typeof(CreatePlant), "LimTravel")]
    public static class CreatePlantPatchB
    {
        public static void Postfix(ref bool __result) => __result = !UnlockAllFusions && __result;
    }
    */

    [HarmonyPatch(typeof(CreatePlant), "SetPlant")]
    public static class CreatePlantPatchC
    {
        public static void Prefix(ref bool isFreeSet) => isFreeSet = FreePlanting || isFreeSet;
    }

    [HarmonyPatch(typeof(CreateZombie), "SetZombie")]
    public static class CreateZombiePatch
    {
        public static void Postfix(GameObject __result)
        {
            if (GargantuarPatch.Flag)
            {
                __result.AddComponent<ImpZombie>();
            }
        }

        public static void Prefix(ref ZombieType theZombieType)
        {
            if (GargantuarPatch.Flag)
            {
                theZombieType = (ZombieType)ImpToBeThrown;
                MLogger.Msg(theZombieType.ToString());
            }
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

    [HarmonyPatch(typeof(DroppedCard), "Update")]
    public static class DroppedCardPatch
    {
        public static void Postfix(DroppedCard __instance)
        {
            if (ItemExistForever) __instance.existTime = 0;
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

    [HarmonyPatch(typeof(GameAPP), "Start")]
    public static class GameAppPatch
    {
        public static void Postfix()
        {
            GameObject obj = new("Modifier");
            UnityEngine.Object.DontDestroyOnLoad(obj);
            obj.AddComponent<DataProcessor>();
            obj.AddComponent<PatchConfig>();
        }
    }

    [HarmonyPatch(typeof(Gargantuar), "AnimThrow")]
    public static class GargantuarPatch
    {
        public static void Postfix() => Flag = false;

        public static void Prefix() => Flag = ImpToBeThrown is not 37;

        public static bool Flag { get; set; } = false;
    }

    [HarmonyPatch(typeof(GloveMgr), "Update")]
    public static class GloveMgrPatchA
    {
        public static void Postfix(GloveMgr __instance)
        {
            __instance.gameObject.transform.GetChild(0).gameObject.SetActive(!GloveNoCD);
            if (GloveFullCD > 0)
            {
                __instance.fullCD = (float)GloveFullCD;
            }
            if (GloveNoCD)
            {
                __instance.CD = __instance.fullCD;
            }
            if (__instance.avaliable || !ShowGameInfo)
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

    [HarmonyPatch(typeof(GridItem), "SetGridItem")]
    public static class GridItemPatch
    {
        public static bool Prefix(ref GridItemType theType) => (int)theType >= 3 || !NoHole;
    }

    [HarmonyPatch(typeof(HammerMgr), "Update")]
    public static class HammerMgrPatchA
    {
        public static void Postfix(HammerMgr __instance)
        {
            __instance.gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(!HammerNoCD);
            if (HammerFullCD > 0)
            {
                __instance.fullCD = (float)HammerFullCD;
            }
            else
            {
                __instance.fullCD = OriginalFullCD;
            }
            if (HammerNoCD)
            {
                __instance.CD = __instance.fullCD;
            }
            if (__instance.avaliable || !ShowGameInfo)
            {
                __instance.transform.FindChild("ModifierHammerCD").GameObject().active = false;
            }
            else
            {
                __instance.transform.FindChild("ModifierHammerCD").GameObject().active = true;
                __instance.transform.FindChild("ModifierHammerCD").GameObject().GetComponent<TextMeshProUGUI>().text = $"{__instance.CD:N1}/{__instance.fullCD}";
            }
        }

        public static float OriginalFullCD { get; set; }
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

    [HarmonyPatch(typeof(ImpZombie), "Thrown")]
    public static class ImpZombiePatch
    {
        public static bool Prefix(ImpZombie __instance)
        {
            __instance.theStatus = ZombieStatus.Default;
            UnityEngine.Object.DestroyImmediate(__instance);
            return false;
        }
    }

    [HarmonyPatch(typeof(InGameBtn), "OnMouseUpAsButton")]
    public static class InGameBtnPatch
    {
        public static void Postfix(InGameBtn __instance)
        {
            if (__instance.buttonNumber == 3)
            {
                TimeSlow = !TimeSlow;
                TimeStop = false;
                Time.timeScale = TimeSlow ? 0.2f : SyncSpeed;
            }
            if (__instance.buttonNumber == 13)
            {
                BottomEnabled = GameObject.Find("InGameUIFHD").GetComponent<InGameUIMgr>().Bottom.active;
            }
        }

        public static bool BottomEnabled { get; set; } = false;
    }

    [HarmonyPatch(typeof(InGameText), "EnableText")]
    public static class InGameTextPatch
    {
        public static void Postfix()
        {
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

    [HarmonyPatch(typeof(InGameUIMgr), "Start")]
    public static class InGameUIMgrPatch
    {
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

        public static void Prefix() => GameAPP.theBoardLevel = originalLevel;
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
                if (SeedGroup is not null)
                {
                    for (int i = SeedGroup!.transform.childCount - 1; i >= 0; i--)
                    {
                        var card = SeedGroup.transform.GetChild(i);
                        if (card is null || card.childCount is 0) continue;
                        card.GetChild(0).gameObject.GetComponent<CardUI>().CD = card.GetChild(0).gameObject.GetComponent<CardUI>().fullCD;
                    }
                }
            }
            HammerMgrPatchA.OriginalFullCD = UnityEngine.Object.FindObjectsOfTypeAll(Il2CppType.Of<HammerMgr>())[0].Cast<HammerMgr>().fullCD;
        }
    }

    [HarmonyPatch(typeof(InitBoard), "RightMoveCamera")]
    public static class InitBoardPatchB
    {
        public static void Postfix()
        {
            GameAPP.gameAPP.GetOrAddComponent<TravelMgr>();
            InGameAdvBuffs = new bool[TravelMgr.advancedBuffs.Count];
            InGameUltiBuffs = new bool[TravelMgr.ultimateBuffs.Count];
            InGameDebuffs = new bool[TravelMgr.debuffs.Count];
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
            var deb = GameAPP.gameAPP.GetOrAddComponent<TravelMgr>().debuff;
            for (int i = 0; i < deb.Count; i++)
            {
                if (GameAPP.theBoardType == 3 && Board.Instance.theCurrentSurvivalRound != 1) break;
                deb[i] = Debuffs[i] || deb[i];
            }
            InGameAdvBuffs = GameAPP.gameAPP.GetOrAddComponent<TravelMgr>().advancedUpgrades;
            InGameUltiBuffs = GameAPP.gameAPP.GetOrAddComponent<TravelMgr>().ultimateUpgrades;
            InGameDebuffs = GameAPP.gameAPP.GetOrAddComponent<TravelMgr>().debuff;
            ChangeCard();
            SyncInGameBuffs();
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

    [HarmonyPatch(typeof(Plant), "Start")]
    public static class PlantPatchA
    {
        public static void Postfix(Plant __instance)
        {
            if (HealthPlants[(PlantType)__instance.thePlantType] >= 0 && __instance.thePlantMaxHealth != HealthPlants[(PlantType)__instance.thePlantType])
            {
                __instance.thePlantMaxHealth = HealthPlants[(PlantType)__instance.thePlantType];
                __instance.thePlantHealth = __instance.thePlantMaxHealth;
                __instance.UpdateHealthText();
            }
        }
    }

    [HarmonyPatch(typeof(Plant), "PlantShootUpdate")]
    public static class PlantPatchB
    {
        public static void Prefix(Plant __instance)
        {
            var s = __instance.TryCast<Shooter>();
            if (FastShooting && s is not null)
            {
                s.AnimShoot();
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

    [HarmonyPatch(typeof(Plant), "Update")]
    public static class PlantPatchD
    {
        public static void Postfix(Plant __instance)
        {
            if (__instance.TryCast<CobCannon>() is not null && CobCannonNoCD
                && __instance.attributeCountdown > 0.1f)
            {
                __instance.attributeCountdown = 0.1f;
            }/*
            if (HealthPlants[(MixData.PlantType)__instance.thePlantType] >= 0 && __instance.thePlantMaxHealth != HealthPlants[(MixData.PlantType)__instance.thePlantType])
            {
                __instance.thePlantMaxHealth = HealthPlants[(MixData.PlantType)__instance.thePlantType];
                if (__instance.thePlantHealth >= __instance.thePlantMaxHealth) __instance.thePlantHealth = __instance.thePlantMaxHealth;
                __instance.UpdateHealthText();
            }*/
        }
    }

    [HarmonyPatch(typeof(PotatoMine), "Update")]
    public static class PotatoMinePatch
    {
        public static void Prefix(PotatoMine __instance)
        {
            if (MineNoCD && __instance.attributeCountdown > 0.05f)
            {
                __instance.attributeCountdown = 0.05f;
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
                CreatePlant.Instance.SetPlant(__instance.thePlantColumn, __instance.thePlantRow, (PlantType)LockPresent);
                if (CreatePlant.Instance.IsPuff((PlantType)LockPresent))
                {
                    CreatePlant.Instance.SetPlant(__instance.thePlantColumn, __instance.thePlantRow, (PlantType)LockPresent);
                    CreatePlant.Instance.SetPlant(__instance.thePlantColumn, __instance.thePlantRow, (PlantType)LockPresent);
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
            if (PresentFastOpen && (int)__instance.thePlantType != 245) __instance.AnimEvent();
        }
    }

    [HarmonyPatch(typeof(ProgressMgr), "Awake")]
    public static class ProgressMgrPatchA
    {
        public static void Postfix(ProgressMgr __instance)
        {
            GameObject obj = new("ModifierGameInfo");
            var text = obj.AddComponent<TextMeshProUGUI>();
            text.font = Resources.Load<TMP_FontAsset>("Fonts/ContinuumBold SDF");
            text.color = new(0, 1, 1);
            obj.transform.SetParent(__instance.GameObject().transform);
            obj.transform.localScale = new(0.4f, 0.2f, 0.2f);
            obj.transform.localPosition = new(100f, 2.2f, 0);
            obj.GetComponent<RectTransform>().sizeDelta = new(800, 50);
        }
    }

    [HarmonyPatch(typeof(ProgressMgr), "Update")]
    public static class ProgressMgrPatchB
    {
        public static void Postfix(ProgressMgr __instance)
        {
            if (ShowGameInfo)
            {
                __instance.transform.FindChild("ModifierGameInfo").GameObject().active = true;
                __instance.transform.FindChild("ModifierGameInfo").GameObject().GetComponent<TextMeshProUGUI>().text = $"波数: {Board.Instance.theWave}/{Board.Instance.theMaxWave} 刷新CD: {Board.Instance.newZombieWaveCountDown:N1}";
            }
            else
            {
                __instance.transform.FindChild("ModifierGameInfo").GameObject().active = false;
            }
        }
    }

    [HarmonyPatch(typeof(RandomZombie), "SetRandomZombie")]
    public static class RamdomZombiePatch
    {
        public static bool Prefix(RandomZombie __instance, ref GameObject __result)
        {
            if (!UltimateRamdomZombie) return true;
            if (Board.Instance is not null && Board.Instance.isEveStarted) return true;
            int id = UnityEngine.Random.RandomRangeInt(200, 223);
            if (UnityEngine.Random.RandomRangeInt(0, 5) == 1)
            {
                if (!__instance.isMindControlled)
                {
                    __result = CreateZombie.Instance.SetZombie(__instance.theZombieRow, (ZombieType)id, __instance.GameObject().transform.position.x);
                }
                else
                {
                    __result = CreateZombie.Instance.SetZombieWithMindControl(__instance.theZombieRow, (ZombieType)id, __instance.GameObject().transform.position.x);
                }
                return false;
            }
            else { return true; }
        }
    }

    [HarmonyPatch(typeof(Squalour), "LourDie")]
    public static class SqualourPatch
    {
        public static void Postfix() => GameAPP.developerMode = OriginalDevMode;

        public static void Prefix()
        {
            OriginalDevMode = GameAPP.developerMode;
            GameAPP.developerMode |= DevLour;
        }

        public static bool OriginalDevMode { get; set; } = false;
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
            obj.transform.localPosition = new(-345.5f, -42.6f, 0);
        }
    }

    [HarmonyPatch(typeof(Zombie), "Start")]
    public static class ZombiePatch
    {
        public static void Postfix(Zombie __instance)
        {
            if (HealthZombies[(ZombieType)__instance.theZombieType] >= 0)
            {
                __instance.theMaxHealth = HealthZombies[(ZombieType)__instance.theZombieType];
                __instance.theHealth = __instance.theMaxHealth;
            }
            if (Health1st[__instance.theFirstArmorType] >= 0 && __instance.theMaxHealth != Health1st[__instance.theFirstArmorType])
            {
                __instance.theFirstArmorMaxHealth = Health1st[__instance.theFirstArmorType];
                __instance.theFirstArmorHealth = __instance.theFirstArmorMaxHealth;
            }
            if (Health2nd[__instance.theSecondArmorType] >= 0 && __instance.theMaxHealth != Health2nd[__instance.theSecondArmorType])
            {
                __instance.theSecondArmorMaxHealth = Health2nd[__instance.theSecondArmorType];
                __instance.theSecondArmorHealth = __instance.theSecondArmorMaxHealth;
            }
            __instance.UpdateHealthText();
        }
    }

    [RegisterTypeInIl2Cpp]
    public class CardUIReplacer : MonoBehaviour
    {
        public CardUIReplacer() : base(ClassInjector.DerivedConstructorPointer<CardUIReplacer>()) => ClassInjector.DerivedConstructorBody(this);

        public CardUIReplacer(IntPtr i) : base(i)
        {
        }

        public static implicit operator int(CardUIReplacer r) => r.OriginalID;

        public void ChangeCard(int id, int cost, float cd)
        {
            Card.theSeedType = id >= 0 ? id : OriginalID;
            Card.theSeedCost = cost >= 0 ? cost : OriginalCost;
            if (cd >= 0.01)
            {
                Card.fullCD = cd;
            }
            else if (cd < 0.01 && cd >= 0)
            {
                Card.fullCD = 0.01f;
            }
            else
            {
                Card.fullCD = OriginalCD;
            }
            Lawnf.ChangeCardSprite((PlantType)Card.theSeedType, Card.gameObject);
        }

        public void Resume() => ChangeCard(-1, -1, -1);

        public void Start()
        {
            OriginalID = Card.theSeedType * 1;
            OriginalCost = Card.theSeedCost * 1;
            OriginalCD = Card.fullCD * 1;
            //Replacers.Add(this);
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
            if (gameObject.GetComponent<CardUI>().isAvailable || !ShowGameInfo)
            {
                gameObject.transform.FindChild("ModifierCardCD").GameObject().active = false;
            }
            else
            {
                gameObject.transform.FindChild("ModifierCardCD").GameObject().active = true;
                gameObject.transform.FindChild("ModifierCardCD").GameObject().GetComponent<TextMeshProUGUI>().text = $"{gameObject.GetComponent<CardUI>().CD:N1}/{gameObject.GetComponent<CardUI>().fullCD}";
            }
        }

        public static List<CardUIReplacer> Replacers { get; set; } = [];
        public CardUI Card => gameObject.GetComponent<CardUI>();
        public float OriginalCD { get; set; }
        public int OriginalCost { get; set; }
        public int OriginalID { get; set; }
    }

    [RegisterTypeInIl2Cpp]
    public class PatchConfig : MonoBehaviour
    {
        static PatchConfig()
        {
            foreach (var p in Enum.GetValues<PlantType>())
            {
                HealthPlants.Add(p, -1);
            }
            foreach (var z in Enum.GetValues<ZombieType>())
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
            foreach (var b in Enum.GetValues<BulletType>())
            {
                BulletDamage.Add(b, -1);
            }
        }

        public PatchConfig() : base(ClassInjector.DerivedConstructorPointer<PatchConfig>()) => ClassInjector.DerivedConstructorBody(this);

        public PatchConfig(IntPtr i) : base(i)
        {
        }

        public static void ChangeCard()
        {
            if (!InGame()) return;
            foreach (var (c, r) in from c in CardUIReplacer.Replacers
                                   where c is not null
                                   from r in CardReplaces
                                   where r.ID == c
                                   select (c, r))
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

        public static bool InGame() => Board.Instance is not null && GameAPP.theGameStatus != -2 && GameAPP.theGameStatus != -1 && GameAPP.theGameStatus != 4;

        public static void SyncInGameBuffs()
        {
            if (!InGame()) return;
            DataSync.Instance.Value.SendData(new SyncTravelBuff()
            {
                AdvInGame = [.. GameAPP.gameAPP.GetOrAddComponent<TravelMgr>().advancedUpgrades!],
                UltiInGame = [.. GameAPP.gameAPP.GetOrAddComponent<TravelMgr>().ultimateUpgrades!],
                DebuffsInGame = [.. GameAPP.gameAPP.GetOrAddComponent<TravelMgr>().debuff!]
            });
        }

        public static void Update()
        {
            if (GameAPP.theGameStatus is 0 or 2 or 3)
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
                    if (Input.GetKeyDown(KeyCode.BackQuote))
                    {
                        ShowGameInfo = !ShowGameInfo;
                    }
                    if (!TimeStop && !TimeSlow)
                    {
                        Time.timeScale = SyncSpeed;
                    }

                    if (!TimeStop && TimeSlow)
                    {
                        Time.timeScale = 0.2f;
                    }
                    if (InGameBtnPatch.BottomEnabled || (TimeStop && !TimeSlow))
                    {
                        Time.timeScale = 0;
                    }
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
                    var t = Board.Instance.boardTag;
                    t.enableTravelPlant = t.enableTravelPlant || UnlockAllFusions;
                    Board.Instance.boardTag = t;
                }
                catch (NullReferenceException) { }
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
                            CreateZombie.Instance.SetZombie(i, (ZombieType)j);
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
            for (int i = 0; i < GameAPP.gameAPP.GetOrAddComponent<TravelMgr>().debuff.Count; i++)
            {
                GameAPP.gameAPP.GetOrAddComponent<TravelMgr>().debuff![i] = InGameDebuffs[i];
            }
        }

        public static bool[] AdvBuffs { get; set; } = new bool[TravelMgr.advancedBuffs.Count];
        public static Dictionary<BulletType, int> BulletDamage { get; set; } = [];
        public static bool CardNoInit { get; set; } = false;
        public static List<ToolModData.Card> CardReplaces { get; set; } = [];
        public static bool ChomperNoCD { get; set; } = false;
        public static bool CobCannonNoCD { get; set; } = false;
        public static bool[] Debuffs { get; set; } = new bool[TravelMgr.debuffs.Count];
        public static bool DevLour { get; set; } = false;
        public static bool FastShooting { get; set; } = false;
        public static bool FreeCD { get; set; } = false;
        public static bool FreePlanting { get; set; } = false;
        public static GameModes GameModes { get; set; }
        public static bool GarlicDay { get; set; } = false;
        public static double GloveFullCD { get; set; } = 0;
        public static bool GloveNoCD { get; set; } = false;
        public static double HammerFullCD { get; set; } = 0;
        public static bool HammerNoCD { get; set; } = false;
        public static bool HardPlant { get; set; } = false;
        public static Dictionary<Zombie.FirstArmorType, int> Health1st { get; set; } = [];
        public static Dictionary<Zombie.SecondArmorType, int> Health2nd { get; set; } = [];
        public static Dictionary<PlantType, int> HealthPlants { get; set; } = [];
        public static Dictionary<ZombieType, int> HealthZombies { get; set; } = [];
        public static bool HyponoEmperorNoCD { get; set; } = false;
        public static int ImpToBeThrown { get; set; } = 37;
        public static bool[] InGameAdvBuffs { get; set; } = new bool[TravelMgr.advancedBuffs.Count];
        public static bool[] InGameDebuffs { get; set; } = new bool[TravelMgr.debuffs.Count];
        public static bool[] InGameUltiBuffs { get; set; } = new bool[TravelMgr.ultimateBuffs.Count];
        public static bool ItemExistForever { get; set; } = false;
        public static bool JackboxNotExplode { get; set; } = false;
        public static int LockBulletType { get; set; } = -2;
        public static bool LockMoney { get; set; } = false;
        public static int LockMoneyCount { get; set; } = 3000;
        public static int LockPresent { get; set; } = -1;
        public static bool LockSun { get; set; } = false;
        public static int LockSunCount { get; set; } = 500;
        public static bool MineNoCD { get; set; } = false;
        public static MelonLogger.Instance MLogger => Core.Instance.Value.LoggerInstance;
        public static float NewZombieUpdateCD { get; set; } = 30;
        public static bool NoHole { get; set; } = false;
        public static bool NoIceRoad { get; set; } = false;
        public static bool PresentFastOpen { get; set; } = false;
        public static List<int> SeaTypes { get; set; } = [];

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

        public static bool ShowGameInfo { get; set; } = false;
        public static bool StopSummon { get; set; } = false;
        public static bool SuperPresent { get; set; } = false;
        public static float SyncSpeed { get; set; } = -1;
        public static bool TimeSlow { get; set; } = false;
        public static bool TimeStop { get; set; } = false;
        public static bool[] UltiBuffs { get; set; } = new bool[TravelMgr.ultimateBuffs.Count];
        public static bool UltimateRamdomZombie { get; set; } = false;
        public static bool UndeadBullet { get; set; } = false;
        public static bool UnlockAllFusions { get; set; } = false;
        public static bool ZombieSea { get; set; } = false;
        public static int ZombieSeaCD { get; set; } = 40;
        internal static int originalLevel;
        internal static bool originalTravel;
        private static int garlicDayTime = 0;
        private static int seaTime = 0;
    }
}