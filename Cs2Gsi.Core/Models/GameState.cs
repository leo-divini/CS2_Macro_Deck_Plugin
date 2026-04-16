namespace Cs2Gsi.Core.Models;

public class GameState
{
    public PlayerState Player { get; set; } = new();
    public RoundState Round { get; set; } = new();
    public BombState Bomb { get; set; } = new();
    public MapState Map { get; set; } = new();
    public string AuthToken { get; set; } = "";
}

public class PlayerState
{
    public int Hp { get; set; }
    public int Armor { get; set; }
    public bool Helmet { get; set; }
    public bool Alive { get; set; }
    public int Money { get; set; }
    public int KillsRound { get; set; }
    public int KillsTotal { get; set; }
    public int Assists { get; set; }
    public int Deaths { get; set; }
    public string Team { get; set; } = "";
    public string ActiveWeapon { get; set; } = "";
    public string WeaponType { get; set; } = "";
    public int AmmoClip { get; set; }
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
    public string Timer { get; set; } = "";
    public DateTime? PlantedAt { get; set; }
}

public class MapState
{
    public string Name { get; set; } = "";
    public string Mode { get; set; } = "";
    public int Round { get; set; }
}