using System.Text.Json;
using Cs2Gsi.Core.Models;

namespace Cs2Gsi.Core.Parsing;

public static class GsiPayloadParser
{
    public static GameState Parse(string json)
    {
        var state = new GameState();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        state.HasPayload = HasGameStatePayload(root);

        if (root.TryGetProperty("auth", out var auth) &&
            auth.TryGetProperty("token", out var token))
        {
            state.AuthToken = GetStringValue(token);
        }

        ParseProvider(root, state);
        ParsePlayer(root, state);
        ParseRound(root, state);
        ParseMap(root, state);
        ParseMapRoundWins(root, state);
        ParsePhaseCountdowns(root, state);
        ParseBomb(root, state);
        ParseAllPlayers(root, state);
        ParseGrenades(root, state);

        return state;
    }

    private static void ParseProvider(JsonElement root, GameState state)
    {
        if (!root.TryGetProperty("provider", out var provider))
        {
            return;
        }

        state.Provider.Name = GetStringProperty(provider, "name");
        state.Provider.AppId = GetIntProperty(provider, "appid");
        state.Provider.Version = GetIntProperty(provider, "version");
        state.Provider.SteamId = GetStringProperty(provider, "steamid");
        state.Provider.Timestamp = GetIntProperty(provider, "timestamp");
    }

    private static void ParsePlayer(JsonElement root, GameState state)
    {
        if (root.TryGetProperty("player", out var player))
        {
            state.Player = ParsePlayerState(player);
        }
    }

    private static PlayerState ParsePlayerState(JsonElement player, string fallbackSteamId = "")
    {
        var parsed = new PlayerState
        {
            SteamId = GetStringProperty(player, "steamid", fallbackSteamId),
            Name = GetStringProperty(player, "name"),
            ObserverSlot = GetIntProperty(player, "observer_slot"),
            Activity = GetStringProperty(player, "activity"),
            Team = GetStringProperty(player, "team"),
            Position = GetStringProperty(player, "position"),
            Forward = GetStringProperty(player, "forward")
        };

        if (player.TryGetProperty("state", out var playerState))
        {
            parsed.Hp = GetIntProperty(playerState, "health");
            parsed.Armor = GetIntProperty(playerState, "armor");
            parsed.Helmet = GetBoolProperty(playerState, "helmet");
            parsed.DefuseKit = GetBoolProperty(playerState, "defusekit");
            parsed.Flashed = GetIntProperty(playerState, "flashed");
            parsed.Smoked = GetIntProperty(playerState, "smoked");
            parsed.Burning = GetIntProperty(playerState, "burning");
            parsed.Money = GetIntProperty(playerState, "money");
            parsed.KillsRound = GetIntProperty(playerState, "round_kills");
            parsed.HeadshotKillsRound = GetIntProperty(playerState, "round_killhs");
            parsed.EquipValue = GetIntProperty(playerState, "equip_value");
        }

        parsed.Alive = parsed.Hp > 0;

        if (player.TryGetProperty("match_stats", out var stats))
        {
            parsed.KillsTotal = GetIntProperty(stats, "kills");
            parsed.Assists = GetIntProperty(stats, "assists");
            parsed.Deaths = GetIntProperty(stats, "deaths");
            parsed.Mvps = GetIntProperty(stats, "mvps");
            parsed.Score = GetIntProperty(stats, "score");
        }

        if (player.TryGetProperty("weapons", out var weapons))
        {
            parsed.WeaponsRawJson = GetRawJson(weapons);
            parsed.Weapons = ParseWeapons(weapons);

            var currentWeapon = parsed.Weapons.FirstOrDefault(w => IsCurrentWeaponState(w.State)) ??
                parsed.Weapons.FirstOrDefault();
            if (currentWeapon is not null)
            {
                parsed.ActiveWeapon = currentWeapon.Name;
                parsed.WeaponType = currentWeapon.Type;
                parsed.WeaponPaintKit = currentWeapon.PaintKit;
                parsed.WeaponState = currentWeapon.State;
                parsed.AmmoClip = currentWeapon.AmmoClip;
                parsed.AmmoClipMax = currentWeapon.AmmoClipMax;
                parsed.AmmoReserve = currentWeapon.AmmoReserve;
            }
        }

        return parsed;
    }

    private static List<WeaponInfo> ParseWeapons(JsonElement weapons)
    {
        var result = new List<WeaponInfo>();
        if (weapons.ValueKind != JsonValueKind.Object)
        {
            return result;
        }

        foreach (var weapon in weapons.EnumerateObject())
        {
            if (weapon.Value.ValueKind != JsonValueKind.Object)
            {
                continue;
            }

            result.Add(new WeaponInfo
            {
                Slot = weapon.Name,
                Name = GetStringProperty(weapon.Value, "name"),
                Type = GetStringProperty(weapon.Value, "type"),
                PaintKit = GetStringProperty(weapon.Value, "paintkit"),
                State = GetStringProperty(weapon.Value, "state"),
                AmmoClip = GetIntProperty(weapon.Value, "ammo_clip"),
                AmmoClipMax = GetIntProperty(weapon.Value, "ammo_clip_max"),
                AmmoReserve = GetIntProperty(weapon.Value, "ammo_reserve")
            });
        }

        return result;
    }

    private static void ParseRound(JsonElement root, GameState state)
    {
        if (!root.TryGetProperty("round", out var round))
        {
            return;
        }

        state.Round.Phase = GetStringProperty(round, "phase");
        state.Round.WinTeam = GetStringProperty(round, "win_team");

        var roundBomb = GetStringProperty(round, "bomb");
        if (!string.IsNullOrEmpty(roundBomb))
        {
            state.Bomb.State = roundBomb;
        }
    }

    private static void ParseMap(JsonElement root, GameState state)
    {
        if (!root.TryGetProperty("map", out var map))
        {
            return;
        }

        state.Map.Name = GetStringProperty(map, "name");
        state.Map.Mode = GetStringProperty(map, "mode");
        state.Map.Phase = GetStringProperty(map, "phase");
        state.Map.Round = GetIntProperty(map, "round");
        state.Map.NumMatchesToWinSeries = GetIntProperty(map, "num_matches_to_win_series");

        if (map.TryGetProperty("team_ct", out var ct))
        {
            state.Round.WinsCt = GetIntProperty(ct, "score");
            state.Map.Ct.ConsecutiveRoundLosses = GetIntProperty(ct, "consecutive_round_losses");
            state.Map.Ct.TimeoutsRemaining = GetIntProperty(ct, "timeouts_remaining");
            state.Map.Ct.MatchesWonThisSeries = GetIntProperty(ct, "matches_won_this_series");
        }

        if (map.TryGetProperty("team_t", out var t))
        {
            state.Round.WinsT = GetIntProperty(t, "score");
            state.Map.T.ConsecutiveRoundLosses = GetIntProperty(t, "consecutive_round_losses");
            state.Map.T.TimeoutsRemaining = GetIntProperty(t, "timeouts_remaining");
            state.Map.T.MatchesWonThisSeries = GetIntProperty(t, "matches_won_this_series");
        }
    }

    private static void ParseMapRoundWins(JsonElement root, GameState state)
    {
        JsonElement mapRoundWins;
        if (root.TryGetProperty("map_round_wins", out var topLevel))
        {
            mapRoundWins = topLevel;
        }
        else if (root.TryGetProperty("map", out var map) &&
            map.TryGetProperty("round_wins", out var nested))
        {
            mapRoundWins = nested;
        }
        else
        {
            return;
        }

        state.MapRoundWins.RawJson = GetRawJson(mapRoundWins);
        state.MapRoundWins.Wins = ParseStringList(mapRoundWins);
        state.MapRoundWins.History = string.Join(",", state.MapRoundWins.Wins);
    }

    private static void ParsePhaseCountdowns(JsonElement root, GameState state)
    {
        if (!root.TryGetProperty("phase_countdowns", out var countdowns))
        {
            return;
        }

        state.PhaseCountdowns.Phase = GetStringProperty(countdowns, "phase");
        state.PhaseCountdowns.PhaseEndsIn = GetStringProperty(countdowns, "phase_ends_in");
    }

    private static void ParseBomb(JsonElement root, GameState state)
    {
        if (!root.TryGetProperty("bomb", out var bomb))
        {
            return;
        }

        var bombState = GetStringProperty(bomb, "state");
        if (!string.IsNullOrEmpty(bombState))
        {
            state.Bomb.State = bombState;
        }

        state.Bomb.Position = GetStringProperty(bomb, "position");
        state.Bomb.Site = state.Bomb.Position;
        state.Bomb.Timer = GetStringProperty(bomb, "countdown");
        state.Bomb.Carrier = GetStringProperty(bomb, "player");
    }

    private static void ParseAllPlayers(JsonElement root, GameState state)
    {
        if (!root.TryGetProperty("allplayers", out var allPlayers) ||
            allPlayers.ValueKind != JsonValueKind.Object)
        {
            return;
        }

        state.AllPlayersRawJson = GetRawJson(allPlayers);
        foreach (var player in allPlayers.EnumerateObject())
        {
            if (player.Value.ValueKind == JsonValueKind.Object)
            {
                state.AllPlayers.Add(ParsePlayerState(player.Value, player.Name));
            }
        }

        state.AllPlayers = state.AllPlayers
            .OrderBy(p => p.ObserverSlot <= 0 ? int.MaxValue : p.ObserverSlot)
            .ThenBy(p => p.Team)
            .ThenBy(p => p.Name)
            .ToList();
    }

    private static void ParseGrenades(JsonElement root, GameState state)
    {
        JsonElement grenades;
        if (root.TryGetProperty("grenades", out var standard))
        {
            grenades = standard;
        }
        else if (root.TryGetProperty("allgrenades", out var namedLikeConfig))
        {
            grenades = namedLikeConfig;
        }
        else
        {
            return;
        }

        if (grenades.ValueKind != JsonValueKind.Object)
        {
            return;
        }

        state.GrenadesRawJson = GetRawJson(grenades);
        foreach (var grenade in grenades.EnumerateObject())
        {
            if (grenade.Value.ValueKind != JsonValueKind.Object)
            {
                continue;
            }

            state.Grenades.Add(new GrenadeState
            {
                Id = grenade.Name,
                Owner = GetStringProperty(grenade.Value, "owner"),
                Type = GetStringProperty(grenade.Value, "type"),
                Position = GetStringProperty(grenade.Value, "position"),
                Velocity = GetStringProperty(grenade.Value, "velocity"),
                Lifetime = GetStringProperty(grenade.Value, "lifetime"),
                EffectTime = GetStringProperty(grenade.Value, "effecttime"),
                FlamesCount = CountObjectProperties(grenade.Value, "flames"),
                RawJson = GetRawJson(grenade.Value)
            });
        }
    }

    private static List<string> ParseStringList(JsonElement element)
    {
        var result = new List<string>();
        switch (element.ValueKind)
        {
            case JsonValueKind.Array:
                result.AddRange(element.EnumerateArray().Select(GetStringValue));
                break;
            case JsonValueKind.Object:
                result.AddRange(element.EnumerateObject().Select(item => GetStringValue(item.Value)));
                break;
        }

        return result;
    }

    private static int CountObjectProperties(JsonElement parent, string propertyName)
    {
        return parent.TryGetProperty(propertyName, out var property) &&
            property.ValueKind == JsonValueKind.Object
            ? property.EnumerateObject().Count()
            : 0;
    }

    private static string GetStringProperty(JsonElement parent, string propertyName, string fallback = "")
    {
        return parent.TryGetProperty(propertyName, out var property)
            ? GetStringValue(property)
            : fallback;
    }

    private static int GetIntProperty(JsonElement parent, string propertyName)
    {
        if (!parent.TryGetProperty(propertyName, out var property))
        {
            return 0;
        }

        return property.ValueKind switch
        {
            JsonValueKind.Number when property.TryGetInt32(out var value) => value,
            JsonValueKind.String when int.TryParse(property.GetString(), out var value) => value,
            _ => 0
        };
    }

    private static bool GetBoolProperty(JsonElement parent, string propertyName)
    {
        if (!parent.TryGetProperty(propertyName, out var property))
        {
            return false;
        }

        return property.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.String when bool.TryParse(property.GetString(), out var value) => value,
            JsonValueKind.Number when property.TryGetInt32(out var value) => value != 0,
            _ => false
        };
    }

    private static string GetStringValue(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString() ?? "",
            JsonValueKind.Number => element.GetRawText(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            _ => ""
        };
    }

    private static string GetRawJson(JsonElement element)
    {
        return element.GetRawText();
    }

    private static bool IsCurrentWeaponState(string? state)
    {
        return string.Equals(state, "active", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(state, "reloading", StringComparison.OrdinalIgnoreCase);
    }

    private static bool HasGameStatePayload(JsonElement root)
    {
        return root.TryGetProperty("player", out _) ||
            root.TryGetProperty("round", out _) ||
            root.TryGetProperty("map", out _) ||
            root.TryGetProperty("bomb", out _) ||
            root.TryGetProperty("phase_countdowns", out _) ||
            root.TryGetProperty("map_round_wins", out _) ||
            root.TryGetProperty("allplayers", out _) ||
            root.TryGetProperty("grenades", out _) ||
            root.TryGetProperty("allgrenades", out _);
    }
}
