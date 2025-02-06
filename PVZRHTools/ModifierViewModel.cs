using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FastHotKeyForWPF;
using HandyControl.Tools.Extension;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Input;
using Tommy;
using ToolModData;

namespace PVZRHTools
{
    public class CardUI
    {
        public CardUI()
        {
            SetEnabled = false;
        }

        public Card GetCard() => new()
        {
            ID = ID,
            NewID = NewID,
            Sun = Sun,
            CD = (float)CD,
            Enabled = SetEnabled
        };

        public double CD { get; set; } = -1;

        public int ID { get; set; } = -1;

        public int NewID { get; set; } = -1;

        public bool SetEnabled
        {
            get; set
            {
                field = value;
                if (App.inited)
                    App.DataSync.Value.SendData(new CardProperties() { CardReplaces = [.. from c in MainWindow.Instance!.ViewModel.CardReplaces select c.CardUI.GetCard()] });
            }
        }

        public int Sun { get; set; } = -1;
    }

    public partial class CardUIVM(CardUI CardUI) : ObservableObject
    {
        [ObservableProperty]
        public partial CardUI CardUI { get; set; } = CardUI;

        public double CD { get => CardUI.CD; set => SetProperty(CardUI.CD, value, CardUI, (t, e) => t.CD = e); }

        public RelayCommand Clear => new(() =>
        {
            (Enabled, ID, NewID, CD, Sun) = (false, -1, -1, -1, -1);
            OnPropertyChanged(new PropertyChangedEventArgs("IsChecked"));
            OnPropertyChanged(new PropertyChangedEventArgs("IsEnabled"));
            OnPropertyChanged(new PropertyChangedEventArgs("SelectedValue"));
            OnPropertyChanged(new PropertyChangedEventArgs("Value"));
        });

        public bool Enabled
        {
            get => CardUI.SetEnabled;
            set => SetProperty(CardUI.SetEnabled, value, CardUI, (t, e) => t.SetEnabled = e);
        }

        public int ID { get => CardUI.ID; set => SetProperty(CardUI.ID, value, CardUI, (t, e) => t.ID = e); }

        public int NewID { get => CardUI.NewID; set => SetProperty(CardUI.NewID, value, CardUI, (t, e) => t.NewID = e); }

        public int Sun { get => CardUI.Sun; set => SetProperty(CardUI.Sun, value, CardUI, (t, e) => t.Sun = e); }
    }

    [Serializable]
    public partial class HotkeyUI : IAutoHotKeyProperty
    {
        public event HotKeyEventHandler? Handler;

        public uint CurrentKeyA { get; set; }

        public Key CurrentKeyB { get; set; }

        [JsonIgnore]
        public int PoolID { get; set; }
    }

    [Serializable]
    public partial class HotkeyUIVM : ObservableObject, IAutoHotKeyUpdate, IAutoHotKeyProperty
    {
        public HotkeyUIVM(HotkeyUI HotkeyUI)
        {
            this.HotkeyUI = HotkeyUI;
            Clear = new(() => (CurrentKeyA, CurrentKeyB) = (0, 0));
        }

        public event HotKeyEventHandler? Handler;

        public void RemoveSame()
        {
        }

        public void UpdateHotKey()
        {
            GlobalHotKey.Add(CurrentKeyA, KeyHelper.KeyToNormalKeys[CurrentKeyB], (_, _) => Command!.Execute(null));
        }

        public void UpdateText()
        {
        }

        [JsonIgnore]
        public RelayCommand Clear { get; init; }

        [JsonIgnore]
        public RelayCommand? Command { get; set; }

        public uint CurrentKeyA
        {
            get => HotkeyUI.CurrentKeyA;
            set => SetProperty(HotkeyUI.CurrentKeyA, value, HotkeyUI, (t, e) => t.CurrentKeyA = e);
        }

        public Key CurrentKeyB
        {
            get => HotkeyUI.CurrentKeyB;
            set => SetProperty(HotkeyUI.CurrentKeyB, value, HotkeyUI, (t, e) => t.CurrentKeyB = e);
        }

        [ObservableProperty]
        public partial HotkeyUI HotkeyUI { get; set; }

        [JsonIgnore]
        public int PoolID { get => HotkeyUI.PoolID; set => HotkeyUI.PoolID = value; }

        [JsonIgnore]
        public string Text { get; init; } = "";
    }

    public partial class InGameHotkeyUI(string text, KeyCode code) : ObservableObject
    {
        [ObservableProperty]
        public partial KeyCode KeyCode { get; set; } = code;

        public string KeyText { get; set; } = text;
    }

    public partial class InGameHotkeyUIVM(InGameHotkeyUI InGameHotkeyUI) : ObservableObject
    {
        [ObservableProperty]
        public partial InGameHotkeyUI InGameHotkeyUI { get; set; } = InGameHotkeyUI;

        public KeyCode KeyCode
        {
            get => InGameHotkeyUI.KeyCode;
            set
            {
                SetProperty(InGameHotkeyUI.KeyCode, value, InGameHotkeyUI, (t, e) => t.KeyCode = e);
            }
        }
    }

    public partial class ModifierViewModel : ObservableObject
    {
        public ModifierViewModel()
        {
            Plants = new()
            {
                { -1, "-1 : 不修改" }
            };
            Bullets2 = new()
            {
                {-2,"-2 : 不修改" },
                {-1,"-1 : 随机子弹" }
            };
            Health1sts = [];
            Health2nds = [];
            HealthPlants = [];
            HealthZombies = [];
            foreach (var kp in App.InitData!.Value.Plants)
            {
                Plants.Add(kp.Key, kp.Value);
            }
            foreach (var h1 in App.InitData.Value.FirstArmors)
            {
                Health1sts.Add(h1.Key, -1);
            }
            foreach (var h2 in App.InitData.Value.SecondArmors)
            {
                Health2nds.Add(h2.Key, -1);
            }
            foreach (var h3 in App.InitData.Value.Plants)
            {
                HealthPlants.Add(h3.Key, -1);
            }
            foreach (var h4 in App.InitData.Value.Zombies)
            {
                HealthZombies.Add(h4.Key, -1);
            }
            foreach (var b in Bullets)
            {
                Bullets2.Add(b.Key, b.Key.ToString() + " : " + b.Value);
            }
            GameSpeed = 1;
            ZombieSeaCD = 40;
            ZombieSeaTypes = [];
            FieldString = "";
            ZombieFieldString = "";
            NewLevelName = "";
            ShowText = "";
            BulletDamageType = 0;
            LockPresent = -1;
            LockBulletType = -2;
            ZombieSeaTypes = [];
            TravelBuffs = [];
            InGameBuffs = [];
            Debuffs = [];
            InGameDebuffs = [];
            ImpToBeThrown = 37;
            Times = 1;
            NewZombieUpdateCD = 30;

            int bi = 0;
            foreach (var b in App.InitData.Value.AdvBuffs)
            {
                TravelBuffs.Add(new(new(bi, b, false, false)));
                InGameBuffs.Add(new(new(bi, b, true, false)));
                bi++;
            }
            foreach (var b in App.InitData.Value.UltiBuffs)
            {
                TravelBuffs.Add(new(new(bi, b, false, false)));
                InGameBuffs.Add(new(new(bi, b, true, false)));
                bi++;
            }
            int di = 0;
            foreach (var d in App.InitData.Value.Debuffs)
            {
                Debuffs.Add(new(new(di, d, true, true)));
                InGameDebuffs.Add(new(new(di, d, true, true)));
                di++;
            }
            TravelBuffs.ListChanged += (sender, e) => SyncTravelBuffs();
            InGameBuffs.ListChanged += (sender, e) => SyncInGameBuffs();
            Debuffs.ListChanged += (_, _) => SyncTravelBuffs();
            InGameDebuffs.ListChanged += (_, _) => SyncInGameBuffs();
            CardReplaces = [];
            for (int i = 0; i < 14; i++)
            {
                CardReplaces.Add(new(new()));
            }
            CardReplaces.ListChanged += (sender, e) => SyncCards();
            Hotkeys = [];
            foreach (var (h, hui) in from h in KeyCommands let hui = new HotkeyUI() select (h, hui))
            {
                Hotkeys.Add(new HotkeyUIVM(hui)
                {
                    Command = new(h.Item2),
                    Text = h.Item1
                });
            }
            using StreamReader reader = File.OpenText("UserData/MelonPreferences.cfg");
            TomlTable table = TOML.Parse(reader);
            InGameHotkeys = [
                new(new("高级时停 TimeStop", Enum.Parse<KeyCode>(table["PVZRHTools"]["KeyTimeStop"].AsString.Value))),
                new(new("卡槽栏置顶 TopMostCardBank", Enum.Parse<KeyCode>(table["PVZRHTools"]["KeyTopMostCardBank"].AsString.Value))),
                new(new("显示CD信息 ShowCDInfo", Enum.Parse<KeyCode>(table["PVZRHTools"]["KeyShowGameInfo"].AsString.Value))),
                new(new("图鉴种植：植物 AlmanacCreatePlant", Enum.Parse<KeyCode>(table["PVZRHTools"]["KeyAlmanacCreatePlant"].AsString.Value))),
                new(new("图鉴种植：僵尸 AlmanacCreateZombie", Enum.Parse<KeyCode>(table["PVZRHTools"]["KeyAlmanacCreateZombie"].AsString.Value))),
                new(new("图鉴种植：僵尸是否魅惑 AlmanacZombieMindCtrl", Enum.Parse<KeyCode>(table["PVZRHTools"]["KeyAlmanacZombieMindCtrl"].AsString.Value))),
            ];
            InGameHotkeys.ListChanged += (_, _) => SyncInGameHotkeys();
        }

        public ModifierViewModel(List<HotkeyUIVM> hotkeys) : this()
        {
            int hi = 0;
            Hotkeys = [];
            foreach (var (h, hui) in from h in KeyCommands let hui = new HotkeyUI() select (h, hui))
            {
                Hotkeys.Add(new HotkeyUIVM(hotkeys[hi].HotkeyUI)
                {
                    Command = new(h.Item2),
                    Text = h.Item1
                });
                hi++;
            }
        }

        public ModifierViewModel(ModifierSaveModel s)
        {
            Plants = new()
            {
                { -1, "-1 : 不修改" }
            };
            Bullets2 = new()
            {
                {-2,"-2 : 不修改" },
                {-1,"-1 : 随机子弹" }
            };
            Health1sts = [];
            Health2nds = [];
            HealthPlants = [];
            HealthZombies = [];
            foreach (var kp in App.InitData!.Value.Plants)
            {
                Plants.Add(kp.Key, kp.Value);
            }
            foreach (var h1 in App.InitData.Value.FirstArmors)
            {
                Health1sts.Add(h1.Key, -1);
            }
            foreach (var h2 in App.InitData.Value.SecondArmors)
            {
                Health2nds.Add(h2.Key, -1);
            }
            foreach (var h3 in App.InitData.Value.Plants)
            {
                HealthPlants.Add(h3.Key, -1);
            }
            foreach (var h4 in App.InitData.Value.Zombies)
            {
                HealthZombies.Add(h4.Key, -1);
            }
            foreach (var b in Bullets)
            {
                Bullets2.Add(b.Key, b.Key.ToString() + " : " + b.Value);
            }
            InGameBuffs = [];
            InGameDebuffs = [];
            CardNoInit = s.CardNoInit;
            CardReplaces = [.. s.CardReplaces];
            ChomperNoCD = s.ChomperNoCD;
            ClearOnWritingField = s.ClearOnWritingField;
            ClearOnWritingZombies = s.ClearOnWritingZombies;
            CobCannonNoCD = s.CobCannonNoCD;
            Col = s.Col;
            ColumnPlanting = s.ColumnPlanting;
            Debuffs = [.. s.Debuffs];
            DeveloperMode = s.DeveloperMode;
            DevLour = s.DevLour;
            Exchange = s.Exchange;
            FastShooting = s.FastShooting;
            FieldString = s.FieldString;
            FreeCD = s.FreeCD;
            FreePlanting = s.FreePlanting;
            GameSpeed = s.GameSpeed;
            GarlicDay = s.GarlicDay;
            GloveNoCD = s.GloveNoCD;
            HammerNoCD = s.HammerNoCD;
            HardPlant = s.HardPlant;
            HyponoEmperorNoCD = s.HyponoEmperorNoCD;
            ImpToBeThrown = s.ImpToBeThrown;
            IsMindCtrl = s.IsMindCtrl;
            ItemExistForever = s.ItemExistForever;
            ItemType = s.ItemType;
            JackboxNotExplode = s.JackboxNotExplode;
            LockBulletType = s.LockBulletType;
            LockMoney = s.LockMoney;
            LockPresent = s.LockPresent;
            LockSun = s.LockSun;
            MineNoCD = s.MineNoCD;
            NeedSave = s.NeedSave;
            NewLevelName = s.NewLevelName;
            NewMoney = s.NewMoney;
            NewSun = s.NewSun;
            NoFail = s.NoFail;
            NoHole = s.NoHole;
            NoIceRoad = s.NoIceRoad;
            PlantingNoCD = s.PlantingNoCD;
            PlantType = s.PlantType;
            PresentFastOpen = s.PresentFastOpen;
            Row = s.Row;
            ScaredyDream = s.ScaredyDream;
            SeedRain = s.SeedRain;
            Shooting1 = s.Shooting1;
            Shooting2 = s.Shooting2;
            Shooting3 = s.Shooting3;
            Shooting4 = s.Shooting4;
            ShowText = s.ShowText;
            StopSummon = s.StopSummon;
            SuperPresent = s.SuperPresent;
            Times = s.Times;
            TopMostSprite = s.TopMostSprite;
            TravelBuffs = [.. s.TravelBuffs];
            UltimateRamdomZombie = s.UltimateRamdomZombie;
            UltimateSuperGatling = s.UltimateSuperGatling;
            UndeadBullet = s.UndeadBullet;
            UnlockAllFusions = s.UnlockAllFusions;
            ZombieFieldString = s.ZombieFieldString;
            ZombieSeaCD = s.ZombieSeaCD;
            ZombieSeaEnabled = s.ZombieSeaEnabled;
            ZombieType = s.ZombieType;
            ZombieSeaTypes = [.. from zst in s.ZombieSeaTypes select new KeyValuePair<int, string>(zst, Zombies[zst])];
            ZombieSeaLowEnabled = s.ZombieSeaLowEnabled;
            HammerFullCD = s.HammerFullCD;
            HammerFullCDEnabled = s.HammerFullCDEnabled;
            GloveFullCD = s.GloveFullCD;
            GloveFullCDEnabled = s.GloveFullCDEnabled;
            NewZombieUpdateCD = s.NewZombieUpdateCD;
            int bi = 0;
            foreach (var b in App.InitData.Value.AdvBuffs)
            {
                InGameBuffs.Add(new(new(bi, b, true, false)));
                bi++;
            }
            foreach (var b in App.InitData.Value.UltiBuffs)
            {
                InGameBuffs.Add(new(new(bi, b, true, false)));
                bi++;
            }
            int di = 0;
            foreach (var d in App.InitData.Value.Debuffs)
            {
                InGameDebuffs.Add(new(new(di, d, true, true)));
            }
            TravelBuffs.ListChanged += (sender, e) => SyncTravelBuffs();
            InGameBuffs.ListChanged += (sender, e) => SyncInGameBuffs();
            Debuffs.ListChanged += (_, _) => SyncTravelBuffs();
            InGameDebuffs.ListChanged += (_, _) => SyncInGameBuffs();
            CardReplaces.ListChanged += (sender, e) => SyncCards();
            int hi = 0;
            Hotkeys = [];
            foreach (var (h, hui) in from h in KeyCommands let hui = new HotkeyUI() select (h, hui))
            {
                Hotkeys.Add(new HotkeyUIVM(s.Hotkeys[hi].HotkeyUI)
                {
                    Command = new(h.Item2),
                    Text = h.Item1
                });
                hi++;
            }
            using StreamReader reader = File.OpenText("UserData/MelonPreferences.cfg");
            TomlTable table = TOML.Parse(reader);
            InGameHotkeys = [
                new(new("高级时停 TimeStop", Enum.Parse<KeyCode>(table["PVZRHTools"]["KeyTimeStop"].AsString.Value))),
                new(new("卡槽栏置顶 TopMostCardBank", Enum.Parse<KeyCode>(table["PVZRHTools"]["KeyTopMostCardBank"].AsString.Value))),
                new(new("显示CD信息 ShowCDInfo", Enum.Parse<KeyCode>(table["PVZRHTools"]["KeyShowGameInfo"].AsString.Value))),
                new(new("图鉴种植：植物 AlmanacCreatePlant", Enum.Parse<KeyCode>(table["PVZRHTools"]["KeyAlmanacCreatePlant"].AsString.Value))),
                new(new("图鉴种植：僵尸 AlmanacCreateZombie", Enum.Parse<KeyCode>(table["PVZRHTools"]["KeyAlmanacCreateZombie"].AsString.Value))),
                new(new("图鉴种植：僵尸是否魅惑 AlmanacZombieMindCtrl", Enum.Parse<KeyCode>(table["PVZRHTools"]["KeyAlmanacZombieMindCtrl"].AsString.Value))),
            ];
            InGameHotkeys.ListChanged += (_, _) => SyncInGameHotkeys();
        }

        #region Commands

        [RelayCommand]
        public void BulletDamage() => App.DataSync.Value.SendData(new ValueProperties() { BulletsDamage = new(BulletDamageType, (int)BulletDamageValue) });

        [RelayCommand]
        public void ClearAllHoles() => App.DataSync.Value.SendData(new InGameActions() { ClearAllHoles = true });

        [RelayCommand]
        public void ClearAllPlants() => App.DataSync.Value.SendData(new InGameActions() { ClearAllPlants = true });

        [RelayCommand]
        public void ClearIceRoads() => App.DataSync.Value.SendData(new InGameActions() { ClearAllIceRoads = true });

        [RelayCommand]
        public void CopyFieldScripts() => App.DataSync.Value.SendData(new InGameActions() { ReadField = true });

        [RelayCommand]
        public void CopyZombieScripts() => App.DataSync.Value.SendData(new InGameActions() { ReadZombies = true });

        [RelayCommand]
        public void CreateActiveMateorite() => App.DataSync.Value.SendData(new InGameActions() { CreateActiveMateorite = true });

        [RelayCommand]
        public void CreateCard() => App.DataSync.Value.SendData(new InGameActions() { Card = true, PlantType = PlantType });

        [RelayCommand]
        public void CreateItem() => App.DataSync.Value.SendData(new InGameActions() { ItemType = ItemType, });

        [RelayCommand]
        public void CreateMower() => App.DataSync.Value.SendData(new InGameActions() { CreateMower = true });

        [RelayCommand]
        public void CreatePassiveMateorite() => App.DataSync.Value.SendData(new InGameActions() { CreatePassiveMateorite = true });

        [RelayCommand]
        public void CreatePlant() => App.DataSync.Value.SendData(new InGameActions()
        {
            Row = (int)Row,
            Column = (int)Col,
            Times = (int)Times,
            PlantType = PlantType,
        });

        [RelayCommand]
        public void CreateUltimateMateorite() => App.DataSync.Value.SendData(new InGameActions() { CreateUltimateMateorite = true });

        [RelayCommand]
        public void CreateZombie() => App.DataSync.Value.SendData(new InGameActions()
        {
            Row = (int)Row,
            Column = (int)Col,
            Times = (int)Times,
            ZombieType = ZombieType,
            SummonMindControlledZombies = IsMindCtrl,
        });

        [RelayCommand]
        public void DebuffSelectAll()
        {
            NeedSync = false;
            foreach (var t in Debuffs)
            {
                t.Enabled = true;
            }
            NeedSync = true;
            SyncTravelBuffs();
        }

        [RelayCommand]
        public void DebuffUnselectAll()
        {
            NeedSync = false;
            foreach (TravelBuffVM t in Debuffs)
            {
                t.Enabled = false;
            }
            NeedSync = true;
            SyncTravelBuffs();
        }

        [RelayCommand]
        public void Health1st() => App.DataSync.Value.SendData(new ValueProperties() { FirstArmorsHealth = new(Health1stType, (int)Health1stValue) });

        [RelayCommand]
        public void Health2nd() => App.DataSync.Value.SendData(new ValueProperties() { SecondArmorsHealth = new(Health2ndType, (int)Health2ndValue) });

        [RelayCommand]
        public void HealthPlant() => App.DataSync.Value.SendData(new ValueProperties() { PlantsHealth = new(HealthPlantType, (int)HealthPlantValue) });

        [RelayCommand]
        public void HealthZombie() => App.DataSync.Value.SendData(new ValueProperties() { ZombiesHealth = new(HealthZombieType, (int)HealthZombieValue) });

        [RelayCommand]
        public void InGameBuffSelectAll()
        {
            if (!App.inited) return;
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
            if (!App.inited) return;
            NeedSync = false;
            foreach (var t in InGameBuffs)
            {
                t.Enabled = false;
            }
            NeedSync = true;
            SyncInGameBuffs();
        }

        [RelayCommand]
        public void InGameDebuffSelectAll()
        {
            if (!App.inited) return;
            NeedSync = false;
            foreach (var t in InGameDebuffs)
            {
                t.Enabled = true;
            }
            NeedSync = true;
            SyncInGameBuffs();
        }

        [RelayCommand]
        public void InGameDebuffUnselectAll()
        {
            if (!App.inited) return;
            NeedSync = false;
            foreach (var t in InGameDebuffs)
            {
                t.Enabled = false;
            }
            NeedSync = true;
            SyncInGameBuffs();
        }

        [RelayCommand]
        public void KillAllZombies() => App.DataSync.Value.SendData(new InGameActions() { ClearAllZombies = true });

        [RelayCommand]
        public void LevelName() => App.DataSync.Value.SendData(new InGameActions() { ChangeLevelName = NewLevelName });

        [RelayCommand]
        public void LockBullet() => App.DataSync.Value.SendData(new ValueProperties() { LockBulletType = LockBulletType });

        [RelayCommand]
        public void MindCtrl() => App.DataSync.Value.SendData(new InGameActions() { MindControlAllZombies = true });

        [RelayCommand]
        public void Money() => App.DataSync.Value.SendData(new InGameActions() { CurrentMoney = (int)NewMoney });

        [RelayCommand]
        public void NextWave() => App.DataSync.Value.SendData(new InGameActions() { NextWave = true });

        [RelayCommand]
        public void PlantVase() => App.DataSync.Value.SendData(new InGameActions()
        {
            PlantVase = true,
            PlantType = PlantType,
            Row = (int)Row,
            Column = (int)Col,
        });

        public void Save()
        {
            ModifierSaveModel s = new()
            {
                CardNoInit = CardNoInit,
                CardReplaces = [.. CardReplaces],
                ChomperNoCD = ChomperNoCD,
                ClearOnWritingField = ClearOnWritingField,
                ClearOnWritingZombies = ClearOnWritingZombies,
                CobCannonNoCD = CobCannonNoCD,
                Col = Col,
                ColumnPlanting = ColumnPlanting,
                Debuffs = [.. Debuffs],
                DeveloperMode = DeveloperMode,
                DevLour = DevLour,
                Exchange = Exchange,
                FastShooting = FastShooting,
                FieldString = FieldString,
                FreeCD = FreeCD,
                FreePlanting = FreePlanting,
                GameSpeed = GameSpeed,
                GarlicDay = GarlicDay,
                GloveNoCD = GloveNoCD,
                HammerNoCD = HammerNoCD,
                HardPlant = HardPlant,
                HyponoEmperorNoCD = HyponoEmperorNoCD,
                ImpToBeThrown = ImpToBeThrown,
                IsMindCtrl = IsMindCtrl,
                ItemExistForever = ItemExistForever,
                ItemType = ItemType,
                JackboxNotExplode = JackboxNotExplode,
                LockBulletType = LockBulletType,
                LockMoney = LockMoney,
                LockPresent = LockPresent,
                LockSun = LockSun,
                MineNoCD = MineNoCD,
                NeedSave = NeedSave,
                NewLevelName = NewLevelName,
                NewMoney = NewMoney,
                NewSun = NewSun,
                NoFail = NoFail,
                NoHole = NoHole,
                NoIceRoad = NoIceRoad,
                PlantingNoCD = PlantingNoCD,
                PlantType = PlantType,
                PresentFastOpen = PresentFastOpen,
                Row = Row,
                ScaredyDream = ScaredyDream,
                SeedRain = SeedRain,
                Shooting1 = Shooting1,
                Shooting2 = Shooting2,
                Shooting3 = Shooting3,
                Shooting4 = Shooting4,
                ShowText = ShowText,
                StopSummon = StopSummon,
                SuperPresent = SuperPresent,
                Times = Times,
                TopMostSprite = TopMostSprite,
                TravelBuffs = [.. TravelBuffs],
                UltimateRamdomZombie = UltimateRamdomZombie,
                UltimateSuperGatling = UltimateSuperGatling,
                UndeadBullet = UndeadBullet,
                UnlockAllFusions = UnlockAllFusions,
                ZombieFieldString = ZombieFieldString,
                ZombieSeaCD = ZombieSeaCD,
                ZombieSeaEnabled = ZombieSeaEnabled,
                ZombieSeaTypes = [],
                ZombieType = ZombieType,
                ZombieSeaLowEnabled = ZombieSeaLowEnabled,
                GloveFullCD = GloveFullCD,
                GloveFullCDEnabled = GloveFullCDEnabled,
                HammerFullCD = HammerFullCD,
                HammerFullCDEnabled = HammerFullCDEnabled,
                Hotkeys = Hotkeys,
                NewZombieUpdateCD = NewZombieUpdateCD,
            };
            if (ZombieSeaTypes.Count > 0)
            {
                s.ZombieSeaTypes.AddRange(from zst in ZombieSeaTypes select zst.Key);
            }
            File.WriteAllText("UserData/ModifierSettings.json", JsonSerializer.Serialize(s, ModifierSaveModelSGC.Default.ModifierSaveModel));
        }

        [RelayCommand]
        public void SetAward() => App.DataSync.Value.SendData(new InGameActions() { SetAward = true });

        [RelayCommand]
        public void ShowingText() => App.DataSync.Value.SendData(new InGameActions() { ShowText = ShowText });

        [RelayCommand]
        public void SimplePresents() => App.DataSync.Value.SendData(new InGameActions()
        {
            WriteField = "H4sIAAAAAAAACjPQMdIxMjWzNoTSQBJMG0NpEyhtCqYNrM2gtDmUtoDSlhAaABg+1o9PAAAA",
            ClearOnWritingField = ClearOnWritingField
        });

        [RelayCommand]
        public void StartMower() => App.DataSync.Value.SendData(new InGameActions() { StartMower = true });

        [RelayCommand]
        public void Sun() => App.DataSync.Value.SendData(new InGameActions() { CurrentSun = (int)NewSun });

        public void SyncAll()
        {
            if (!NeedSave) return;
            List<bool> adv = [];
            List<bool> ulti = [];
            List<bool> deb = [];
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
            foreach (var d in Debuffs)
            {
                deb.Add(d.TravelBuff.Enabled);
            }
            InGameActions iga = new()
            {
                NoFail = NoFail,
                StopSummon = StopSummon,
                ZombieSeaCD = (int)ZombieSeaCD,
                ZombieSeaEnabled = ZombieSeaEnabled,
                ZombieSeaTypes = [],
                ZombieType = ZombieType,
            };
            iga.ZombieSeaTypes.AddRange(from zst in ZombieSeaTypes select zst.Key);
            List<Card> cards = [];
            cards.AddRange(from c in CardReplaces select c.CardUI.GetCard());
            SyncAll syncAll = new()
            {
                BasicProperties = new BasicProperties()
                {
                    CardNoInit = CardNoInit,
                    ChomperNoCD = ChomperNoCD,
                    CobCannonNoCD = CobCannonNoCD,
                    DeveloperMode = DeveloperMode,
                    DevLour = DevLour,
                    FastShooting = FastShooting,
                    FreePlanting = FreePlanting,
                    GameSpeed = (int)GameSpeed,
                    GarlicDay = GarlicDay,
                    GloveNoCD = GloveNoCD,
                    HammerNoCD = HammerNoCD,
                    HardPlant = HardPlant,
                    HyponoEmperorNoCD = HyponoEmperorNoCD,
                    ImpToBeThrown = ImpToBeThrown,
                    ItemExistForever = ItemExistForever,
                    JackboxNotExplode = JackboxNotExplode,
                    LockPresent = LockPresent,
                    MineNoCD = MineNoCD,
                    NoHole = NoHole,
                    NoIceRoad = NoIceRoad,
                    PlantingNoCD = PlantingNoCD,
                    PresentFastOpen = PresentFastOpen,
                    SuperPresent = SuperPresent,
                    UltimateRamdomZombie = UltimateRamdomZombie,
                    UndeadBullet = UndeadBullet,
                    UnlockAllFusions = UnlockAllFusions,
                    GloveFullCD = GloveFullCDEnabled ? (int)GloveFullCD : -1,
                    HammerFullCD = HammerFullCDEnabled ? (int)HammerFullCD : -1,
                    NewZombieUpdateCD = NewZombieUpdateCD,
                },
                CardProperties = new CardProperties() { CardReplaces = cards },
                InGameActions = iga,
                TravelBuffs = new SyncTravelBuff()
                {
                    AdvTravelBuff = adv,
                    UltiTravelBuff = ulti,
                    Debuffs = deb
                },
                ValueProperties = new ValueProperties() { LockBulletType = LockBulletType },
                GameModes = new GameModes()
                {
                    Exchange = Exchange,
                    ScaredyDream = ScaredyDream,
                    ColumnPlanting = ColumnPlanting,
                    SeedRain = SeedRain,
                    Shooting1 = Shooting1,
                    Shooting2 = Shooting2,
                    Shooting3 = Shooting3,
                    Shooting4 = Shooting4,
                }
            };

            App.DataSync.Value.SendData(syncAll);
        }

        public void SyncCards()
        {
            List<Card> cards = [.. from c in CardReplaces select c.CardUI.GetCard()];
            App.DataSync.Value.SendData(new CardProperties() { CardReplaces = cards });
        }

        public void SyncInGameBuffs()
        {
            List<bool> adv = [];
            List<bool> ulti = [];
            List<bool> deb = [];
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
            foreach (var d in InGameDebuffs)
            {
                deb.Add(d.TravelBuff.Enabled);
            }

            App.DataSync.Value.SendData(new SyncTravelBuff()
            {
                AdvInGame = adv,
                UltiInGame = ulti,
                DebuffsInGame = deb
            });
        }

        public void SyncInGameHotkeys()
        {
            DataSync.Enabled = false;
            List<int> keys = [];
            foreach (var igh in InGameHotkeys)
            {
                keys.Add(((int)igh.InGameHotkeyUI.KeyCode));
            }
            DataSync.Enabled = true;

            App.DataSync.Value.SendData(new InGameHotkeys() { KeyCodes = keys });
        }

        public void SyncTravelBuffs()
        {
            List<bool> adv = [];
            List<bool> ulti = [];
            List<bool> deb = [];
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
            foreach (var d in Debuffs)
            {
                deb.Add(d.TravelBuff.Enabled);
            }
            App.DataSync.Value.SendData(new SyncTravelBuff()
            {
                AdvTravelBuff = adv,
                UltiTravelBuff = ulti,
                Debuffs = deb
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
            foreach (TravelBuffVM t in TravelBuffs)
            {
                t.Enabled = false;
            }
            NeedSync = true;
            SyncTravelBuffs();
        }

        [RelayCommand]
        public void Win() => App.DataSync.Value.SendData(new InGameActions() { Win = true });

        [RelayCommand]
        public void WriteField() => App.DataSync.Value.SendData(new InGameActions() { WriteField = FieldString, ClearOnWritingField = ClearOnWritingField });

        [RelayCommand]
        public void WriteZombies() => App.DataSync.Value.SendData(new InGameActions() { WriteZombies = ZombieFieldString, ClearOnWritingZombies = ClearOnWritingZombies });

        public void ZombieSea()
        {
            if (!App.inited) return;
            List<int> types = [];
            foreach (var type in ZombieSeaTypes)
            {
                types.Add(type.Key);
            }
            App.DataSync.Value.SendData(new InGameActions()
            {
                ZombieSeaEnabled = ZombieSeaEnabled,
                ZombieSeaLowEnabled = ZombieSeaLowEnabled,
                ZombieSeaCD = (int)ZombieSeaCD,
                ZombieSeaTypes = types
            });
        }

        [RelayCommand]
        public void ZombieVase() => App.DataSync.Value.SendData(new InGameActions()
        {
            ZombieVase = true,
            ZombieType = ZombieType,
            Row = (int)Row,
            Column = (int)Col,
        });

        private void GameModes() => App.DataSync.Value.SendData(new GameModes()
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

        partial void OnCardNoInitChanged(bool value) => App.DataSync.Value.SendData(new BasicProperties() { CardNoInit = value });

        partial void OnChomperNoCDChanged(bool value) => App.DataSync.Value.SendData(new BasicProperties() { ChomperNoCD = value });

        partial void OnCobCannonNoCDChanged(bool value) => App.DataSync.Value.SendData(new BasicProperties() { CobCannonNoCD = value });

        partial void OnColumnPlantingChanged(bool value) => GameModes();

        partial void OnDeveloperModeChanged(bool value) => App.DataSync.Value.SendData(new BasicProperties() { DeveloperMode = value, PlantingNoCD = FreeCD });

        partial void OnDevLourChanged(bool value) => App.DataSync.Value.SendData(new BasicProperties() { DevLour = value });

        partial void OnExchangeChanged(bool value) => GameModes();

        partial void OnFastShootingChanged(bool value) => App.DataSync.Value.SendData(new BasicProperties() { FastShooting = value });

        partial void OnFreePlantingChanged(bool value) => App.DataSync.Value.SendData(new BasicProperties() { FreePlanting = value });

        partial void OnGameSpeedChanged(double value) => App.DataSync.Value.SendData(new BasicProperties() { GameSpeed = value });

        partial void OnGarlicDayChanged(bool value) => App.DataSync.Value.SendData(new BasicProperties() { GarlicDay = value });

        partial void OnGloveFullCDChanged(double value) => App.DataSync.Value.SendData(new BasicProperties() { GloveFullCD = value });

        partial void OnGloveFullCDEnabledChanged(bool value) => App.DataSync.Value.SendData(new BasicProperties() { GloveFullCD = value ? GloveFullCD : -1 });

        partial void OnGloveNoCDChanged(bool value) => App.DataSync.Value.SendData(new BasicProperties() { GloveNoCD = value });

        partial void OnHammerFullCDChanged(double value) => App.DataSync.Value.SendData(new BasicProperties() { HammerFullCD = value });

        partial void OnHammerFullCDEnabledChanged(bool value) => App.DataSync.Value.SendData(new BasicProperties() { HammerFullCD = value ? HammerFullCD : -1 });

        partial void OnHammerNoCDChanged(bool value) => App.DataSync.Value.SendData(new BasicProperties() { HammerNoCD = value });

        partial void OnHardPlantChanged(bool value) => App.DataSync.Value.SendData(new BasicProperties() { HardPlant = value });

        partial void OnHyponoEmperorNoCDChanged(bool value) => App.DataSync.Value.SendData(new BasicProperties() { HyponoEmperorNoCD = value });

        partial void OnImpToBeThrownChanged(int value) => App.DataSync.Value.SendData(new BasicProperties() { ImpToBeThrown = value });

        partial void OnItemExistForeverChanged(bool value) => App.DataSync.Value.SendData(new BasicProperties() { ItemExistForever = value });

        partial void OnJackboxNotExplodeChanged(bool value) => App.DataSync.Value.SendData(new BasicProperties() { JackboxNotExplode = value });

        partial void OnLockMoneyChanged(bool value) => App.DataSync.Value.SendData(new InGameActions() { LockMoney = value, CurrentMoney = (int)NewMoney });

        partial void OnLockPresentChanged(int value) => App.DataSync.Value.SendData(new BasicProperties() { LockPresent = value });

        partial void OnLockSunChanged(bool value) => App.DataSync.Value.SendData(new InGameActions() { LockSun = value, CurrentSun = (int)NewSun });

        partial void OnMineNoCDChanged(bool value) => App.DataSync.Value.SendData(new BasicProperties() { MineNoCD = value });

        partial void OnNewZombieUpdateCDChanged(double value) => App.DataSync.Value.SendData(new BasicProperties() { NewZombieUpdateCD = value });

        partial void OnNoFailChanged(bool value) => App.DataSync.Value.SendData(new InGameActions() { NoFail = value });

        partial void OnNoHoleChanged(bool value) => App.DataSync.Value.SendData(new BasicProperties() { NoHole = value });

        partial void OnNoIceRoadChanged(bool value) => App.DataSync.Value.SendData(new BasicProperties() { NoIceRoad = value });

        partial void OnPlantingNoCDChanged(bool value) => App.DataSync.Value.SendData(new BasicProperties() { PlantingNoCD = value });

        partial void OnPresentFastOpenChanged(bool value) => App.DataSync.Value.SendData(new BasicProperties() { PresentFastOpen = value });

        partial void OnScaredyDreamChanged(bool value) => GameModes();

        partial void OnSeedRainChanged(bool value) => GameModes();

        partial void OnShooting1Changed(bool value) => GameModes();

        partial void OnShooting2Changed(bool value) => GameModes();

        partial void OnShooting3Changed(bool value) => GameModes();

        partial void OnShooting4Changed(bool value) => GameModes();

        partial void OnStopSummonChanged(bool value) => App.DataSync.Value.SendData(new InGameActions() { StopSummon = value });

        partial void OnSuperPresentChanged(bool value) => App.DataSync.Value.SendData(new BasicProperties() { SuperPresent = value });

        partial void OnTopMostSpriteChanged(bool value)
        {
            if (value)
            {
                MainWindow.Instance!.ModifierSprite.Show();
            }
            else
            {
                MainWindow.Instance!.ModifierSprite.Hide();
            }
        }

        partial void OnUltimateRamdomZombieChanged(bool value) => App.DataSync.Value.SendData(new BasicProperties() { UltimateRamdomZombie = value });

        partial void OnUltimateSuperGatlingChanged(bool value) => App.DataSync.Value.SendData(new BasicProperties() { UltimateSuperGatling = value });

        partial void OnUndeadBulletChanged(bool value) => App.DataSync.Value.SendData(new BasicProperties() { UndeadBullet = value });

        partial void OnUnlockAllFusionsChanged(bool value) => App.DataSync.Value.SendData(new BasicProperties() { UnlockAllFusions = value });

        partial void OnZombieSeaCDChanged(double value) => ZombieSea();

        partial void OnZombieSeaEnabledChanged(bool value) => ZombieSea();

        partial void OnZombieSeaLowEnabledChanged(bool value) => ZombieSea();

        partial void OnZombieSeaTypesChanged(List<KeyValuePair<int, string>> value) => ZombieSea();

        #endregion Commands

        #region ItemSources

        public static bool NeedSync { get; set; } = true;

        public static Dictionary<int, string>? Plants { get; set; }

        public Dictionary<int, string> Bullets => App.InitData!.Value.Bullets;

        public Dictionary<int, string> Bullets2 { get; set; }

        public Dictionary<int, string> FirstArmor => App.InitData!.Value.FirstArmors;

        public Dictionary<int, int> Health1sts { get; set; }

        public Dictionary<int, int> Health2nds { get; set; }

        public Dictionary<int, int> HealthPlants { get; set; }

        public Dictionary<int, int> HealthZombies { get; set; }

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

        public List<(string, Action)> KeyCommands =>
        [
            ("手套无CD",()=>GloveNoCD=!GloveNoCD),
            ("锤子无CD",()=>HammerNoCD=!HammerNoCD),
            ("植物卡槽无CD",()=>PlantingNoCD=!PlantingNoCD),
            ("自由种植",()=>FreePlanting=!FreePlanting),
            ("解锁全部融合配方",()=>UnlockAllFusions=!UnlockAllFusions),
            ("游戏加速",() => GameSpeed=GameSpeed<9?++GameSpeed:GameSpeed),
            ("游戏减速",()=>GameSpeed=GameSpeed>1?--GameSpeed:GameSpeed),
            ("胆小菇之梦",()=>ScaredyDream=!ScaredyDream),
            ("排山倒海",()=>ColumnPlanting=!ColumnPlanting),
            ("植物攻击无间隔",()=>FastShooting=!FastShooting),
            ("植物无敌",()=>HardPlant=!HardPlant),
            ("生成植物",CreatePlant),
            ("生成僵尸",CreateZombie),
            ("生成物品",CreateItem),
            ("生成究极陨星",CreateUltimateMateorite),
            ("斗蛐蛐快速布阵",SimplePresents),
            ("植物布阵",WriteField),
            ("僵尸布阵",WriteZombies),
            ("读取场上植物代码",CopyFieldScripts),
            ("读取场上僵尸代码",CopyZombieScripts),
            ("极限僵尸海",()=>ZombieSeaEnabled=!ZombieSeaEnabled),
            ("修改阳光",Sun),
            ("锁定阳光",()=>LockSun=!LockSun),
            ("修改钱数",Money),
            ("锁定钱数",()=>LockMoney=!LockMoney),
            ("清空全部植物",ClearAllPlants),
            ("秒杀全部僵尸",KillAllZombies),
            ("清除所有冰道",ClearIceRoads),
            ("魅惑所有僵尸",MindCtrl),
            ("清除所有坑洞",ClearAllHoles),
            ("生成下一波僵尸",NextWave),
            ("暂停出怪",()=>StopSummon=!StopSummon),
            ("僵尸进家不死",()=>NoFail=!NoFail),
            ("启动所有小推车",StartMower),
            ("生成小推车",CreateMower),
            ("修改关卡名称",LevelName),
            ("显示字幕",ShowingText),
            ("显示悬浮窗",()=>TopMostSprite=!TopMostSprite),
            ("显示修改窗口", () => {
                MainWindow.Instance!.Topmost = true;
                MainWindow.Instance!.Topmost = false;
            }),
        ];

        public Dictionary<int, string> Plants2 => App.InitData!.Value.Plants;

        public Dictionary<int, string> SecondArmor => App.InitData!.Value.SecondArmors;

        public Dictionary<int, string> Zombies => App.InitData!.Value.Zombies;

        #endregion ItemSources

        #region Properties

        [ObservableProperty]
        public partial int BulletDamageType { get; set; }

        [ObservableProperty]
        public partial double BulletDamageValue { get; set; }

        [ObservableProperty]
        public partial bool CardNoInit { get; set; }

        [ObservableProperty]
        public partial BindingList<CardUIVM> CardReplaces { get; set; }

        [ObservableProperty]
        public partial bool ChomperNoCD { get; set; }

        [ObservableProperty]
        public partial bool ClearOnWritingField { get; set; }

        [ObservableProperty]
        public partial bool ClearOnWritingZombies { get; set; }

        [ObservableProperty]
        public partial bool CobCannonNoCD { get; set; }

        [ObservableProperty]
        public partial double Col { get; set; }

        [ObservableProperty]
        public partial bool ColumnPlanting { get; set; }

        [ObservableProperty]
        public partial BindingList<TravelBuffVM> Debuffs { get; set; }

        [ObservableProperty]
        public partial bool DeveloperMode { get; set; }

        [ObservableProperty]
        public partial bool DevLour { get; set; }

        [ObservableProperty]
        public partial bool Exchange { get; set; }

        [ObservableProperty]
        public partial bool FastShooting { get; set; }

        [ObservableProperty]
        public partial string FieldString { get; set; }

        [ObservableProperty]
        public partial bool FreeCD { get; set; }

        [ObservableProperty]
        public partial bool FreePlanting { get; set; }

        [ObservableProperty]
        public partial double GameSpeed { get; set; }

        [ObservableProperty]
        public partial bool GarlicDay { get; set; }

        [ObservableProperty]
        public partial double GloveFullCD { get; set; }

        [ObservableProperty]
        public partial bool GloveFullCDEnabled { get; set; }

        [ObservableProperty]
        public partial bool GloveNoCD { get; set; }

        [ObservableProperty]
        public partial double HammerFullCD { get; set; }

        [ObservableProperty]
        public partial bool HammerFullCDEnabled { get; set; }

        [ObservableProperty]
        public partial bool HammerNoCD { get; set; }

        [ObservableProperty]
        public partial bool HardPlant { get; set; }

        [ObservableProperty]
        public partial int Health1stType { get; set; }

        [ObservableProperty]
        public partial double Health1stValue { get; set; }

        [ObservableProperty]
        public partial int Health2ndType { get; set; }

        [ObservableProperty]
        public partial double Health2ndValue { get; set; }

        [ObservableProperty]
        public partial int HealthPlantType { get; set; }

        [ObservableProperty]
        public partial double HealthPlantValue { get; set; }

        [ObservableProperty]
        public partial int HealthZombieType { get; set; }

        [ObservableProperty]
        public partial double HealthZombieValue { get; set; }

        [ObservableProperty]
        public partial List<HotkeyUIVM> Hotkeys { get; set; }

        [ObservableProperty]
        public partial bool HyponoEmperorNoCD { get; set; }

        [ObservableProperty]
        public partial int ImpToBeThrown { get; set; }

        [ObservableProperty]
        public partial BindingList<TravelBuffVM> InGameBuffs { get; set; }

        [ObservableProperty]
        public partial BindingList<TravelBuffVM> InGameDebuffs { get; set; }

        [ObservableProperty]
        public partial BindingList<InGameHotkeyUIVM> InGameHotkeys { get; set; }

        [ObservableProperty]
        public partial bool IsMindCtrl { get; set; }

        [ObservableProperty]
        public partial bool ItemExistForever { get; set; }

        [ObservableProperty]
        public partial int ItemType { get; set; }

        [ObservableProperty]
        public partial bool JackboxNotExplode { get; set; }

        [ObservableProperty]
        public partial int LockBulletType { get; set; }

        [ObservableProperty]
        public partial bool LockMoney { get; set; }

        [ObservableProperty]
        public partial int LockPresent { get; set; }

        [ObservableProperty]
        public partial bool LockSun { get; set; }

        [ObservableProperty]
        public partial bool MineNoCD { get; set; }

        [ObservableProperty]
        public partial bool NeedSave { get; set; }

        [ObservableProperty]
        public partial string NewLevelName { get; set; }

        [ObservableProperty]
        public partial double NewMoney { get; set; }

        [ObservableProperty]
        public partial double NewSun { get; set; }

        [ObservableProperty]
        public partial double NewZombieUpdateCD { get; set; }

        [ObservableProperty]
        public partial bool NoFail { get; set; }

        [ObservableProperty]
        public partial bool NoHole { get; set; }

        [ObservableProperty]
        public partial bool NoIceRoad { get; set; }

        [ObservableProperty]
        public partial bool PlantingNoCD { get; set; }

        [ObservableProperty]
        public partial int PlantType { get; set; }

        [ObservableProperty]
        public partial bool PresentFastOpen { get; set; }

        [ObservableProperty]
        public partial double Row { get; set; }

        [ObservableProperty]
        public partial bool ScaredyDream { get; set; }

        [ObservableProperty]
        public partial bool SeedRain { get; set; }

        [ObservableProperty]
        public partial bool Shooting1 { get; set; }

        [ObservableProperty]
        public partial bool Shooting2 { get; set; }

        [ObservableProperty]
        public partial bool Shooting3 { get; set; }

        [ObservableProperty]
        public partial bool Shooting4 { get; set; }

        [ObservableProperty]
        public partial string ShowText { get; set; }

        [ObservableProperty]
        public partial bool StopSummon { get; set; }

        [ObservableProperty]
        public partial bool SuperPresent { get; set; }

        [ObservableProperty]
        public partial double Times { get; set; }

        [ObservableProperty]
        public partial bool TopMostSprite { get; set; }

        [ObservableProperty]
        public partial BindingList<TravelBuffVM> TravelBuffs { get; set; }

        [ObservableProperty]
        public partial bool UltimateRamdomZombie { get; set; }

        [ObservableProperty]
        public partial bool UltimateSuperGatling { get; set; }

        [ObservableProperty]
        public partial bool UndeadBullet { get; set; }

        [ObservableProperty]
        public partial bool UnlockAllFusions { get; set; }

        [ObservableProperty]
        public partial string ZombieFieldString { get; set; }

        [ObservableProperty]
        public partial double ZombieSeaCD { get; set; }

        [ObservableProperty]
        public partial bool ZombieSeaEnabled { get; set; }

        [ObservableProperty]
        public partial bool ZombieSeaLowEnabled { get; set; }

        [ObservableProperty]
        public partial List<KeyValuePair<int, string>> ZombieSeaTypes { get; set; }

        [ObservableProperty]
        public partial int ZombieType { get; set; }

        #endregion Properties
    }

    public partial class TravelBuff : ObservableObject, INotifyPropertyChanged
    {
        public TravelBuff(int index, string text, bool inGame, bool debuff)
        {
            Text = text;
            Index = index;
            InGame = inGame;
            Debuff = debuff;
        }

        public TravelBuff()
        { }

        public bool Debuff { get; set; }

        [ObservableProperty]
        public partial bool Enabled { get; set; }

        public int Index { get; set; }
        public bool InGame { get; set; }
        public string Text { get; set; } = "";
    }

    public partial class TravelBuffVM(TravelBuff TravelBuff) : ObservableObject
    {
        public bool Enabled
        {
            get => TravelBuff.Enabled;
            set
            {
                SetProperty(TravelBuff.Enabled, value, TravelBuff, (t, e) => t.Enabled = e);
                OnPropertyChanged(new PropertyChangedEventArgs("IsChecked"));
            }
        }

        [ObservableProperty]
        public partial TravelBuff TravelBuff { get; set; } = TravelBuff;
    }

    //copy from UnityEngine.KeyCode
    public enum KeyCode
    {
        None = 0,
        Backspace = 8,
        Delete = 127,
        Tab = 9,
        Clear = 12,
        Return = 13,
        Pause = 19,
        Escape = 27,
        Space = 32,
        Keypad0 = 256,
        Keypad1 = 257,
        Keypad2 = 258,
        Keypad3 = 259,
        Keypad4 = 260,
        Keypad5 = 261,
        Keypad6 = 262,
        Keypad7 = 263,
        Keypad8 = 264,
        Keypad9 = 265,
        KeypadPeriod = 266,
        KeypadDivide = 267,
        KeypadMultiply = 268,
        KeypadMinus = 269,
        KeypadPlus = 270,
        KeypadEnter = 271,
        KeypadEquals = 272,
        UpArrow = 273,
        DownArrow = 274,
        RightArrow = 275,
        LeftArrow = 276,
        Insert = 277,
        Home = 278,
        End = 279,
        PageUp = 280,
        PageDown = 281,
        F1 = 282,
        F2 = 283,
        F3 = 284,
        F4 = 285,
        F5 = 286,
        F6 = 287,
        F7 = 288,
        F8 = 289,
        F9 = 290,
        F10 = 291,
        F11 = 292,
        F12 = 293,
        F13 = 294,
        F14 = 295,
        F15 = 296,
        Alpha0 = 48,
        Alpha1 = 49,
        Alpha2 = 50,
        Alpha3 = 51,
        Alpha4 = 52,
        Alpha5 = 53,
        Alpha6 = 54,
        Alpha7 = 55,
        Alpha8 = 56,
        Alpha9 = 57,
        Exclaim = 33,
        DoubleQuote = 34,
        Hash = 35,
        Dollar = 36,
        Percent = 37,
        Ampersand = 38,
        Quote = 39,
        LeftParen = 40,
        RightParen = 41,
        Asterisk = 42,
        Plus = 43,
        Comma = 44,
        Minus = 45,
        Period = 46,
        Slash = 47,
        Colon = 58,
        Semicolon = 59,
        Less = 60,
        Equals = 61,
        Greater = 62,
        Question = 63,
        At = 64,
        LeftBracket = 91,
        Backslash = 92,
        RightBracket = 93,
        Caret = 94,
        Underscore = 95,
        BackQuote = 96,
        A = 97,
        B = 98,
        C = 99,
        D = 100,
        E = 101,
        F = 102,
        G = 103,
        H = 104,
        I = 105,
        J = 106,
        K = 107,
        L = 108,
        M = 109,
        N = 110,
        O = 111,
        P = 112,
        Q = 113,
        R = 114,
        S = 115,
        T = 116,
        U = 117,
        V = 118,
        W = 119,
        X = 120,
        Y = 121,
        Z = 122,
        LeftCurlyBracket = 123,
        Pipe = 124,
        RightCurlyBracket = 125,
        Tilde = 126,
        Numlock = 300,
        CapsLock = 301,
        ScrollLock = 302,
        RightShift = 303,
        LeftShift = 304,
        RightControl = 305,
        LeftControl = 306,
        RightAlt = 307,
        LeftAlt = 308,
        LeftCommand = 310,
        LeftWindows = 311,
        RightCommand = 309,
        RightWindows = 312,
        AltGr = 313,
        Help = 315,
        Print = 316,
        SysReq = 317,
        Break = 318,
        Menu = 319,
        Mouse0 = 323,
        Mouse1 = 324,
        Mouse2 = 325,
        Mouse3 = 326,
        Mouse4 = 327,
        Mouse5 = 328,
        Mouse6 = 329,
        /*
        JoystickButton0 = 330,
        JoystickButton1 = 331,
        JoystickButton2 = 332,
        JoystickButton3 = 333,
        JoystickButton4 = 334,
        JoystickButton5 = 335,
        JoystickButton6 = 336,
        JoystickButton7 = 337,
        JoystickButton8 = 338,
        JoystickButton9 = 339,
        JoystickButton10 = 340,
        JoystickButton11 = 341,
        JoystickButton12 = 342,
        JoystickButton13 = 343,
        JoystickButton14 = 344,
        JoystickButton15 = 345,
        JoystickButton16 = 346,
        JoystickButton17 = 347,
        JoystickButton18 = 348,
        JoystickButton19 = 349,
        Joystick1Button0 = 350,
        Joystick1Button1 = 351,
        Joystick1Button2 = 352,
        Joystick1Button3 = 353,
        Joystick1Button4 = 354,
        Joystick1Button5 = 355,
        Joystick1Button6 = 356,
        Joystick1Button7 = 357,
        Joystick1Button8 = 358,
        Joystick1Button9 = 359,
        Joystick1Button10 = 360,
        Joystick1Button11 = 361,
        Joystick1Button12 = 362,
        Joystick1Button13 = 363,
        Joystick1Button14 = 364,
        Joystick1Button15 = 365,
        Joystick1Button16 = 366,
        Joystick1Button17 = 367,
        Joystick1Button18 = 368,
        Joystick1Button19 = 369,
        Joystick2Button0 = 370,
        Joystick2Button1 = 371,
        Joystick2Button2 = 372,
        Joystick2Button3 = 373,
        Joystick2Button4 = 374,
        Joystick2Button5 = 375,
        Joystick2Button6 = 376,
        Joystick2Button7 = 377,
        Joystick2Button8 = 378,
        Joystick2Button9 = 379,
        Joystick2Button10 = 380,
        Joystick2Button11 = 381,
        Joystick2Button12 = 382,
        Joystick2Button13 = 383,
        Joystick2Button14 = 384,
        Joystick2Button15 = 385,
        Joystick2Button16 = 386,
        Joystick2Button17 = 387,
        Joystick2Button18 = 388,
        Joystick2Button19 = 389,
        Joystick3Button0 = 390,
        Joystick3Button1 = 391,
        Joystick3Button2 = 392,
        Joystick3Button3 = 393,
        Joystick3Button4 = 394,
        Joystick3Button5 = 395,
        Joystick3Button6 = 396,
        Joystick3Button7 = 397,
        Joystick3Button8 = 398,
        Joystick3Button9 = 399,
        Joystick3Button10 = 400,
        Joystick3Button11 = 401,
        Joystick3Button12 = 402,
        Joystick3Button13 = 403,
        Joystick3Button14 = 404,
        Joystick3Button15 = 405,
        Joystick3Button16 = 406,
        Joystick3Button17 = 407,
        Joystick3Button18 = 408,
        Joystick3Button19 = 409,
        Joystick4Button0 = 410,
        Joystick4Button1 = 411,
        Joystick4Button2 = 412,
        Joystick4Button3 = 413,
        Joystick4Button4 = 414,
        Joystick4Button5 = 415,
        Joystick4Button6 = 416,
        Joystick4Button7 = 417,
        Joystick4Button8 = 418,
        Joystick4Button9 = 419,
        Joystick4Button10 = 420,
        Joystick4Button11 = 421,
        Joystick4Button12 = 422,
        Joystick4Button13 = 423,
        Joystick4Button14 = 424,
        Joystick4Button15 = 425,
        Joystick4Button16 = 426,
        Joystick4Button17 = 427,
        Joystick4Button18 = 428,
        Joystick4Button19 = 429,
        Joystick5Button0 = 430,
        Joystick5Button1 = 431,
        Joystick5Button2 = 432,
        Joystick5Button3 = 433,
        Joystick5Button4 = 434,
        Joystick5Button5 = 435,
        Joystick5Button6 = 436,
        Joystick5Button7 = 437,
        Joystick5Button8 = 438,
        Joystick5Button9 = 439,
        Joystick5Button10 = 440,
        Joystick5Button11 = 441,
        Joystick5Button12 = 442,
        Joystick5Button13 = 443,
        Joystick5Button14 = 444,
        Joystick5Button15 = 445,
        Joystick5Button16 = 446,
        Joystick5Button17 = 447,
        Joystick5Button18 = 448,
        Joystick5Button19 = 449,
        Joystick6Button0 = 450,
        Joystick6Button1 = 451,
        Joystick6Button2 = 452,
        Joystick6Button3 = 453,
        Joystick6Button4 = 454,
        Joystick6Button5 = 455,
        Joystick6Button6 = 456,
        Joystick6Button7 = 457,
        Joystick6Button8 = 458,
        Joystick6Button9 = 459,
        Joystick6Button10 = 460,
        Joystick6Button11 = 461,
        Joystick6Button12 = 462,
        Joystick6Button13 = 463,
        Joystick6Button14 = 464,
        Joystick6Button15 = 465,
        Joystick6Button16 = 466,
        Joystick6Button17 = 467,
        Joystick6Button18 = 468,
        Joystick6Button19 = 469,
        Joystick7Button0 = 470,
        Joystick7Button1 = 471,
        Joystick7Button2 = 472,
        Joystick7Button3 = 473,
        Joystick7Button4 = 474,
        Joystick7Button5 = 475,
        Joystick7Button6 = 476,
        Joystick7Button7 = 477,
        Joystick7Button8 = 478,
        Joystick7Button9 = 479,
        Joystick7Button10 = 480,
        Joystick7Button11 = 481,
        Joystick7Button12 = 482,
        Joystick7Button13 = 483,
        Joystick7Button14 = 484,
        Joystick7Button15 = 485,
        Joystick7Button16 = 486,
        Joystick7Button17 = 487,
        Joystick7Button18 = 488,
        Joystick7Button19 = 489,
        Joystick8Button0 = 490,
        Joystick8Button1 = 491,
        Joystick8Button2 = 492,
        Joystick8Button3 = 493,
        Joystick8Button4 = 494,
        Joystick8Button5 = 495,
        Joystick8Button6 = 496,
        Joystick8Button7 = 497,
        Joystick8Button8 = 498,
        Joystick8Button9 = 499,
        Joystick8Button10 = 500,
        Joystick8Button11 = 501,
        Joystick8Button12 = 502,
        Joystick8Button13 = 503,
        Joystick8Button14 = 504,
        Joystick8Button15 = 505,
        Joystick8Button16 = 506,
        Joystick8Button17 = 507,
        Joystick8Button18 = 508,
        Joystick8Button19 = 509*/
    }
}