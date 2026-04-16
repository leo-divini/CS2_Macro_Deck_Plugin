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

        // Auth token
        if (root.TryGetProperty("auth", out var auth) &&
            auth.TryGetProperty("token", out var token))
            state.AuthToken = token.GetString() ?? "";

        // Player
        if (root.TryGetProperty("player", out var player))
        {
            if (player.TryGetProperty("state", out var ps))
            {
                state.Player.Hp = ps.TryGetProperty("health", out var h) ? h.GetInt32() : 0;
                state.Player.Armor = ps.TryGetProperty("armor", out var a) ? a.GetInt32() : 0;
                state.Player.Helmet = ps.TryGetProperty("helmet", out var he) && he.GetBoolean();
                state.Player.Money = ps.TryGetProperty("money", out var m) ? m.GetInt32() : 0;
                state.Player.KillsRound = ps.TryGetProperty("round_kills", out var rk) ? rk.GetInt32() : 0;
            }
            state.Player.Alive = state.Player.Hp > 0;
            state.Player.Team = player.TryGetProperty("team", out var t) ? t.GetString() ?? "" : "";

            if (player.TryGetProperty("match_stats", out var stats))
            {
                state.Player.KillsTotal = stats.TryGetProperty("kills", out var kt) ? kt.GetInt32() : 0;
                state.Player.Assists = stats.TryGetProperty("assists", out var ast) ? ast.GetInt32() : 0;
                state.Player.Deaths = stats.TryGetProperty("deaths", out var d) ? d.GetInt32() : 0;
            }

            // Arma attiva
            if (player.TryGetProperty("weapons", out var weapons))
            {
                foreach (var w in weapons.EnumerateObject())
                {
                    if (w.Value.TryGetProperty("state", out var ws) &&
                        string.Equals(ws.GetString(), "active", StringComparison.OrdinalIgnoreCase))
                    {
                        state.Player.ActiveWeapon = w.Value.TryGetProperty("name", out var wn) ? wn.GetString() ?? "" : "";
                        state.Player.WeaponType = w.Value.TryGetProperty("type", out var wt) ? wt.GetString() ?? "" : "";
                        state.Player.AmmoClip = w.Value.TryGetProperty("ammo_clip", out var ac) ? ac.GetInt32() : 0;
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
            state.Map.Round = map.TryGetProperty("round", out var mr) ? mr.GetInt32() : 0;

            if (map.TryGetProperty("team_ct", out var tct) &&
                tct.TryGetProperty("score", out var sct))
                state.Round.WinsCt = sct.GetInt32();

            if (map.TryGetProperty("team_t", out var tt) &&
                tt.TryGetProperty("score", out var st))
                state.Round.WinsT = st.GetInt32();
        }

        // Bomb
        if (root.TryGetProperty("bomb", out var bomb))
        {
            state.Bomb.State = bomb.TryGetProperty("state", out var bs) ? bs.GetString() ?? "" : state.Bomb.State;
            state.Bomb.Position = bomb.TryGetProperty("position", out var bp) ? GetStringValue(bp) : "";
            state.Bomb.Site = state.Bomb.Position;
            state.Bomb.Timer = bomb.TryGetProperty("countdown", out var bc) ? GetStringValue(bc) : "";
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
}
