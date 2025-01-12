namespace ToolModData
{
    public interface ISyncData
    {
        public int ID { get; }
    }

    /// <summary>
    /// modifier<-game
    /// 0
    /// </summary>
    [Serializable]
    public struct InitData : ISyncData
    {
        public readonly int ID => 0;
        public string[] AdvBuffs { get; set; }
        public string[] UltiBuffs { get; set; }
        public Dictionary<int, string> Plants { get; set; }
        public Dictionary<int, string> Zombies { get; set; }
        public Dictionary<int, string> Bullets { get; set; }
        public Dictionary<int, string> FirstArmors { get; set; }
        public Dictionary<int, string> SecondArmors { get; set; }
    }

    /// <summary>
    /// modifier<->game
    /// 2
    /// </summary>
    [Serializable]
    public struct BasicProperties : ISyncData
    {
        public readonly int ID => 2;
        public bool? DeveloperMode { get; set; }
        public bool? PlantingNoCD { get; set; }
        public bool? FreePlanting { get; set; }
        public bool? UnlockAllFusions { get; set; }
        public bool? HammerNoCD { get; set; }
        public bool? GloveNoCD { get; set; }
        public bool? SuperPresent { get; set; }
        public bool? UltimateRamdomZombie { get; set; }
        public bool? PresentFastOpen { get; set; }
        public int? LockPresent { get; set; }
        public bool? FastShooting { get; set; }
        public bool? HardPlant { get; set; }
        public bool? NoHole { get; set; }
        public bool? MineNoCD { get; set; }
        public bool? ChomperNoCD { get; set; }
        public bool? HyponoEmperorNoCD { get; set; }
        public bool? CobCannonNoCD { get; set; }
        public bool? NoIceRoad { get; set; }
        public bool? ItemExistForever { get; set; }
        public bool? CardNoInit { get; set; }
        public bool? JackboxNotExplode { get; set; }
        public bool? UndeadBullet { get; set; }
        public bool? GarlicDay { get; set; }
    }

    /// <summary>
    /// modifier->game
    /// 1
    /// </summary>
    [Serializable]
    public struct ValueProperties : ISyncData
    {
        public readonly int ID => 1;
        public KeyValuePair<int, int>? PlantsHealth { get; set; }
        public KeyValuePair<int, int>? ZombiesHealth { get; set; }
        public KeyValuePair<int, int>? FirstArmorsHealth { get; set; }
        public KeyValuePair<int, int>? SecondArmorsHealth { get; set; }
        public int? LockAllBullet { get; set; }
        public KeyValuePair<int, int>? BulletsDamage { get; set; }
    }

    /// <summary>
    /// modifier<->game
    /// 3
    /// </summary>
    [Serializable]
    public struct SyncProperties : ISyncData
    {
        public readonly int ID => 3;
        public int GameStatus { get; set; }
        public int GameSpeed { get; set; }
    }

    /// <summary>
    /// modifier<->game
    /// 4
    /// </summary>
    [Serializable]
    public struct SyncTravelBuff : ISyncData
    {
        public readonly int ID => 4;
        public List<bool>? AdvTravelBuff { get; set; }
        public List<bool>? UltiTravelBuff { get; set; }
        public List<bool>? AdvInGame { get; set; }
        public List<bool>? UltiInGame { get; set; }
    }

    /// <summary>
    /// modifier->game
    /// 5
    /// </summary>
    [Serializable]
    public struct CardProperties : ISyncData
    {
        public readonly int ID => 5;
        public List<Card>? CardReplaces { get; set; }
    }

    /// <summary>
    /// 7
    /// </summary>
    [Serializable]
    public struct GameModes : ISyncData
    {
        public GameModes()
        {
        }

        public readonly int ID => 7;
        public bool ScaredyDream { get; set; } = false;
        public bool ColumnPlanting { get; set; } = false;
        public bool SeedRain { get; set; } = false;
        public bool Exchange { get; set; } = false;
        public bool Shooting1 { get; set; } = false;
        public bool Shooting2 { get; set; } = false;
        public bool Shooting3 { get; set; } = false;
        public bool Shooting4 { get; set; } = false;

        public readonly bool IsShooting() => Shooting1 || Shooting2 || Shooting3 || Shooting4;
    }

    /// <summary>
    /// modifier->game
    /// 6
    /// </summary>
    [Serializable]
    public struct InGameActions : ISyncData
    {
        public readonly int ID => 6;
        public bool? Card { get; set; }
        public int? Times { get; set; }
        public int? Row { get; set; }
        public int? Column { get; set; }
        public int? PlantType { get; set; }
        public int? ZombieType { get; set; }
        public int? ItemType { get; set; }
        public bool? SummonMindControlledZombies { get; set; }
        public List<int>? ZombieSeaTypes { get; set; }
        public bool? ZombieSea { get; set; }
        public int? ZombieSeaCD { get; set; }
        public bool? CreatePassiveMateorite { get; set; }
        public bool? CreateActiveMateorite { get; set; }
        public bool? CreateUltimateMateorite { get; set; }
        public bool? LockSun { get; set; }
        public int? CurrentSun { get; set; }
        public bool? LockMoney { get; set; }
        public int? CurrentMoney { get; set; }
        public bool? NextWave { get; set; }
        public bool? StopSummon { get; set; }
        public bool? NoFail { get; set; }
        public bool? ClearAllPlants { get; set; }
        public bool? ClearAllZombies { get; set; }
        public bool? MindControlAllZombies { get; set; }
        public bool? Win { get; set; }
        public bool? ClearAllIceRoads { get; set; }
        public bool? ClearAllHoles { get; set; }
        public bool? ReadField { get; set; }
        public bool? ReadZombies { get; set; }
        public string? WriteField { get; set; }
        public string? WriteZombies { get; set; }
        public bool? ClearOnWritingField { get; set; }
        public bool? ClearOnWritingZombies { get; set; }
        public string? ChangeLevelName { get; set; }
        public string? ShowText { get; set; }
    }

    [Serializable]
    public struct Key : ISyncData
    {
        public readonly int ID => 8;
        public int? KeyCode { get; set; }
    }

    [Serializable]
    public struct Exit : ISyncData
    {
        public readonly int ID => 16;
    }

    [Serializable]
    public struct Card
    {
        public bool Enabled { get; set; }
        public int NewID { get; set; }
        public int ID { get; set; }
        public int Sun { get; set; }
        public float CD { get; set; }
    }

    [Serializable]
    public struct PlantInfo
    {
        public int ID { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public int LilyType { get; set; }
    }

    [Serializable]
    public struct ZombieInfo
    {
        public int ID { get; set; }
        public float X { get; set; }
        public int Row { get; set; }
    }
}