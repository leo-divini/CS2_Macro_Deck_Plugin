using Cs2Gsi.Core.Config;
using Cs2Gsi.Core.Http;
using Cs2Gsi.Core.Models;
using Cs2Gsi.Listener.Config;

var config = new ListenerConfig
{
    Token = GsiDefaults.AuthToken,
    Port = GsiDefaults.Port
};

Console.WriteLine("=== CS2 GSI Listener ===");
Console.WriteLine($"Porta: {config.Port}");
Console.WriteLine();

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

using var server = new GsiHttpServer(config.Token, config.Port);
server.StateReceived = LogState;
server.RequestFailed = ex => Console.WriteLine($"Errore richiesta GSI: {ex.Message}");
server.Start();

Console.WriteLine($"Listener avviato su {server.Prefix}");
Console.WriteLine("In attesa di dati da CS2...\n");
try
{
    await Task.Delay(Timeout.InfiniteTimeSpan, cts.Token);
}
catch (OperationCanceledException)
{
    // Shutdown requested via CTRL+C.
}

static void LogState(GameState s)
{
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] " +
        $"Map: {s.Map.Name} | Round: {s.Map.Round} | Phase: {s.Round.Phase} | " +
        $"HP: {s.Player.Hp} | Armor: {s.Player.Armor} | " +
        $"Weapon: {s.Player.ActiveWeapon} | Ammo: {s.Player.AmmoClip}/{s.Player.AmmoReserve} | " +
        $"Bomb: {s.Bomb.State} {s.Bomb.Timer}");
}
