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

        // Auth token
        if (root.TryGetProperty("auth", out var auth) &&
            auth.TryGetProperty("token", out var token))
            state.AuthToken = token.GetString() ?? "";

        // Provider
        if (root.TryGetProperty("provider", out var provider))
        {
            state.Provider.Name = provider.TryGetProperty("name", out var pn) ? pn.GetString() ?? "" : "";
            state.Provider.AppId = provider.TryGetProperty("appid", out var pa) ? pa.GetInt32() : 0;
            state.Provider.Version = provider.TryGetProperty("version", out var pv) ? pv.GetInt32() : 0;
            state.Provider.SteamId = provider.TryGetProperty("steamid", out var ps) ? ps.GetString() ?? "" : "";
            state.Provider.Timestamp = provider.TryGetProperty("timestamp", out var pt) ? pt.GetInt32() : 0;
        }

        // Player
        if (root.TryGetProperty("player", out var player))
        {
            state.Player.SteamId = player.TryGetProperty("steamid", out var sid) ? sid.GetString() ?? "" : "";
            state.Player.Name = player.TryGetProperty("name", out var name) ? name.GetString() ?? "" : "";
            state.Player.ObserverSlot = player.TryGetProperty("observer_slot", out var os) ? os.GetInt32() : 0;
            state.Player.Activity = player.TryGetProperty("activity", out var act) ? act.GetString() ?? "" : "";

            if (player.TryGetProperty("state", out var ps))
            {
                state.Player.Hp = ps.TryGetProperty("health", out var h) ? h.GetInt32() : 0;
                state.Player.Armor = ps.TryGetProperty("armor", out var a) ? a.GetInt32() : 0;
                state.Player.Helmet = ps.TryGetProperty("helmet", out var he) && he.GetBoolean();
                state.Player.DefuseKit = ps.TryGetProperty("defusekit", out var dk) && dk.GetBoolean();
                state.Player.Flashed = ps.TryGetProperty("flashed", out var fl) ? fl.GetInt32() : 0;
                state.Player.Smoked = ps.TryGetProperty("smoked", out var sm) ? sm.GetInt32() : 0;
                state.Player.Burning = ps.TryGetProperty("burning", out var bu) ? bu.GetInt32() : 0;
                state.Player.Money = ps.TryGetProperty("money", out var m) ? m.GetInt32() : 0;
                state.Player.KillsRound = ps.TryGetProperty("round_kills", out var rk) ? rk.GetInt32() : 0;
                state.Player.HeadshotKillsRound = ps.TryGetProperty("round_killhs", out var rh) ? rh.GetInt32() : 0;
                state.Player.EquipValue = ps.TryGetProperty("equip_value", out var ev) ? ev.GetInt32() : 0;
            }
            state.Player.Alive = state.Player.Hp > 0;
            state.Player.Team = player.TryGetProperty("team", out var t) ? t.GetString() ?? "" : "";

            if (player.TryGetProperty("match_stats", out var stats))
            {
                state.Player.KillsTotal = stats.TryGetProperty("kills", out var kt) ? kt.GetInt32() : 0;
                state.Player.Assists = stats.TryGetProperty("assists", out var ast) ? ast.GetInt32() : 0;
                state.Player.Deaths = stats.TryGetProperty("deaths", out var d) ? d.GetInt32() : 0;
                state.Player.Mvps = stats.TryGetProperty("mvps", out var mvps) ? mvps.GetInt32() : 0;
                state.Player.Score = stats.TryGetProperty("score", out var score) ? score.GetInt32() : 0;
            }

            // Arma attiva
            if (player.TryGetProperty("weapons", out var weapons))
            {
                foreach (var w in weapons.EnumerateObject())
                {
                    if (w.Value.TryGetProperty("state", out var ws) &&
                        IsCurrentWeaponState(ws.GetString()))
                    {
                        state.Player.ActiveWeapon = w.Value.TryGetProperty("name", out var wn) ? wn.GetString() ?? "" : "";
                        state.Player.WeaponType = w.Value.TryGetProperty("type", out var wt) ? wt.GetString() ?? "" : "";
                        state.Player.WeaponPaintKit = w.Value.TryGetProperty("paintkit", out var wp) ? wp.GetString() ?? "" : "";
                        state.Player.WeaponState = w.Value.TryGetProperty("state", out var wst) ? wst.GetString() ?? "" : "";
                        state.Player.AmmoClip = w.Value.TryGetProperty("ammo_clip", out var ac) ? ac.GetInt32() : 0;
                        state.Player.AmmoClipMax = w.Value.TryGetProperty("ammo_clip_max", out var am) ? am.GetInt32() : 0;
                        state.Player.AmmoReserve = w.Value.TryGetProperty("ammo_reserve", out var ar) ? ar.GetInt32() : 0;
                        break;
                    }
                }
            }
        }

        // Round
        if (root.TryGetProperty("round", out var round))
        {
            state.Round.Phase = round.TryGetProperty("phase", out var rp) ? rp.GetString() ?? "" : "";
            state.Bomb.State = round.TryGetProperty("bomb", out var rb) ? rb.GetString() ?? "" : "";
        }

        // Map
        if (root.TryGetProperty("map", out var map))
        {
            state.Map.Name = map.TryGetProperty("name", out var mn) ? mn.GetString() ?? "" : "";
            state.Map.Mode = map.TryGetProperty("mode", out var mm) ? mm.GetString() ?? "" : "";
            state.Map.Phase = map.TryGetProperty("phase", out var mp) ? mp.GetString() ?? "" : "";
            state.Map.Round = map.TryGetProperty("round", out var mr) ? mr.GetInt32() : 0;
            state.Map.NumMatchesToWinSeries = map.TryGetProperty("num_matches_to_win_series", out var nm) ? nm.GetInt32() : 0;

            if (map.TryGetProperty("team_ct", out var tct))
            {
                if (tct.TryGetProperty("score", out var sct))
                    state.Round.WinsCt = sct.GetInt32();

                state.Map.Ct.ConsecutiveRoundLosses = tct.TryGetProperty("consecutive_round_losses", out var ctLosses) ? ctLosses.GetInt32() : 0;
                state.Map.Ct.TimeoutsRemaining = tct.TryGetProperty("timeouts_remaining", out var ctTimeouts) ? ctTimeouts.GetInt32() : 0;
                state.Map.Ct.MatchesWonThisSeries = tct.TryGetProperty("matches_won_this_series", out var ctSeries) ? ctSeries.GetInt32() : 0;
            }

            if (map.TryGetProperty("team_t", out var tt))
            {
                if (tt.TryGetProperty("score", out var st))
                    state.Round.WinsT = st.GetInt32();

                state.Map.T.ConsecutiveRoundLosses = tt.TryGetProperty("consecutive_round_losses", out var tLosses) ? tLosses.GetInt32() : 0;
                state.Map.T.TimeoutsRemaining = tt.TryGetProperty("timeouts_remaining", out var tTimeouts) ? tTimeouts.GetInt32() : 0;
                state.Map.T.MatchesWonThisSeries = tt.TryGetProperty("matches_won_this_series", out var tSeries) ? tSeries.GetInt32() : 0;
            }
        }

        // Bomb
        if (root.TryGetProperty("bomb", out var bomb))
        {
            state.Bomb.State = bomb.TryGetProperty("state", out var bs) ? bs.GetString() ?? "" : state.Bomb.State;
            state.Bomb.Position = bomb.TryGetProperty("position", out var bp) ? GetStringValue(bp) : "";
            state.Bomb.Site = state.Bomb.Position;
            state.Bomb.Timer = bomb.TryGetProperty("countdown", out var bc) ? GetStringValue(bc) : "";
            state.Bomb.Carrier = bomb.TryGetProperty("player", out var bpl) ? GetStringValue(bpl) : "";
        }

        return state;
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
            root.TryGetProperty("bomb", out _);
    }
}
