using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Tools.Extension;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using ToolModData;

namespace PVZRHTools
{
    [Serializable]
    public partial class ModifierViewModel : ObservableObject
    {
        public ModifierViewModel()
        {
            Plants = new()
            {
                { -1, "不修改" }
            };
            Bullets2 = new()
            {
                {-2,"-2 : 不修改" },
                {-1,"-1 : 随机子弹" }
            };
            Health1st = new();
            Health2nd = new();
            HealthPlant = new();
            HealthZombie = new();
            foreach (var kp in App.InitData!.Value.Plants)
            {
                Plants.Add(kp.Key, kp.Value);
            }
            foreach (var h1 in App.InitData.Value.FirstArmors)
            {
                Health1st.Add(h1.Key, -1);
            }
            foreach (var h2 in App.InitData.Value.SecondArmors)
            {
                Health2nd.Add(h2.Key, -1);
            }
            foreach (var h3 in App.InitData.Value.Plants)
            {
                HealthPlant.Add(h3.Key, -1);
            }
            foreach (var h4 in App.InitData.Value.Zombies)
            {
                HealthZombie.Add(h4.Key, -1);
            }
            foreach (var b in Bullets)
            {
                Bullets2.Add(b.Key, b.Key.ToString() + " : " + b.Value);
            }
            SpeedText = "游戏速度：";
            Row = "0";
            Col = "0";
            Times = "1";
            IsMindCtrl = false;
            ZombieSeaCD = "40";
            ZombieSeaTypes = new();
            FieldString = "";
            ZombieFieldString = "";
            Sun = "";
            Money = "";
            LevelName = "";
            ShowText = "";
            Health1stValue = "";
            Health2ndValue = "";
            HealthPlantValue = "";
            HealthZombieValue = "";
            BulletDamageValue = "";
            BulletDamageType = -2;
            ZombieSeaTypes = new();
            DeveloperMode = new(Set_DeveloperMode);
            GloveNoCD = new(Set_GloveNoCD);
            HammerNoCD = new(Set_HammerNoCD);
            PlantingNoCD = new(Set_PlantingNoCD);
            FreePlanting = new(Set_FreePlanting);
            UnlockAllFusions = new(Set_UnlockAllFusions);
            SuperPresent = new(Set_SuperPresent);
            UltimateRamdomZombie = new(Set_UltimateRamdomZombie);
            PresentFastOpen = new(Set_PresentFastOpen);
            LockPresent = new(Set_LockPresent);
            FastShooting = new(Set_FastShooting);
            HardPlant = new(Set_HardPlant);
            NoHole = new(Set_NoHole);
            HyponoEmperorNoCD = new(Set_HyponoEmperorNoCD);
            MineNoCD = new(Set_MineNoCD);
            ChomperNoCD = new(Set_ChomperNoCD);
            CobCannonNoCD = new(Set_CobCannonNoCD);
            NoIceRoad = new(Set_NoIceRoad);
            ItemExistForever = new(Set_ItemExistForever);
            CardNoInit = new(Set_CardNoInit);
            JackboxNotExplode = new(Set_JackboxNotExplode);
            GameSpeed = new(Set_GameSpeed);
            CreatePlant = new(Set_CreatePlant);
            CreateZombie = new(Set_CreateZombie);
            CreateItem = new(Set_CreateItem);
            CreatePassiveMateorite = new(Set_CreatePassiveMateorite);
            CreateActiveMateorite = new(Set_CreateActiveMateorite);
            CreateUltimateMateorite = new(Set_CreateUltimateMateorite);
            SetZombieSea = new(Set_ZombieSea);
            SetSun = new(Set_Sun);
            SetMoney = new(Set_Money);
            LockSun = new(Set_LockSun);
            LockMoney = new(Set_LockMoney);
            NextWave = new(Set_NextWave);
            StopSummon = new(Set_StopSummon);
            NoFail = new(Set_NoFail);
            ClearIceRoads = new(Set_ClearIceRoads);
            ClearAllHoles = new(Set_ClearAllHoles);
            MindCtrlAll = new(Set_MindCtrl);
            Win = new(Set_Win);
            SetLevelName = new(Set_LevelName);
            ShowingText = new(Set_ShowingText);
            ClearFieldScripts = new(Set_ClearFieldScripts);
            WriteField = new(Set_WriteField);
            SimplePresents = new(Set_SimplePresents);
            ClearAllPlants = new(Set_ClearAllPlants);
            KillAllZombies = new(Set_KillAllZombies);
            CopyFieldScripts = new(Set_CopyFieldScripts);
            SetHealthPlant = new(Set_HealthPlant);
            SetHealthZombie = new(Set_HealthZombie);
            SetHealth1st = new(Set_Health1st);
            SetHealth2nd = new(Set_Health2nd);
            SetBulletDamage = new(Set_BulletDamage);
            SetLockBullet = new(Set_LockBullet);
            SetGameModes = new(Set_GameModes);
            CreateCard = new(Set_CreateCard);
            TravelBuffs = new();
            InGameBuffs = new();
            int bi = 0;
            foreach (var b in App.InitData.Value.AdvBuffs)
            {
                TravelBuffs.Add(new(new(bi, b, false)));
                InGameBuffs.Add(new(new(bi, b, true)));
                bi++;
            }
            foreach (var b in App.InitData.Value.UltiBuffs)
            {
                TravelBuffs.Add(new(new(bi, b, false)));
                InGameBuffs.Add(new(new(bi, b, true)));
                bi++;
            }
            TravelBuffs.ListChanged += (sender, e) => SyncTravelBuffs();
            InGameBuffs.ListChanged += (sender, e) => SyncInGameBuffs();
            CardReplaces = new();
            for (int i = 0; i < 14; i++)
            {
                CardReplaces.Add(new(new()));
            }
            CardReplaces.ListChanged += (sender, e) =>
            {
                MessageBox.Show("1");
                List<Card> cards = new();
                foreach (var c in MainWindow.Instance!.ViewModel.CardReplaces)
                {
                    cards.Add(c.CardUI.GetCard());
                }
                App.DataSync.Value.SendData(new CardProperties()
                {
                    CardReplaces = cards
                });
            };
        }

        [ObservableProperty]
        private string speedText;

        [ObservableProperty]
        private string row;

        [ObservableProperty]
        private string col;

        [ObservableProperty]
        private string times;

        [ObservableProperty]
        private int plantType;

        [ObservableProperty]
        private int zombieType;

        [ObservableProperty]
        private bool isMindCtrl;

        [ObservableProperty]
        private int itemType;

        [ObservableProperty]
        private bool zombieSea;

        [ObservableProperty]
        private string zombieSeaCD;

        [ObservableProperty]
        private List<KeyValuePair<int, string>> zombieSeaTypes;

        [ObservableProperty]
        private string fieldString;

        [ObservableProperty]
        private string zombieFieldString;

        [ObservableProperty]
        private string sun;

        [ObservableProperty]
        private string money;

        [ObservableProperty]
        private string levelName;

        [ObservableProperty]
        private string showText;

        [ObservableProperty]
        private bool clearOnWritingField;

        [ObservableProperty]
        private bool clearOnWritingZombies;

        [ObservableProperty]
        private int healthPlantType;

        [ObservableProperty]
        private int healthZombieType;

        [ObservableProperty]
        private int health1stType;

        [ObservableProperty]
        private int health2ndType;

        [ObservableProperty]
        private string healthPlantValue;

        [ObservableProperty]
        private string healthZombieValue;

        [ObservableProperty]
        private string health1stValue;

        [ObservableProperty]
        private string health2ndValue;

        [ObservableProperty]
        private int bulletDamageType;

        [ObservableProperty]
        private string bulletDamageValue;

        [ObservableProperty]
        private KeyValuePair<int, string> lockBullet;

        [ObservableProperty]
        private bool scaredyDream;

        [ObservableProperty]
        private bool columnPlanting;

        [ObservableProperty]
        private bool seedRain;

        [ObservableProperty]
        private bool exchange;

        [ObservableProperty]
        private bool shooting1;

        [ObservableProperty]
        private bool shooting2;

        [ObservableProperty]
        private bool shooting3;

        [ObservableProperty]
        private bool shooting4;

        [ObservableProperty]
        private bool freeCD;

        [ObservableProperty]
        private bool needSave;

        [ObservableProperty]
        private BindingList<TravelBuffVM> travelBuffs;

        [ObservableProperty]
        private BindingList<TravelBuffVM> inGameBuffs;

        [ObservableProperty]
        private BindingList<CardUIVM> cardReplaces;

        public Dictionary<int, int> HealthPlant { get; set; }
        public Dictionary<int, int> HealthZombie { get; set; }
        public Dictionary<int, int> Health1st { get; set; }
        public Dictionary<int, int> Health2nd { get; set; }
        public RelayCommand CreateCard { get; set; }
        public RelayCommand ClearFieldScripts { get; set; }
        public RelayCommand WriteField { get; set; }
        public RelayCommand CopyFieldScripts { get; set; }
        public RelayCommand SimplePresents { get; set; }
        public RelayCommand CreatePlant { get; set; }
        public RelayCommand CreateZombie { get; set; }
        public RelayCommand CreateItem { get; set; }
        public RelayCommand CreatePassiveMateorite { get; set; }
        public RelayCommand CreateActiveMateorite { get; set; }
        public RelayCommand CreateUltimateMateorite { get; set; }
        public RelayCommand SetZombieSea { get; set; }
        public RelayCommand SetSun { get; set; }
        public RelayCommand SetMoney { get; set; }
        public RelayCommand NextWave { get; set; }
        public RelayCommand ClearIceRoads { get; set; }
        public RelayCommand ClearAllHoles { get; set; }
        public RelayCommand MindCtrlAll { get; set; }
        public RelayCommand Win { get; set; }
        public RelayCommand SetLevelName { get; set; }
        public RelayCommand ShowingText { get; set; }
        public RelayCommand ClearAllPlants { get; set; }
        public RelayCommand KillAllZombies { get; set; }
        public RelayCommand SetHealthPlant { get; set; }
        public RelayCommand SetHealthZombie { get; set; }
        public RelayCommand SetHealth1st { get; set; }
        public RelayCommand SetHealth2nd { get; set; }
        public RelayCommand SetBulletDamage { get; set; }
        public RelayCommand SetLockBullet { get; set; }
        public RelayCommand SetGameModes { get; set; }
        public RelayCommand<bool> LockSun { get; set; }
        public RelayCommand<bool> LockMoney { get; set; }
        public RelayCommand<bool> DeveloperMode { get; set; }
        public RelayCommand<bool> GloveNoCD { get; set; }
        public RelayCommand<bool> HammerNoCD { get; set; }
        public RelayCommand<bool> PlantingNoCD { get; set; }
        public RelayCommand<bool> FreePlanting { get; set; }
        public RelayCommand<bool> UnlockAllFusions { get; set; }
        public RelayCommand<bool> SuperPresent { get; set; }
        public RelayCommand<bool> UltimateRamdomZombie { get; set; }
        public RelayCommand<bool> PresentFastOpen { get; set; }
        public RelayCommand<int> LockPresent { get; set; }
        public RelayCommand<bool> FastShooting { get; set; }
        public RelayCommand<bool> HardPlant { get; set; }
        public RelayCommand<bool> NoHole { get; set; }
        public RelayCommand<bool> HyponoEmperorNoCD { get; set; }
        public RelayCommand<bool> MineNoCD { get; set; }
        public RelayCommand<bool> ChomperNoCD { get; set; }
        public RelayCommand<bool> CobCannonNoCD { get; set; }
        public RelayCommand<bool> NoIceRoad { get; set; }
        public RelayCommand<bool> ItemExistForever { get; set; }
        public RelayCommand<bool> CardNoInit { get; set; }
        public RelayCommand<bool> JackboxNotExplode { get; set; }
        public RelayCommand<bool> StopSummon { get; set; }
        public RelayCommand<bool> NoFail { get; set; }
        public RelayCommand<double> GameSpeed { get; set; }
        public static Dictionary<int, string>? Plants { get; set; }

        [JsonIgnore]
        public Dictionary<int, string> Plants2 => App.InitData!.Value.Plants;

        [JsonIgnore]
        public Dictionary<int, string> Zombies => App.InitData!.Value.Zombies;

        [JsonIgnore]
        public Dictionary<int, string> Items => new()
        {
            {0, "肥料"},
            {1, "铁桶"},
            {2, "橄榄头盔"},
            {3, "小丑礼盒"},
            {4, "镐子"},
            {5, "机甲碎片"},
            {6, "超级机甲碎片" }
        };

        [JsonIgnore]
        public Dictionary<int, string> Bullets2 { get; set; }

        [JsonIgnore]
        public Dictionary<int, string> Bullets => App.InitData!.Value.Bullets;

        [JsonIgnore]
        public Dictionary<int, string> FirstArmor => App.InitData!.Value.FirstArmors;

        [JsonIgnore]
        public Dictionary<int, string> SecondArmor => App.InitData!.Value.SecondArmors;

        [JsonIgnore]
        public static bool NeedSync { get; set; } = true;

        [RelayCommand]
        public void TopMostSprite(bool b)
        {
            if (b)
            {
                MainWindow.Instance!.ModifierSprite.Show();
            }
            else
            {
                MainWindow.Instance!.ModifierSprite.Hide();
            }
        }

        [RelayCommand]
        public void GarlicDay(bool b)
        {
            App.DataSync.Value.SendData(new BasicProperties()
            {
                GarlicDay = b
            });
        }

        [RelayCommand]
        public void WriteZombies()
        {
            App.DataSync.Value.SendData(new InGameActions()
            {
                WriteZombies = ZombieFieldString,
                ClearOnWritingZombies = ClearOnWritingZombies
            });
        }

        [RelayCommand]
        public void CopyZombieScripts()
        {
            App.DataSync.Value.SendData(new InGameActions()
            {
                ReadZombies = true
            });
        }

        [RelayCommand]
        public void TravelBuffSelectAll()
        {
            NeedSync = false;
            foreach (var t in TravelBuffs)
            {
                t.Enabled = true;
            }
            NeedSync = true;
            SyncTravelBuffs();
        }

        [RelayCommand]
        public void TravelBuffUnselectAll()
        {
            NeedSync = false;
            foreach (var t in TravelBuffs)
            {
                t.Enabled = false;
            }
            NeedSync = true;
            SyncTravelBuffs();
        }

        [RelayCommand]
        public void InGameBuffSelectAll()
        {
            NeedSync = false;
            foreach (var t in InGameBuffs)
            {
                t.Enabled = true;
            }
            NeedSync = true;
            SyncInGameBuffs();
        }

        [RelayCommand]
        public void InGameBuffUnselectAll()
        {
            NeedSync = false;
            foreach (var t in InGameBuffs)
            {
                t.Enabled = false;
            }
            NeedSync = true;
            SyncInGameBuffs();
        }

        [RelayCommand]
        public void UndeadBullet(bool b)
        {
            App.DataSync.Value.SendData(new BasicProperties()
            {
                UndeadBullet = b
            });
        }

        public void SyncTravelBuffs()
        {
            if (!NeedSync) return;
            List<bool> adv = new();
            List<bool> ulti = new();
            foreach (TravelBuffVM buff in TravelBuffs)
            {
                if (buff.TravelBuff.Index < App.InitData!.Value.AdvBuffs.Length)
                {
                    adv.Add(buff.TravelBuff.Enabled);
                }
                else
                {
                    ulti.Add(buff.TravelBuff.Enabled);
                }
            }
            App.DataSync.Value.SendData(new SyncTravelBuff()
            {
                AdvTravelBuff = adv,
                UltiTravelBuff = ulti
            });
        }

        public void SyncInGameBuffs()
        {
            if (!NeedSync) return;
            List<bool> adv = new();
            List<bool> ulti = new();
            foreach (TravelBuffVM buff in InGameBuffs)
            {
                if (buff.TravelBuff.Index < App.InitData!.Value.AdvBuffs.Length)
                {
                    adv.Add(buff.TravelBuff.Enabled);
                }
                else
                {
                    ulti.Add(buff.TravelBuff.Enabled);
                }
            }
            App.DataSync.Value.SendData(new SyncTravelBuff()
            {
                AdvInGame = adv,
                UltiInGame = ulti
            });
        }

        public void Set_CreateCard()
        {
            App.DataSync.Value.SendData(new InGameActions()
            {
                Card = true,
                PlantType = PlantType
            });
        }

        public void Set_GameModes()
        {
            App.DataSync.Value.SendData(new GameModes()
            {
                Exchange = Exchange,
                ScaredyDream = ScaredyDream,
                ColumnPlanting = ColumnPlanting,
                SeedRain = SeedRain,
                Shooting1 = Shooting1,
                Shooting2 = Shooting2,
                Shooting3 = Shooting3,
                Shooting4 = Shooting4,
            });
        }

        public void Set_LockBullet()
        {
            App.DataSync.Value.SendData(new ValueProperties()
            {
                LockAllBullet = LockBullet.Key
            });
        }

        public void Set_BulletDamage()
        {
            if (int.TryParse(BulletDamageValue, out var v))
            {
                App.DataSync.Value.SendData(new ValueProperties()
                {
                    BulletsDamage = new(BulletDamageType, v)
                });
            }
        }

        public void Set_HealthPlant()
        {
            if (int.TryParse(HealthPlantValue, out var v))
            {
                App.DataSync.Value.SendData(new ValueProperties()
                {
                    PlantsHealth = new(HealthPlantType, v)
                });
            }
        }

        public void Set_HealthZombie()
        {
            if (int.TryParse(HealthZombieValue, out var v))
            {
                App.DataSync.Value.SendData(new ValueProperties()
                {
                    ZombiesHealth = new(HealthZombieType, v)
                });
            }
        }

        public void Set_Health1st()
        {
            if (int.TryParse(Health1stValue, out var v))
            {
                App.DataSync.Value.SendData(new ValueProperties()
                {
                    FirstArmorsHealth = new(Health1stType, v)
                });
            }
        }

        public void Set_Health2nd()
        {
            if (int.TryParse(Health2ndValue, out var v))
            {
                App.DataSync.Value.SendData(new ValueProperties()
                {
                    SecondArmorsHealth = new(Health2ndType, v)
                });
            }
        }

        public void Set_CopyFieldScripts()
        {
            App.DataSync.Value.SendData(new InGameActions()
            {
                ReadField = true
            });
        }

        public void Set_ClearAllPlants()
        {
            App.DataSync.Value.SendData(new InGameActions()
            {
                ClearAllPlants = true
            });
        }

        public void Set_KillAllZombies()
        {
            App.DataSync.Value.SendData(new InGameActions()
            {
                ClearAllZombies = true
            });
        }

        public void Set_SimplePresents()
        {
            App.DataSync.Value.SendData(new InGameActions()
            {
                WriteField = "[{\"ID\":256,\"Row\":2,\"Column\":0,\"LilyType\":-1},{\"ID\":256,\"Row\":2,\"Column\":1,\"LilyType\":-1},{\"ID\":256,\"Row\":2,\"Column\":2,\"LilyType\":-1},{\"ID\":256,\"Row\":2,\"Column\":3,\"LilyType\":-1},{\"ID\":256,\"Row\":2,\"Column\":4,\"LilyType\":-1},{\"ID\":250,\"Row\":2,\"Column\":5,\"LilyType\":-1},{\"ID\":250,\"Row\":2,\"Column\":6,\"LilyType\":-1},{\"ID\":250,\"Row\":2,\"Column\":7,\"LilyType\":-1},{\"ID\":250,\"Row\":2,\"Column\":8,\"LilyType\":-1},{\"ID\":250,\"Row\":2,\"Column\":9,\"LilyType\":-1}]",
                ClearOnWritingField = ClearOnWritingField
            });
        }

        public void Set_ClearFieldScripts()
        {
            FieldString = "";
        }

        public void Set_WriteField()
        {
            App.DataSync.Value.SendData(new InGameActions()
            {
                WriteField = FieldString,
                ClearOnWritingField = ClearOnWritingField
            });
        }

        public void Set_ClearAllHoles()
        {
            App.DataSync.Value.SendData(new InGameActions()
            {
                ClearAllHoles = true
            });
        }

        public void Set_MindCtrl()
        {
            App.DataSync.Value.SendData(new InGameActions()
            {
                MindControlAllZombies = true
            });
        }

        public void Set_Win()
        {
            App.DataSync.Value.SendData(new InGameActions()
            {
                Win = true
            });
        }

        public void Set_LevelName()
        {
            App.DataSync.Value.SendData(new InGameActions()
            {
                ChangeLevelName = LevelName
            });
        }

        public void Set_ShowingText()
        {
            App.DataSync.Value.SendData(new InGameActions()
            {
                ShowText = ShowText
            });
        }

        public void Set_ClearIceRoads()
        {
            App.DataSync.Value.SendData(new InGameActions()
            {
                ClearAllIceRoads = true
            });
        }

        public void Set_NoFail(bool b)
        {
            App.DataSync.Value.SendData(new InGameActions()
            {
                NoFail = b
            });
        }

        public void Set_StopSummon(bool b)
        {
            App.DataSync.Value.SendData(new InGameActions()
            {
                StopSummon = b
            });
        }

        public void Set_NextWave()
        {
            App.DataSync.Value.SendData(new InGameActions()
            {
                NextWave = true
            });
        }

        public void Set_LockSun(bool b)
        {
            if (string.IsNullOrEmpty(Sun)) return;
            App.DataSync.Value.SendData(new InGameActions()
            {
                LockSun = b,
                CurrentSun = int.Parse(Sun)
            });
        }

        public void Set_LockMoney(bool b)
        {
            if (string.IsNullOrEmpty(Money)) return;
            App.DataSync.Value.SendData(new InGameActions()
            {
                LockMoney = b,
                CurrentMoney = int.Parse(Money)
            });
        }

        public void Set_Sun()
        {
            if (string.IsNullOrEmpty(Sun)) return;
            App.DataSync.Value.SendData(new InGameActions()
            {
                CurrentSun = int.Parse(Sun)
            });
        }

        public void Set_Money()
        {
            if (string.IsNullOrEmpty(Money)) return;
            App.DataSync.Value.SendData(new InGameActions()
            {
                CurrentMoney = int.Parse(Money)
            });
        }

        public void Set_ZombieSea()
        {
            List<int> types = new();
            foreach (var type in ZombieSeaTypes)
            {
                types.Add(type.Key);
            }
            App.DataSync.Value.SendData(new InGameActions()
            {
                ZombieSea = ZombieSea,
                ZombieSeaCD = int.Parse(ZombieSeaCD),
                ZombieSeaTypes = types
            });
        }

        public void Set_CreatePassiveMateorite()
        {
            App.DataSync.Value.SendData(new InGameActions()
            {
                CreatePassiveMateorite = true
            });
        }

        public void Set_CreateUltimateMateorite()
        {
            App.DataSync.Value.SendData(new InGameActions()
            {
                CreateUltimateMateorite = true
            });
        }

        public void Set_CreateActiveMateorite()
        {
            App.DataSync.Value.SendData(new InGameActions()
            {
                CreateActiveMateorite = true
            });
        }

        public void Set_CreatePlant()
        {
            App.DataSync.Value.SendData(new InGameActions()
            {
                Row = string.IsNullOrEmpty(Row) || string.IsNullOrWhiteSpace(Row) ? 0 : int.Parse(Row),
                Column = string.IsNullOrEmpty(Col) || string.IsNullOrWhiteSpace(Col) ? 0 : int.Parse(Col),
                PlantType = PlantType,
                Times = string.IsNullOrEmpty(Times) || string.IsNullOrWhiteSpace(Times) ? 1 : int.Parse(Times),
            });
        }

        public void Set_CreateZombie()
        {
            App.DataSync.Value.SendData(new InGameActions()
            {
                Row = string.IsNullOrEmpty(Row) || string.IsNullOrWhiteSpace(Row) ? 0 : int.Parse(Row),
                Column = string.IsNullOrEmpty(Col) || string.IsNullOrWhiteSpace(Col) ? 0 : int.Parse(Col),
                ZombieType = ZombieType,
                SummonMindControlledZombies = IsMindCtrl,
                Times = string.IsNullOrEmpty(Times) || string.IsNullOrWhiteSpace(Times) ? 0 : int.Parse(Times),
            });
        }

        public void Set_CreateItem()
        {
            App.DataSync.Value.SendData(new InGameActions()
            {
                ItemType = ItemType,
            });
        }

        public void Set_GameSpeed(double b)
        {
            App.DataSync.Value.SendData(new SyncProperties()
            {
                GameSpeed = (int)b
            });
            SpeedText = "游戏速度：" + TickToSpeed((int)b).ToString();
        }

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

        public void Set_JackboxNotExplode(bool b) => App.DataSync.Value.SendData(new BasicProperties()
        {
            JackboxNotExplode = b
        });

        public void Set_CardNoInit(bool b) => App.DataSync.Value.SendData(new BasicProperties()
        {
            CardNoInit = b
        });

        public void Set_ItemExistForever(bool b) => App.DataSync.Value.SendData(new BasicProperties()
        {
            ItemExistForever = b
        });

        public void Set_NoIceRoad(bool b) => App.DataSync.Value.SendData(new BasicProperties()
        {
            NoIceRoad = b
        });

        public void Set_MineNoCD(bool b) => App.DataSync.Value.SendData(new BasicProperties()
        {
            MineNoCD = b
        });

        public void Set_ChomperNoCD(bool b) => App.DataSync.Value.SendData(new BasicProperties()
        {
            ChomperNoCD = b
        });

        public void Set_CobCannonNoCD(bool b) => App.DataSync.Value.SendData(new BasicProperties()
        {
            CobCannonNoCD = b
        });

        public void Set_HyponoEmperorNoCD(bool b) => App.DataSync.Value.SendData(new BasicProperties()
        {
            HyponoEmperorNoCD = b
        });

        public void Set_NoHole(bool b) => App.DataSync.Value.SendData(new BasicProperties()
        {
            NoHole = b
        });

        public void Set_DeveloperMode(bool b) => App.DataSync.Value.SendData(new BasicProperties()
        {
            DeveloperMode = b,
            PlantingNoCD = FreeCD
        });

        public void Set_GloveNoCD(bool b) => App.DataSync.Value.SendData(new BasicProperties()
        {
            GloveNoCD = b
        });

        public void Set_HammerNoCD(bool b) => App.DataSync.Value.SendData(new BasicProperties()
        {
            HammerNoCD = b
        });

        public void Set_PlantingNoCD(bool b) => App.DataSync.Value.SendData(new BasicProperties()
        {
            PlantingNoCD = b
        });

        public void Set_FreePlanting(bool b) => App.DataSync.Value.SendData(new BasicProperties()
        {
            FreePlanting = b
        });

        public void Set_UnlockAllFusions(bool b) => App.DataSync.Value.SendData(new BasicProperties()
        {
            UnlockAllFusions = b
        });

        public void Set_SuperPresent(bool b) => App.DataSync.Value.SendData(new BasicProperties()
        {
            SuperPresent = b
        });

        public void Set_UltimateRamdomZombie(bool b) => App.DataSync.Value.SendData(new BasicProperties()
        {
            UltimateRamdomZombie = b
        });

        public void Set_PresentFastOpen(bool b) => App.DataSync.Value.SendData(new BasicProperties()
        {
            PresentFastOpen = b
        });

        public void Set_LockPresent(int b) => App.DataSync.Value.SendData(new BasicProperties()
        {
            LockPresent = b
        });

        public void Set_FastShooting(bool b) => App.DataSync.Value.SendData(new BasicProperties()
        {
            FastShooting = b
        });

        public void Set_HardPlant(bool b) => App.DataSync.Value.SendData(new BasicProperties()
        {
            HardPlant = b
        });
    }

    public partial class TravelBuff : ObservableObject, INotifyPropertyChanged
    {
        public TravelBuff(int index, string text, bool inGame)
        {
            Text = text;
            Index = index;
            InGame = inGame;
        }

        [ObservableProperty]
        private bool enabled;

        public int Index { get; init; }

        [JsonIgnore]
        public string Text { get; set; }

        public bool InGame { get; init; }
    }

    public partial class TravelBuffVM : ObservableObject
    {
        public TravelBuffVM(TravelBuff b)
        {
            TravelBuff = b;
        }

        [ObservableProperty]
        private TravelBuff travelBuff;

        public bool Enabled
        {
            get => TravelBuff.Enabled;
            set
            {
                SetProperty(TravelBuff.Enabled, value, TravelBuff, (t, e) => t.Enabled = e);
                OnPropertyChanged(new PropertyChangedEventArgs("IsChecked"));
            }
        }
    }

    public partial class CardUIVM : ObservableObject
    {
        public CardUIVM(CardUI c)
        {
            CardUI = c;
        }

        [ObservableProperty]
        private CardUI cardUI;

        public bool Enabled
        {
            get => CardUI.SetEnabled;
            set
            {
                SetProperty(CardUI.SetEnabled, value, CardUI, (t, e) => t.SetEnabled = e);
                OnPropertyChanged(new PropertyChangedEventArgs("IsChecked"));
            }
        }
    }

    public partial class CardUI : ObservableObject
    {
        public CardUI()
        {
            SetEnabled = false;
        }

        [ObservableProperty]
        private bool setEnabled;

        partial void OnSetEnabledChanged(bool value)
        {
            List<Card> cards = new();
            foreach (var c in MainWindow.Instance!.ViewModel.CardReplaces)
            {
                cards.Add(c.CardUI.GetCard());
            }
            App.DataSync.Value.SendData(new CardProperties()
            {
                CardReplaces = cards
            });
        }

        public int ID { get; set; } = -1;
        public int NewID { get; set; } = -1;
        public int Sun { get; set; } = -1;
        public double CD { get; set; } = -1;

        public Card GetCard() => new()
        {
            ID = ID,
            NewID = NewID,
            Sun = Sun,
            CD = (float)CD,
            Enabled = SetEnabled
        };
    }
}