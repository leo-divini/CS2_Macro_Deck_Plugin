namespace Cs2Gsi.Core.Models;

public class GameState
{
    public bool HasPayload { get; set; }
    public ProviderState Provider { get; set; } = new();
    public PlayerState Player { get; set; } = new();
    public RoundState Round { get; set; } = new();
    public BombState Bomb { get; set; } = new();
    public MapState Map { get; set; } = new();
    public string AuthToken { get; set; } = "";
}

public class ProviderState
{
    public string Name { get; set; } = "";
    public int AppId { get; set; }
    public int Version { get; set; }
    public string SteamId { get; set; } = "";
    public int Timestamp { get; set; }
}

public class PlayerState
{
    public string SteamId { get; set; } = "";
    public string Name { get; set; } = "";
    public int ObserverSlot { get; set; }
    public string Activity { get; set; } = "";
    public int Hp { get; set; }
    public int Armor { get; set; }
    public bool Helmet { get; set; }
    public bool DefuseKit { get; set; }
    public int Flashed { get; set; }
    public int Smoked { get; set; }
    public int Burning { get; set; }
    public bool Alive { get; set; }
    public int Money { get; set; }
    public int KillsRound { get; set; }
    public int HeadshotKillsRound { get; set; }
    public int KillsTotal { get; set; }
    public int Assists { get; set; }
    public int Deaths { get; set; }
    public int Mvps { get; set; }
    public int Score { get; set; }
    public int EquipValue { get; set; }
    public string Team { get; set; } = "";
    public string ActiveWeapon { get; set; } = "";
    public string WeaponType { get; set; } = "";
    public string WeaponPaintKit { get; set; } = "";
    public string WeaponState { get; set; } = "";
    public int AmmoClip { get; set; }
    public int AmmoClipMax { get; set; }
    public int AmmoReserve { get; set; }
}

public class RoundState
{
    public string Phase { get; set; } = "";
    public int WinsCt { get; set; }
    public int WinsT { get; set; }
}

public class BombState
{
    public string State { get; set; } = "";
    public string Site { get; set; } = "";
    public string Position { get; set; } = "";
    public string Timer { get; set; } = "";
    public string Carrier { get; set; } = "";
    public DateTime? PlantedAt { get; set; }
}

public class MapState
{
    public string Name { get; set; } = "";
    public string Mode { get; set; } = "";
    public string Phase { get; set; } = "";
    public int Round { get; set; }
    public int NumMatchesToWinSeries { get; set; }
    public TeamMapState Ct { get; set; } = new();
    public TeamMapState T { get; set; } = new();
}

public class TeamMapState
{
    public int ConsecutiveRoundLosses { get; set; }
    public int TimeoutsRemaining { get; set; }
    public int MatchesWonThisSeries { get; set; }
}
