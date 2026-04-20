using SuchByte.MacroDeck.Variables;

namespace Cs2MacroDeck.Plugin;

internal static class Cs2VariableCatalog
{
    public const string Info = "Info";
    public const string Player = "Player";
    public const string Bomb = "Bomb";
    public const string OtherPlayers = "Other Players";
    public const string Grenades = "Grenades";
    public const string RawDebug = "Raw / Debug";

    public const string ConnectionStatus = "Connection / Status";
    public const string Provider = "Provider";
    public const string Map = "Map";
    public const string Round = "Round";
    public const string PhaseCountdowns = "Phase countdowns";
    public const string MapRoundWins = "Map round wins";
    public const string Identity = "Identity";
    public const string Status = "Status";
    public const string Health = "Health";
    public const string Economy = "Economy";
    public const string MatchStats = "Match stats";
    public const string Position = "Position";
    public const string Weapon = "Weapon";
    public const string WeaponInventory = "Weapon inventory";
    public const string Basic = "Basic";
    public const string PositionCarrier = "Position / carrier";
    public const string General = "General";
    public const string ActiveWeapon = "Active weapon";
    public const string Slots = "Slots";
    public const string RawJson = "Raw JSON";

    private static readonly Lazy<IReadOnlyList<Cs2VariableDefinition>> LazyDefinitions = new(BuildDefinitions);

    public static IReadOnlyList<Cs2VariableDefinition> Definitions => LazyDefinitions.Value;

    public static IReadOnlyDictionary<string, Cs2VariableDefinition> ByName { get; } =
        Definitions.ToDictionary(definition => definition.Name, StringComparer.OrdinalIgnoreCase);

    public static string GroupKey(string category, string group)
    {
        return $"{category}|{group}";
    }

    private static IReadOnlyList<Cs2VariableDefinition> BuildDefinitions()
    {
        var definitions = new List<Cs2VariableDefinition>();

        Add(definitions, "cs2md.connected", VariableType.Bool, Info, ConnectionStatus, true, required: true);
        Add(definitions, "cs2md.status", VariableType.String, Info, ConnectionStatus, true, required: true);

        Add(definitions, "cs2md.provider.name", VariableType.String, Info, Provider, true);
        Add(definitions, "cs2md.provider.appid", VariableType.Integer, Info, Provider, true);
        Add(definitions, "cs2md.provider.version", VariableType.Integer, Info, Provider, true);
        Add(definitions, "cs2md.provider.steamid", VariableType.String, Info, Provider, true);
        Add(definitions, "cs2md.provider.timestamp", VariableType.Integer, Info, Provider, true);

        Add(definitions, "cs2md.map.name", VariableType.String, Info, Map, true);
        Add(definitions, "cs2md.map.mode", VariableType.String, Info, Map, true);
        Add(definitions, "cs2md.map.phase", VariableType.String, Info, Map, true);
        Add(definitions, "cs2md.map.round", VariableType.Integer, Info, Map, true);
        Add(definitions, "cs2md.map.num_matches_to_win_series", VariableType.Integer, Info, Map, false);
        Add(definitions, "cs2md.map.ct.consecutive_round_losses", VariableType.Integer, Info, Map, false);
        Add(definitions, "cs2md.map.ct.timeouts_remaining", VariableType.Integer, Info, Map, false);
        Add(definitions, "cs2md.map.ct.matches_won_this_series", VariableType.Integer, Info, Map, false);
        Add(definitions, "cs2md.map.t.consecutive_round_losses", VariableType.Integer, Info, Map, false);
        Add(definitions, "cs2md.map.t.timeouts_remaining", VariableType.Integer, Info, Map, false);
        Add(definitions, "cs2md.map.t.matches_won_this_series", VariableType.Integer, Info, Map, false);

        Add(definitions, "cs2md.round.phase", VariableType.String, Info, Round, true);
        Add(definitions, "cs2md.round.win_team", VariableType.String, Info, Round, true);
        Add(definitions, "cs2md.round.wins_ct", VariableType.Integer, Info, Round, true);
        Add(definitions, "cs2md.round.wins_t", VariableType.Integer, Info, Round, true);

        Add(definitions, "cs2md.phase_countdowns.phase", VariableType.String, Info, PhaseCountdowns, false);
        Add(definitions, "cs2md.phase_countdowns.ends_in", VariableType.String, Info, PhaseCountdowns, false);

        Add(definitions, "cs2md.rw.count", VariableType.Integer, Info, MapRoundWins, false);
        Add(definitions, "cs2md.rw.history", VariableType.String, Info, MapRoundWins, false);
        Add(definitions, "cs2md.rw.raw_json", VariableType.String, RawDebug, RawJson, false);
        for (var slot = 1; slot <= 30; slot++)
        {
            Add(definitions, $"cs2md.rw.{Slot(slot)}", VariableType.String, Info, MapRoundWins, false);
        }

        Add(definitions, "cs2md.player.steamid", VariableType.String, Player, Identity, true);
        Add(definitions, "cs2md.player.name", VariableType.String, Player, Identity, true);
        Add(definitions, "cs2md.player.observer_slot", VariableType.Integer, Player, Identity, false);
        Add(definitions, "cs2md.player.activity", VariableType.String, Player, Status, true);
        Add(definitions, "cs2md.player.team", VariableType.String, Player, Identity, true);
        Add(definitions, "cs2md.player.hp", VariableType.Integer, Player, Health, true);
        Add(definitions, "cs2md.player.armor", VariableType.Integer, Player, Health, true);
        Add(definitions, "cs2md.player.helmet", VariableType.Bool, Player, Health, true);
        Add(definitions, "cs2md.player.defusekit", VariableType.Bool, Player, Health, true);
        Add(definitions, "cs2md.player.flashed", VariableType.Integer, Player, Status, true);
        Add(definitions, "cs2md.player.smoked", VariableType.Integer, Player, Status, true);
        Add(definitions, "cs2md.player.burning", VariableType.Integer, Player, Status, true);
        Add(definitions, "cs2md.player.alive", VariableType.Bool, Player, Status, true);
        Add(definitions, "cs2md.player.money", VariableType.Integer, Player, Economy, true);
        Add(definitions, "cs2md.player.kills_round", VariableType.Integer, Player, MatchStats, true);
        Add(definitions, "cs2md.player.headshot_kills_round", VariableType.Integer, Player, MatchStats, true);
        Add(definitions, "cs2md.player.kills_total", VariableType.Integer, Player, MatchStats, true);
        Add(definitions, "cs2md.player.assists", VariableType.Integer, Player, MatchStats, true);
        Add(definitions, "cs2md.player.deaths", VariableType.Integer, Player, MatchStats, true);
        Add(definitions, "cs2md.player.mvps", VariableType.Integer, Player, MatchStats, true);
        Add(definitions, "cs2md.player.score", VariableType.Integer, Player, MatchStats, true);
        Add(definitions, "cs2md.player.equip_value", VariableType.Integer, Player, Economy, true);
        Add(definitions, "cs2md.player.position", VariableType.String, Player, Position, false);
        Add(definitions, "cs2md.player.forward", VariableType.String, Player, Position, false);

        Add(definitions, "cs2md.pw.count", VariableType.Integer, Player, WeaponInventory, false);
        Add(definitions, "cs2md.pw.raw_json", VariableType.String, RawDebug, RawJson, false);
        for (var slot = 1; slot <= 8; slot++)
        {
            AddWeapon(definitions, $"cs2md.pw{Slot(slot)}", Player, WeaponInventory, false);
        }

        Add(definitions, "cs2md.weapon.name", VariableType.String, Player, Weapon, true);
        Add(definitions, "cs2md.weapon.type", VariableType.String, Player, Weapon, true);
        Add(definitions, "cs2md.weapon.paintkit", VariableType.String, Player, Weapon, true);
        Add(definitions, "cs2md.weapon.state", VariableType.String, Player, Weapon, true);
        Add(definitions, "cs2md.weapon.ammo_clip", VariableType.Integer, Player, Weapon, true);
        Add(definitions, "cs2md.weapon.ammo_clip_max", VariableType.Integer, Player, Weapon, true);
        Add(definitions, "cs2md.weapon.ammo_reserve", VariableType.Integer, Player, Weapon, true);

        Add(definitions, "cs2md.bomb.state", VariableType.String, Bomb, Basic, true);
        Add(definitions, "cs2md.bomb.timer", VariableType.String, Bomb, Basic, true);
        Add(definitions, "cs2md.bomb.site", VariableType.String, Bomb, PositionCarrier, false);
        Add(definitions, "cs2md.bomb.position", VariableType.String, Bomb, PositionCarrier, false);
        Add(definitions, "cs2md.bomb.carrier", VariableType.String, Bomb, PositionCarrier, false);

        Add(definitions, "cs2md.ap.count", VariableType.Integer, OtherPlayers, General, false);
        Add(definitions, "cs2md.ap.raw_json", VariableType.String, RawDebug, RawJson, false);
        for (var slot = 1; slot <= 10; slot++)
        {
            AddPlayerSlot(definitions, $"cs2md.ap{Slot(slot)}");
        }

        Add(definitions, "cs2md.g.count", VariableType.Integer, Grenades, General, false);
        Add(definitions, "cs2md.g.raw_json", VariableType.String, RawDebug, RawJson, false);
        for (var slot = 1; slot <= 16; slot++)
        {
            AddGrenade(definitions, $"cs2md.g{Slot(slot)}");
        }

        return definitions;
    }

    private static void AddPlayerSlot(List<Cs2VariableDefinition> definitions, string prefix)
    {
        Add(definitions, $"{prefix}.steamid", VariableType.String, OtherPlayers, Identity, false);
        Add(definitions, $"{prefix}.name", VariableType.String, OtherPlayers, Identity, false);
        Add(definitions, $"{prefix}.slot", VariableType.Integer, OtherPlayers, Identity, false);
        Add(definitions, $"{prefix}.activity", VariableType.String, OtherPlayers, Status, false);
        Add(definitions, $"{prefix}.team", VariableType.String, OtherPlayers, Identity, false);
        Add(definitions, $"{prefix}.hp", VariableType.Integer, OtherPlayers, Health, false);
        Add(definitions, $"{prefix}.armor", VariableType.Integer, OtherPlayers, Health, false);
        Add(definitions, $"{prefix}.helmet", VariableType.Bool, OtherPlayers, Health, false);
        Add(definitions, $"{prefix}.defusekit", VariableType.Bool, OtherPlayers, Health, false);
        Add(definitions, $"{prefix}.flashed", VariableType.Integer, OtherPlayers, Status, false);
        Add(definitions, $"{prefix}.smoked", VariableType.Integer, OtherPlayers, Status, false);
        Add(definitions, $"{prefix}.burning", VariableType.Integer, OtherPlayers, Status, false);
        Add(definitions, $"{prefix}.alive", VariableType.Bool, OtherPlayers, Status, false);
        Add(definitions, $"{prefix}.money", VariableType.Integer, OtherPlayers, Economy, false);
        Add(definitions, $"{prefix}.kr", VariableType.Integer, OtherPlayers, MatchStats, false);
        Add(definitions, $"{prefix}.hsr", VariableType.Integer, OtherPlayers, MatchStats, false);
        Add(definitions, $"{prefix}.kt", VariableType.Integer, OtherPlayers, MatchStats, false);
        Add(definitions, $"{prefix}.assists", VariableType.Integer, OtherPlayers, MatchStats, false);
        Add(definitions, $"{prefix}.deaths", VariableType.Integer, OtherPlayers, MatchStats, false);
        Add(definitions, $"{prefix}.mvps", VariableType.Integer, OtherPlayers, MatchStats, false);
        Add(definitions, $"{prefix}.score", VariableType.Integer, OtherPlayers, MatchStats, false);
        Add(definitions, $"{prefix}.equip", VariableType.Integer, OtherPlayers, Economy, false);
        Add(definitions, $"{prefix}.pos", VariableType.String, OtherPlayers, Position, false);
        Add(definitions, $"{prefix}.forward", VariableType.String, OtherPlayers, Position, false);
        Add(definitions, $"{prefix}.aw.name", VariableType.String, OtherPlayers, ActiveWeapon, false);
        Add(definitions, $"{prefix}.aw.type", VariableType.String, OtherPlayers, ActiveWeapon, false);
        Add(definitions, $"{prefix}.aw.paint", VariableType.String, OtherPlayers, ActiveWeapon, false);
        Add(definitions, $"{prefix}.aw.state", VariableType.String, OtherPlayers, ActiveWeapon, false);
        Add(definitions, $"{prefix}.aw.ammo", VariableType.Integer, OtherPlayers, ActiveWeapon, false);
        Add(definitions, $"{prefix}.aw.ammo_max", VariableType.Integer, OtherPlayers, ActiveWeapon, false);
        Add(definitions, $"{prefix}.aw.reserve", VariableType.Integer, OtherPlayers, ActiveWeapon, false);
        Add(definitions, $"{prefix}.w.count", VariableType.Integer, OtherPlayers, WeaponInventory, false);
        Add(definitions, $"{prefix}.w.raw_json", VariableType.String, RawDebug, RawJson, false);

        for (var slot = 1; slot <= 8; slot++)
        {
            AddWeapon(definitions, $"{prefix}.w{Slot(slot)}", OtherPlayers, WeaponInventory, false);
        }
    }

    private static void AddWeapon(
        List<Cs2VariableDefinition> definitions,
        string prefix,
        string category,
        string group,
        bool enabledByDefault)
    {
        Add(definitions, $"{prefix}.slot", VariableType.String, category, group, enabledByDefault);
        Add(definitions, $"{prefix}.name", VariableType.String, category, group, enabledByDefault);
        Add(definitions, $"{prefix}.type", VariableType.String, category, group, enabledByDefault);
        Add(definitions, $"{prefix}.paint", VariableType.String, category, group, enabledByDefault);
        Add(definitions, $"{prefix}.state", VariableType.String, category, group, enabledByDefault);
        Add(definitions, $"{prefix}.ammo", VariableType.Integer, category, group, enabledByDefault);
        Add(definitions, $"{prefix}.ammo_max", VariableType.Integer, category, group, enabledByDefault);
        Add(definitions, $"{prefix}.reserve", VariableType.Integer, category, group, enabledByDefault);
    }

    private static void AddGrenade(List<Cs2VariableDefinition> definitions, string prefix)
    {
        Add(definitions, $"{prefix}.id", VariableType.String, Grenades, Slots, false);
        Add(definitions, $"{prefix}.owner", VariableType.String, Grenades, Slots, false);
        Add(definitions, $"{prefix}.type", VariableType.String, Grenades, Slots, false);
        Add(definitions, $"{prefix}.pos", VariableType.String, Grenades, Slots, false);
        Add(definitions, $"{prefix}.vel", VariableType.String, Grenades, Slots, false);
        Add(definitions, $"{prefix}.life", VariableType.String, Grenades, Slots, false);
        Add(definitions, $"{prefix}.effect", VariableType.String, Grenades, Slots, false);
        Add(definitions, $"{prefix}.flames", VariableType.Integer, Grenades, Slots, false);
        Add(definitions, $"{prefix}.raw_json", VariableType.String, RawDebug, RawJson, false);
    }

    private static void Add(
        List<Cs2VariableDefinition> definitions,
        string name,
        VariableType type,
        string category,
        string group,
        bool enabledByDefault,
        bool required = false)
    {
        definitions.Add(new Cs2VariableDefinition(name, type, category, group, enabledByDefault, required));
    }

    private static string Slot(int slot)
    {
        return slot.ToString("00");
    }
}
