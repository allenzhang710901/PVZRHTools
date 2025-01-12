using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppTMPro;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using ToolModData;
using Unity.VisualScripting;
using UnityEngine;
using static ToolMod.PatchConfig;

namespace ToolMod
{
    [RegisterTypeInIl2Cpp]
    public class DataProcessor : MonoBehaviour
    {
        public DataProcessor() : base(ClassInjector.DerivedConstructorPointer<DataProcessor>()) => ClassInjector.DerivedConstructorBody(this);

        public DataProcessor(IntPtr i) : base(i)
        {
        }

        public void Update()
        {
            lock (Data)
            {
                if (!string.IsNullOrEmpty(Data))
                {
                    ProcessData(Data);
                }
                Data = "";
            }
        }

        private static string Data = "";

        public static void AddData(string d) => Data = d;

        public void ProcessData(string data)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data) || string.IsNullOrEmpty(data)) return;
                if (Core.Dev) MLogger.Msg(data);
                JsonObject json = JsonNode.Parse(data)!.AsObject();
                switch ((int)json["ID"]!)
                {
                    case 1:
                        {
                            ValueProperties v = JsonSerializer.Deserialize<ValueProperties>(json);
                            if (v.PlantsHealth is not null)
                            {
                                HealthPlants[(MixData.PlantType)v.PlantsHealth.Value.Key] = v.PlantsHealth.Value.Value;
                            }
                            if (v.ZombiesHealth is not null)
                            {
                                HealthZombies[(ZombietType)v.ZombiesHealth.Value.Key] = v.ZombiesHealth.Value.Value;
                            }
                            if (v.FirstArmorsHealth is not null)
                            {
                                Health1st[(Zombie.FirstArmorType)v.FirstArmorsHealth.Value.Key] = v.FirstArmorsHealth.Value.Value;
                            }
                            if (v.SecondArmorsHealth is not null)
                            {
                                Health2nd[(Zombie.SecondArmorType)v.SecondArmorsHealth.Value.Key] = v.SecondArmorsHealth.Value.Value;
                            }
                            if (v.BulletsDamage is not null)
                            {
                                BulletDamage[(CreateBullet.BulletType)v.BulletsDamage.Value.Key] = v.BulletsDamage.Value.Value;
                            }
                            if (v.LockAllBullet is not null)
                            {
                                LockBullet = (int)v.LockAllBullet;
                            }
                            break;
                        }
                    case 2:
                        {
                            BasicProperties p = JsonSerializer.Deserialize<BasicProperties>(json);
                            if (p.DeveloperMode is not null)
                            {
                                GameAPP.developerMode = (bool)p.DeveloperMode;
                            };
                            if (p.GloveNoCD is not null) GloveNoCD = (bool)p.GloveNoCD;
                            if (p.HammerNoCD is not null) HammerNoCD = (bool)p.HammerNoCD;
                            if (p.PlantingNoCD is not null && Board.Instance is not null)
                            {
                                FreeCD = (bool)p.PlantingNoCD;
                                Board.Instance.freeCD = FreeCD;
                            }
                            if (p.FreePlanting is not null) FreePlanting = (bool)p.FreePlanting;
                            if (p.UnlockAllFusions is not null)
                            {
                                UnlockAllFusions = (bool)p.UnlockAllFusions;
                                try
                                {
                                    _ = FindObjectsOfTypeAll(Il2CppType.Of<PresentCard>()).All((obj) =>
                                    {
                                        obj.TryCast<PresentCard>()!.gameObject.SetActive(UnlockAllFusions);
                                        return true;
                                    });
                                }
                                catch { }
                            }
                            if (p.SuperPresent is not null) SuperPresent = (bool)p.SuperPresent;
                            if (p.UltimateRamdomZombie is not null) UltimateRamdomZombie = (bool)p.UltimateRamdomZombie;
                            if (p.PresentFastOpen is not null) PresentFastOpen = (bool)p.PresentFastOpen;
                            if (p.LockPresent is not null) LockPresent = (int)p.LockPresent;
                            if (p.FastShooting is not null) FastShooting = (bool)p.FastShooting;
                            if (p.HardPlant is not null) HardPlant = (bool)p.HardPlant;
                            if (p.NoHole is not null) NoHole = (bool)p.NoHole;
                            if (p.HyponoEmperorNoCD is not null) HyponoEmperorNoCD = (bool)p.HyponoEmperorNoCD;
                            if (p.MineNoCD is not null) MineNoCD = (bool)p.MineNoCD;
                            if (p.ChomperNoCD is not null) ChomperNoCD = (bool)p.ChomperNoCD;
                            if (p.CobCannonNoCD is not null) CobCannonNoCD = (bool)p.CobCannonNoCD;
                            if (p.NoIceRoad is not null) NoIceRoad = (bool)p.NoIceRoad;
                            if (p.ItemExistForever is not null) ItemExistForever = (bool)p.ItemExistForever;
                            if (p.CardNoInit is not null) CardNoInit = (bool)p.CardNoInit;
                            if (p.JackboxNotExplode is not null) JackboxNotExplode = (bool)p.JackboxNotExplode;
                            if (p.UndeadBullet is not null) UndeadBullet = (bool)p.UndeadBullet;
                            if (p.GarlicDay is not null) GarlicDay = (bool)p.GarlicDay;
                            break;
                        }
                    case 3:
                        {
                            SyncProperties p = JsonSerializer.Deserialize<SyncProperties>(json);
                            SyncSpeed = TickToSpeed(p.GameSpeed);
                            break;
                        }
                    case 4:
                        {
                            SyncTravelBuff s = JsonSerializer.Deserialize<SyncTravelBuff>(json);

                            if (s.AdvTravelBuff is not null)
                            {
                                AdvBuffs = [.. s.AdvTravelBuff];
                            }
                            if (s.UltiTravelBuff is not null)
                            {
                                UltiBuffs = [.. s.UltiTravelBuff];
                            }
                            if (InGame())
                            {
                                if (s.AdvInGame is not null)
                                {
                                    InGameAdvBuffs = s.AdvInGame.ToArray();
                                }
                                if (s.UltiInGame is not null)
                                {
                                    InGameUltiBuffs = s.UltiInGame.ToArray();
                                }
                                UpdateInGameBuffs();
                            }
                            break;
                        }
                    case 5:
                        {
                            CardProperties c = JsonSerializer.Deserialize<CardProperties>(json);
                            if (c.CardReplaces is not null)
                            {
                                CardReplaces = new(c.CardReplaces);
                                ChangeCard();
                            }
                            break;
                        }
                    case 6:
                        {
                            if (!InGame()) break;
                            InGameActions iga = JsonSerializer.Deserialize<InGameActions>(json);
                            //Set_CreatePlant
                            if (iga.Row is not null
                              && iga.Column is not null
                              && iga.PlantType is not null
                              && iga.Times is not null)
                            {
                                int id = (int)iga.PlantType;
                                int r = (int)iga.Row;
                                int c = (int)iga.Column;
                                if (iga.Times > 50) iga.Times = 50;

                                for (int n = 0; n < iga.Times; n++)
                                {
                                    if (r * r + c * c == 0)
                                    {
                                        for (int i = 0; i < Board.Instance.rowNum; i++)
                                        {
                                            for (int j = 0; j < Board.Instance.columnNum; j++)
                                            {
                                                CreatePlant.Instance.SetPlant(j, i, id);
                                            }
                                        }
                                        continue;
                                    };
                                    if (r == 0 && c != 0)
                                    {
                                        for (int j = 0; j < Board.Instance.columnNum; j++)
                                        {
                                            CreatePlant.Instance.SetPlant(c - 1, j, id);
                                        }
                                        continue;
                                    }
                                    if (c == 0 && r != 0)
                                    {
                                        for (int j = 0; j < Board.Instance.columnNum; j++)
                                        {
                                            CreatePlant.Instance.SetPlant(j, r - 1, id);
                                        }
                                        continue;
                                    }
                                    if (c > 0 && r > 0 && c <= Board.Instance.columnNum && r <= Board.Instance.rowNum)
                                    {
                                        CreatePlant.Instance.SetPlant(c - 1, r - 1, id);
                                    }
                                    continue;
                                }
                            }
                            //Set_CreateZombie
                            if (iga.Row is not null
                              && iga.Column is not null
                              && iga.ZombieType is not null
                              && iga.SummonMindControlledZombies is not null
                              && iga.Times is not null)
                            {
                                if (iga.Times > 50) iga.Times = 50;
                                for (int n = 0; n < iga.Times; n++)
                                {
                                    int id = (int)iga.ZombieType;
                                    int r = (int)iga.Row;
                                    int c = (int)iga.Column;
                                    if (r * r + c * c == 0)
                                    {
                                        for (int i = 0; i < Board.Instance.rowNum; i++)
                                        {
                                            for (int j = 0; j < Board.Instance.columnNum; j++)
                                            {
                                                if (!(bool)iga.SummonMindControlledZombies)
                                                {
                                                    CreateZombie.Instance.SetZombie(i, id, -5f + (j) * 1.37f);
                                                }
                                                else
                                                {
                                                    CreateZombie.Instance.SetZombieWithMindControl(i, id, -5f + (j) * 1.37f);
                                                }
                                            }
                                        }
                                        continue;
                                    };
                                    if (r == 0 && c != 0)
                                    {
                                        for (int j = 0; j < Board.Instance.rowNum; j++)
                                        {
                                            if (!(bool)iga.SummonMindControlledZombies)
                                            {
                                                CreateZombie.Instance.SetZombie(j, id, -5f + (c - 1) * 1.37f);
                                            }
                                            else
                                            {
                                                CreateZombie.Instance.SetZombieWithMindControl(j, id, -5f + (c - 1) * 1.37f);
                                            }
                                        }
                                        continue;
                                    }
                                    if (c == 0 && r != 0)
                                    {
                                        for (int j = 0; j < Board.Instance.columnNum; j++)
                                        {
                                            if (!(bool)iga.SummonMindControlledZombies)
                                            {
                                                CreateZombie.Instance.SetZombie(r - 1, id, -5f + (j) * 1.37f);
                                            }
                                            else
                                            {
                                                CreateZombie.Instance.SetZombieWithMindControl(r - 1, id, -5f + (j) * 1.37f);
                                            }
                                        }
                                        continue;
                                    }
                                    if (c > 0 && r > 0 && c <= Board.Instance.columnNum + 1 && r <= Board.Instance.rowNum)
                                    {
                                        if (!(bool)iga.SummonMindControlledZombies)
                                        {
                                            CreateZombie.Instance.SetZombie(r - 1, id, -5f + (c - 1) * 1.37f);
                                        }
                                        else
                                        {
                                            CreateZombie.Instance.SetZombieWithMindControl(r - 1, id, -5f + (c - 1) * 1.37f);
                                        }
                                        continue;
                                    }
                                }
                            }
                            //Set_CreateItem
                            if (iga.ItemType is not null)
                            {
                                if (!InGame()) return;
                                Instantiate(Items[(int)iga.ItemType]).transform.SetParent(GameAPP.board.transform);
                            }
                            //Set_CreatePassiveMateorite
                            if (iga.CreatePassiveMateorite is not null)
                            {
                                Board.Instance.CreatePassiveMateorite();
                            }
                            //Set_CreateActiveMateorite
                            if (iga.CreateActiveMateorite is not null)
                            {
                                Board.Instance.CreateActiveMateorite();
                            }
                            //Set_CreateUltimateMateorite
                            if (iga.CreateUltimateMateorite is not null)
                            {
                                Board.Instance.CreateUltimateMateorite();
                            }
                            //Set_Sun
                            if (iga.CurrentSun is not null)
                            {
                                if (!InGame()) break;
                                Board.Instance.theSun = (int)iga.CurrentSun;
                            }
                            //Set_Money
                            if (iga.CurrentMoney is not null)
                            {
                                if (!InGame()) break;
                                Board.Instance.theMoney = (int)iga.CurrentMoney;
                            }
                            //Set_LockSun
                            if (iga.LockSun is not null
                              && iga.CurrentSun is not null)
                            {
                                if (!InGame()) break;
                                LockSun = (bool)iga.LockSun;
                                LockSunCount = (int)iga.CurrentSun;
                            }
                            //Set_LockMoney
                            if (iga.LockMoney is not null
                              && iga.CurrentMoney is not null)
                            {
                                if (!InGame()) break;
                                LockMoney = (bool)iga.LockMoney;
                                LockMoneyCount = (int)iga.CurrentMoney;
                            }
                            //Set_ClearAllPlants
                            if (iga.ClearAllPlants is not null)
                            {
                                if (!InGame()) break;
                                for (int i = Board.Instance.plantArray.Count - 1; i >= 0; i--)
                                {
                                    Board.Instance.plantArray[i]?.Die();
                                }
                                Board.Instance.plantArray.Clear();
                            }
                            //Set_KillAllZombies
                            if (iga.ClearAllZombies is not null)
                            {
                                if (!InGame()) break;
                                Il2CppReferenceArray<UnityEngine.Object> zombies = FindObjectsOfTypeAll(Il2CppType.Of<Zombie>());
                                for (int i = zombies.Count - 1; i >= 0; i--)
                                {
                                    try { ((Zombie)zombies[i])?.Die(); } catch { }
                                }
                                Board.Instance.zombieArray.Clear();
                            }
                            //Set_ClearAllHoles
                            if (iga.ClearAllHoles is not null)
                            {
                                if (!InGame()) break;

                                for (int i = Board.Instance.griditemArray.Count - 1; i >= 0; i--)
                                {
                                    Destroy(Board.Instance.griditemArray[i].GameObject());
                                }
                                Board.Instance.griditemArray.Clear();
                            }
                            //Set_MindCtrl
                            if (iga.MindControlAllZombies is not null)
                            {
                                Board.Instance.zombieArray?.ForEach((Il2CppSystem.Action<Zombie>)(zombie => zombie?.SetMindControl()));
                            }
                            //Set_Win
                            if (iga.Win is not null)
                            {
                                Destroy(GameAPP.board);
                                UIMgr.EnterMainMenu();
                            }
                            //Set_LevelName
                            if (iga.ChangeLevelName is not null)
                            {
                                if (!InGame()) return;
                                var uimgr = GameObject.Find("InGameUIFHD").GetComponent<InGameUIMgr>();
                                uimgr.ChangeString(new([
                                    uimgr.LevelName1.GetComponent<TextMeshProUGUI>(),
                                    uimgr.LevelName2.GetComponent<TextMeshProUGUI>(),
                                    uimgr.LevelName3.GetComponent<TextMeshProUGUI>(),
                                    uimgr.LevelName1.transform.GetChild(0).GameObject().GetComponent<TextMeshProUGUI>(),
                                    uimgr.LevelName2.transform.GetChild(0).GameObject().GetComponent<TextMeshProUGUI>(),
                                    uimgr.LevelName3.transform.GetChild(0).GameObject().GetComponent<TextMeshProUGUI>(),
                                    ]), iga.ChangeLevelName);
                            }
                            //Set_ShowingText
                            if (iga.ShowText is not null)
                            {
                                try
                                {
                                    GameObject.Find("Tutor").GetComponent<InGameText>().EnableText(iga.ShowText, 5);
                                }
                                catch (NullReferenceException e) { MLogger.Msg(e.ToString()); }
                            }
                            //Set_ClearIceRoads
                            if (iga.ClearAllIceRoads is not null)
                            {
                                for (int i = 0; i < Board.Instance.iceRoadFadeTime.Count; i++)
                                {
                                    Board.Instance.iceRoadFadeTime[i] = 0f;
                                }
                            }
                            //Set_NoFail
                            if (iga.NoFail is not null)
                            {
                                EnableAll<GameLose>(!(bool)iga.NoFail);
                            }
                            //Set_StopSummon
                            if (iga.StopSummon is not null)
                            {
                                StopSummon = (bool)iga.StopSummon;
                            }
                            //Set_NextWave
                            if (iga.NextWave is not null)
                            {
                                Board.Instance.newZombieWaveCountDown = 0;
                            }
                            //Set_ZombieSea
                            if (iga.ZombieSea is not null
                              && iga.ZombieSeaCD is not null
                              && iga.ZombieSeaTypes is not null)
                            {
                                ZombieSea = (bool)iga.ZombieSea;
                                ZombieSeaCD = (int)iga.ZombieSeaCD;
                                SeaTypes = iga.ZombieSeaTypes;
                            }
                            //Set_WriteField
                            if (iga.WriteField is not null
                                && iga.ClearOnWritingField is not null)
                            {
                                try
                                {
                                    var plants = JsonSerializer.Deserialize<List<PlantInfo>>(iga.WriteField);
                                    if (plants is null) return;

                                    if ((bool)iga.ClearOnWritingField)
                                    {
                                        for (int i = Board.Instance.plantArray.Count - 1; i >= 0; i--)
                                        {
                                            Board.Instance.plantArray[i]?.Die();
                                        }
                                        Board.Instance.plantArray.Clear();
                                    }

                                    foreach (var plant in plants)
                                    {
                                        var p = CreatePlant.Instance.SetPlant(plant.Column, plant.Row, plant.ID);
                                        if (p is null) continue;
                                        if (p.GetComponent<Plant>().isLily) p.GetComponent<Plant>().theLilyType = plant.LilyType;
                                    }
                                }
                                catch (JsonException) { MLogger.Error("布阵代码存在错误！"); }
                                catch (NotSupportedException) { MLogger.Error("布阵代码存在错误！"); }
                            }
                            //Set_CopyFieldScripts
                            if (iga.ReadField is not null)
                            {
                                List<PlantInfo> bases = [];
                                List<PlantInfo> plants = [];
                                foreach (var plant in Board.Instance.plantArray)
                                {
                                    if (plant is null) continue;
                                    if (plant.plantTag.potPlant || plant.isLily)
                                    {
                                        bases.Add(new()
                                        {
                                            ID = plant.thePlantType,
                                            Row = plant.thePlantRow,
                                            Column = plant.thePlantColumn,
                                            LilyType = plant.theLilyType
                                        });
                                        continue;
                                    }
                                    plants.Add(new()
                                    {
                                        ID = plant.thePlantType,
                                        Row = plant.thePlantRow,
                                        Column = plant.thePlantColumn,
                                        LilyType = plant.theLilyType
                                    });
                                }
                                bases.AddRange(plants);
                                DataSync.Instance.Value.SendData(new InGameActions()
                                {
                                    WriteField = JsonSerializer.Serialize(bases)
                                });
                            }
                            //
                            if (iga.Card is not null
                              && iga.PlantType is not null)
                            {
                                Lawnf.SetDroppedCard(new(0f, 0f), (int)iga.PlantType).GameObject().transform.SetParent(GameObject.Find("InGameUIFHD").transform);
                            }
                            if (iga.ReadZombies is not null)
                            {
                                List<ZombieInfo> zombies = [];
                                foreach (Zombie zombie in Board.Instance.zombieArray!)
                                {
                                    if (zombie is not null && zombie.gameObject is not null && !zombie.isMindControlled)
                                    {
                                        zombies.Add(new()
                                        {
                                            ID = zombie.theZombieType,
                                            X = zombie.gameObject.transform.position.x,
                                            Row = zombie.theZombieRow
                                        });
                                    }
                                }
                                DataSync.Instance.Value.SendData(new InGameActions()
                                {
                                    WriteZombies = JsonSerializer.Serialize(zombies)
                                });
                            }
                            if (iga.WriteZombies is not null
                                && iga.ClearOnWritingZombies is not null)
                            {
                                try
                                {
                                    var fieldZombies = JsonSerializer.Deserialize<List<ZombieInfo>>(iga.WriteZombies);
                                    if (fieldZombies is null) return;
                                    if ((bool)iga.ClearOnWritingZombies)
                                    {
                                        Il2CppReferenceArray<UnityEngine.Object> zombies = FindObjectsOfTypeAll(Il2CppType.Of<Zombie>());
                                        for (int i = zombies.Count - 1; i >= 0; i--)
                                        {
                                            try { ((Zombie)zombies[i])?.Die(); } catch { }
                                        }
                                        Board.Instance.zombieArray!.Clear();
                                    }
                                    foreach (var z in fieldZombies)
                                    {
                                        CreateZombie.Instance.SetZombie(z.Row, z.ID, z.X);
                                    }
                                }
                                catch (JsonException) { MLogger.Error("布阵代码存在错误！"); }
                                catch (NotSupportedException) { MLogger.Error("布阵代码存在错误！"); }
                            }
                            break;
                        }
                    case 7:
                        {
                            GameModes g = JsonSerializer.Deserialize<GameModes>(json);
                            PatchConfig.GameModes = g;
                            if (InGame())
                            {
                                originalLevel = GameAPP.theBoardLevel;
                                var t = Board.Instance.boardTag;
                                t.isScaredyDream = PatchConfig.GameModes.ScaredyDream;
                                t.isColumn = PatchConfig.GameModes.ColumnPlanting;
                                t.isSeedRain = PatchConfig.GameModes.SeedRain;
                                t.isShooting = PatchConfig.GameModes.IsShooting();
                                t.isExchange = PatchConfig.GameModes.Exchange;
                                Board.Instance.boardTag = t;
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

                            break;
                        }
                    case 8:
                        {
                            break;
                        }
                    case 16:
                        {
                            Application.Quit();
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Core.Instance.Value.LoggerInstance.Error(ex.ToString() + ex.StackTrace);
            }
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

        public static float TickToSpeed(int speed) => speed switch
        {
            0 => 0f,
            1 => 0.2f,
            2 => 0.5f,
            3 => 1f,
            4 => 2f,
            5 => 3f,
            6 => 4f,
            7 => 6f,
            8 => 8f,
            9 => 16f,
            _ => 1f
        };

        public static List<GameObject> Items =>
        [
            Resources.Load<GameObject>("Items/Fertilize/Ferilize"),
            Resources.Load<GameObject>("Items/Bucket"),
            Resources.Load<GameObject>("Items/Helmet"),
            Resources.Load<GameObject>("Items/Jackbox"),
            Resources.Load<GameObject>("Items/Pickaxe"),
            Resources.Load<GameObject>("Items/Machine"),
            Resources.Load<GameObject>("Items/SuperMachine"),
        ];

        protected static void EnableAll<T>(bool enabled) where T : Component
        {
            _ = FindObjectsOfTypeAll(Il2CppType.Of<T>()).All((c) =>
            {
                var v = ((c?.Cast<T>())?.gameObject.GetComponent<BoxCollider2D>());
                var v2 = ((c?.Cast<T>())?.gameObject.GetComponent<PolygonCollider2D>());
                if (v is not null) v.enabled = enabled;
                if (v2 is not null) v2.enabled = enabled;
                return true;
            });
        }
    }
}