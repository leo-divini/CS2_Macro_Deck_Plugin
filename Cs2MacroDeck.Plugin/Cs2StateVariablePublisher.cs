using System.Net.Http;
using System.Net.Http.Json;
using Cs2Gsi.Core.Config;
using Cs2Gsi.Core.Models;
using SuchByte.MacroDeck.Logging;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Variables;

namespace Cs2MacroDeck.Plugin;

internal static class Cs2StateVariablePublisher
{
    private static readonly Uri StateUri = new(new Uri(GsiDefaults.Prefix()), GsiDefaults.StatePath);
    private static readonly HttpClient HttpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(2)
    };

    private static readonly object SyncRoot = new();
    private static CancellationTokenSource? cancellationTokenSource;
    private static MacroDeckPlugin? plugin;
    private static bool loggedConnectionError;
    private static bool pollingStartedBecausePortInUse;

    public static void Initialize(MacroDeckPlugin macroDeckPlugin)
    {
        lock (SyncRoot)
        {
            plugin = macroDeckPlugin;
            EnsureVariables(macroDeckPlugin);
        }
    }

    public static void StartPolling(MacroDeckPlugin macroDeckPlugin)
    {
        lock (SyncRoot)
        {
            plugin = macroDeckPlugin;
            EnsureVariables(macroDeckPlugin);
            pollingStartedBecausePortInUse = true;

            if (cancellationTokenSource is not null)
            {
                return;
            }

            cancellationTokenSource = new CancellationTokenSource();
            _ = Task.Run(() => PollLoopAsync(cancellationTokenSource.Token));
        }
    }

    public static void StopPolling()
    {
        lock (SyncRoot)
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource = null;
            pollingStartedBecausePortInUse = false;
            loggedConnectionError = false;
        }
    }

    public static void PublishConnected(MacroDeckPlugin currentPlugin, GameState state)
    {
        if (!state.HasPayload)
        {
            PublishWaitingForCs2(currentPlugin);
            return;
        }

        PublishConnectionState(currentPlugin, true, "connected");
        PublishState(currentPlugin, state);
        loggedConnectionError = false;
    }

    public static void PublishWaitingForCs2(MacroDeckPlugin currentPlugin)
    {
        PublishConnectionState(currentPlugin, false, "waiting_for_cs2");
    }

    public static void PublishTokenInvalid(MacroDeckPlugin currentPlugin)
    {
        PublishConnectionState(currentPlugin, false, "token_invalid");
    }

    public static void PublishPortInUse(MacroDeckPlugin currentPlugin)
    {
        PublishConnectionState(currentPlugin, false, "port_in_use");
    }

    public static void PublishOffline(MacroDeckPlugin currentPlugin, string status)
    {
        PublishConnectionState(currentPlugin, false, status);
    }

    public static async Task RefreshOnceAsync()
    {
        var currentPlugin = plugin;
        if (currentPlugin is null)
        {
            return;
        }

        await PublishOnceAsync(currentPlugin, CancellationToken.None).ConfigureAwait(false);
    }

    private static async Task PollLoopAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var currentPlugin = plugin;
            if (currentPlugin is not null)
            {
                await PublishOnceAsync(currentPlugin, cancellationToken).ConfigureAwait(false);
            }

            try
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private static async Task PublishOnceAsync(MacroDeckPlugin currentPlugin, CancellationToken cancellationToken)
    {
        try
        {
            var state = await HttpClient.GetFromJsonAsync<GameState>(StateUri, cancellationToken)
                .ConfigureAwait(false);

            if (state is null)
            {
                PublishConnectionState(currentPlugin, false, EmptyStateStatus());
                return;
            }

            if (!state.HasPayload)
            {
                PublishConnectionState(currentPlugin, false, EmptyStateStatus());
                return;
            }

            PublishConnectionState(currentPlugin, true, "connected");
            PublishState(currentPlugin, state);
            loggedConnectionError = false;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
        {
            PublishConnectionState(currentPlugin, false, "listener_offline");

            if (!loggedConnectionError)
            {
                MacroDeckLogger.Warning(currentPlugin, $"CS2 listener not reachable at {StateUri}.");
                loggedConnectionError = true;
            }
        }
        catch (Exception ex)
        {
            PublishConnectionState(currentPlugin, false, "error");
            MacroDeckLogger.Error(currentPlugin, $"Failed to publish CS2 state: {ex.Message}");
        }
    }

    private static string EmptyStateStatus()
    {
        return pollingStartedBecausePortInUse ? "port_in_use" : "waiting_for_cs2";
    }

    private static void EnsureVariables(MacroDeckPlugin currentPlugin)
    {
        Set(currentPlugin, "cs2md.connected", false, VariableType.Bool);
        Set(currentPlugin, "cs2md.status", "starting", VariableType.String);
        Set(currentPlugin, "cs2md.provider.name", "", VariableType.String);
        Set(currentPlugin, "cs2md.provider.appid", 0, VariableType.Integer);
        Set(currentPlugin, "cs2md.provider.version", 0, VariableType.Integer);
        Set(currentPlugin, "cs2md.provider.steamid", "", VariableType.String);
        Set(currentPlugin, "cs2md.provider.timestamp", 0, VariableType.Integer);
        Set(currentPlugin, "cs2md.map.name", "", VariableType.String);
        Set(currentPlugin, "cs2md.map.mode", "", VariableType.String);
        Set(currentPlugin, "cs2md.map.phase", "", VariableType.String);
        Set(currentPlugin, "cs2md.map.round", 0, VariableType.Integer);
        Set(currentPlugin, "cs2md.map.num_matches_to_win_series", 0, VariableType.Integer);
        Set(currentPlugin, "cs2md.map.ct.consecutive_round_losses", 0, VariableType.Integer);
        Set(currentPlugin, "cs2md.map.ct.timeouts_remaining", 0, VariableType.Integer);
        Set(currentPlugin, "cs2md.map.ct.matches_won_this_series", 0, VariableType.Integer);
        Set(currentPlugin, "cs2md.map.t.consecutive_round_losses", 0, VariableType.Integer);
        Set(currentPlugin, "cs2md.map.t.timeouts_remaining", 0, VariableType.Integer);
        Set(currentPlugin, "cs2md.map.t.matches_won_this_series", 0, VariableType.Integer);
        Set(currentPlugin, "cs2md.round.phase", "", VariableType.String);
        Set(currentPlugin, "cs2md.round.wins_ct", 0, VariableType.Integer);
        Set(currentPlugin, "cs2md.round.wins_t", 0, VariableType.Integer);
        Set(currentPlugin, "cs2md.player.steamid", "", VariableType.String);
        Set(currentPlugin, "cs2md.player.name", "", VariableType.String);
        Set(currentPlugin, "cs2md.player.observer_slot", 0, VariableType.Integer);
        Set(currentPlugin, "cs2md.player.activity", "", VariableType.String);
        Set(currentPlugin, "cs2md.player.hp", 0, VariableType.Integer);
        Set(currentPlugin, "cs2md.player.armor", 0, VariableType.Integer);
        Set(currentPlugin, "cs2md.player.helmet", false, VariableType.Bool);
        Set(currentPlugin, "cs2md.player.defusekit", false, VariableType.Bool);
        Set(currentPlugin, "cs2md.player.flashed", 0, VariableType.Integer);
        Set(currentPlugin, "cs2md.player.smoked", 0, VariableType.Integer);
        Set(currentPlugin, "cs2md.player.burning", 0, VariableType.Integer);
        Set(currentPlugin, "cs2md.player.alive", false, VariableType.Bool);
        Set(currentPlugin, "cs2md.player.money", 0, VariableType.Integer);
        Set(currentPlugin, "cs2md.player.team", "", VariableType.String);
        Set(currentPlugin, "cs2md.player.kills_round", 0, VariableType.Integer);
        Set(currentPlugin, "cs2md.player.headshot_kills_round", 0, VariableType.Integer);
        Set(currentPlugin, "cs2md.player.kills_total", 0, VariableType.Integer);
        Set(currentPlugin, "cs2md.player.assists", 0, VariableType.Integer);
        Set(currentPlugin, "cs2md.player.deaths", 0, VariableType.Integer);
        Set(currentPlugin, "cs2md.player.mvps", 0, VariableType.Integer);
        Set(currentPlugin, "cs2md.player.score", 0, VariableType.Integer);
        Set(currentPlugin, "cs2md.player.equip_value", 0, VariableType.Integer);
        Set(currentPlugin, "cs2md.weapon.name", "", VariableType.String);
        Set(currentPlugin, "cs2md.weapon.type", "", VariableType.String);
        Set(currentPlugin, "cs2md.weapon.paintkit", "", VariableType.String);
        Set(currentPlugin, "cs2md.weapon.state", "", VariableType.String);
        Set(currentPlugin, "cs2md.weapon.ammo_clip", 0, VariableType.Integer);
        Set(currentPlugin, "cs2md.weapon.ammo_clip_max", 0, VariableType.Integer);
        Set(currentPlugin, "cs2md.weapon.ammo_reserve", 0, VariableType.Integer);
        Set(currentPlugin, "cs2md.bomb.state", "", VariableType.String);
        Set(currentPlugin, "cs2md.bomb.site", "", VariableType.String);
        Set(currentPlugin, "cs2md.bomb.position", "", VariableType.String);
        Set(currentPlugin, "cs2md.bomb.timer", "", VariableType.String);
        Set(currentPlugin, "cs2md.bomb.carrier", "", VariableType.String);
    }

    private static void PublishConnectionState(MacroDeckPlugin currentPlugin, bool connected, string status)
    {
        Set(currentPlugin, "cs2md.connected", connected, VariableType.Bool);
        Set(currentPlugin, "cs2md.status", status, VariableType.String);
    }

    private static void PublishState(MacroDeckPlugin currentPlugin, GameState state)
    {
        Set(currentPlugin, "cs2md.provider.name", state.Provider.Name, VariableType.String);
        Set(currentPlugin, "cs2md.provider.appid", state.Provider.AppId, VariableType.Integer);
        Set(currentPlugin, "cs2md.provider.version", state.Provider.Version, VariableType.Integer);
        Set(currentPlugin, "cs2md.provider.steamid", state.Provider.SteamId, VariableType.String);
        Set(currentPlugin, "cs2md.provider.timestamp", state.Provider.Timestamp, VariableType.Integer);
        Set(currentPlugin, "cs2md.map.name", state.Map.Name, VariableType.String);
        Set(currentPlugin, "cs2md.map.mode", state.Map.Mode, VariableType.String);
        Set(currentPlugin, "cs2md.map.phase", state.Map.Phase, VariableType.String);
        Set(currentPlugin, "cs2md.map.round", state.Map.Round, VariableType.Integer);
        Set(currentPlugin, "cs2md.map.num_matches_to_win_series", state.Map.NumMatchesToWinSeries, VariableType.Integer);
        Set(currentPlugin, "cs2md.map.ct.consecutive_round_losses", state.Map.Ct.ConsecutiveRoundLosses, VariableType.Integer);
        Set(currentPlugin, "cs2md.map.ct.timeouts_remaining", state.Map.Ct.TimeoutsRemaining, VariableType.Integer);
        Set(currentPlugin, "cs2md.map.ct.matches_won_this_series", state.Map.Ct.MatchesWonThisSeries, VariableType.Integer);
        Set(currentPlugin, "cs2md.map.t.consecutive_round_losses", state.Map.T.ConsecutiveRoundLosses, VariableType.Integer);
        Set(currentPlugin, "cs2md.map.t.timeouts_remaining", state.Map.T.TimeoutsRemaining, VariableType.Integer);
        Set(currentPlugin, "cs2md.map.t.matches_won_this_series", state.Map.T.MatchesWonThisSeries, VariableType.Integer);
        Set(currentPlugin, "cs2md.round.phase", state.Round.Phase, VariableType.String);
        Set(currentPlugin, "cs2md.round.wins_ct", state.Round.WinsCt, VariableType.Integer);
        Set(currentPlugin, "cs2md.round.wins_t", state.Round.WinsT, VariableType.Integer);
        Set(currentPlugin, "cs2md.player.steamid", state.Player.SteamId, VariableType.String);
        Set(currentPlugin, "cs2md.player.name", state.Player.Name, VariableType.String);
        Set(currentPlugin, "cs2md.player.observer_slot", state.Player.ObserverSlot, VariableType.Integer);
        Set(currentPlugin, "cs2md.player.activity", state.Player.Activity, VariableType.String);
        Set(currentPlugin, "cs2md.player.hp", state.Player.Hp, VariableType.Integer);
        Set(currentPlugin, "cs2md.player.armor", state.Player.Armor, VariableType.Integer);
        Set(currentPlugin, "cs2md.player.helmet", state.Player.Helmet, VariableType.Bool);
        Set(currentPlugin, "cs2md.player.defusekit", state.Player.DefuseKit, VariableType.Bool);
        Set(currentPlugin, "cs2md.player.flashed", state.Player.Flashed, VariableType.Integer);
        Set(currentPlugin, "cs2md.player.smoked", state.Player.Smoked, VariableType.Integer);
        Set(currentPlugin, "cs2md.player.burning", state.Player.Burning, VariableType.Integer);
        Set(currentPlugin, "cs2md.player.alive", state.Player.Alive, VariableType.Bool);
        Set(currentPlugin, "cs2md.player.money", state.Player.Money, VariableType.Integer);
        Set(currentPlugin, "cs2md.player.team", state.Player.Team, VariableType.String);
        Set(currentPlugin, "cs2md.player.kills_round", state.Player.KillsRound, VariableType.Integer);
        Set(currentPlugin, "cs2md.player.headshot_kills_round", state.Player.HeadshotKillsRound, VariableType.Integer);
        Set(currentPlugin, "cs2md.player.kills_total", state.Player.KillsTotal, VariableType.Integer);
        Set(currentPlugin, "cs2md.player.assists", state.Player.Assists, VariableType.Integer);
        Set(currentPlugin, "cs2md.player.deaths", state.Player.Deaths, VariableType.Integer);
        Set(currentPlugin, "cs2md.player.mvps", state.Player.Mvps, VariableType.Integer);
        Set(currentPlugin, "cs2md.player.score", state.Player.Score, VariableType.Integer);
        Set(currentPlugin, "cs2md.player.equip_value", state.Player.EquipValue, VariableType.Integer);
        Set(currentPlugin, "cs2md.weapon.name", state.Player.ActiveWeapon, VariableType.String);
        Set(currentPlugin, "cs2md.weapon.type", state.Player.WeaponType, VariableType.String);
        Set(currentPlugin, "cs2md.weapon.paintkit", state.Player.WeaponPaintKit, VariableType.String);
        Set(currentPlugin, "cs2md.weapon.state", state.Player.WeaponState, VariableType.String);
        Set(currentPlugin, "cs2md.weapon.ammo_clip", state.Player.AmmoClip, VariableType.Integer);
        Set(currentPlugin, "cs2md.weapon.ammo_clip_max", state.Player.AmmoClipMax, VariableType.Integer);
        Set(currentPlugin, "cs2md.weapon.ammo_reserve", state.Player.AmmoReserve, VariableType.Integer);
        Set(currentPlugin, "cs2md.bomb.state", state.Bomb.State, VariableType.String);
        Set(currentPlugin, "cs2md.bomb.site", state.Bomb.Site, VariableType.String);
        Set(currentPlugin, "cs2md.bomb.position", state.Bomb.Position, VariableType.String);
        Set(currentPlugin, "cs2md.bomb.timer", state.Bomb.Timer, VariableType.String);
        Set(currentPlugin, "cs2md.bomb.carrier", state.Bomb.Carrier, VariableType.String);
    }

    private static void Set(
        MacroDeckPlugin currentPlugin,
        string name,
        object value,
        VariableType type)
    {
        VariableManager.SetValue(name, value, type, currentPlugin, Array.Empty<string>());
    }
}
