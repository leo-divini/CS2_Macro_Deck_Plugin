using System.Net;
using Cs2Gsi.Core.Config;
using Cs2Gsi.Core.Http;
using SuchByte.MacroDeck.Logging;
using SuchByte.MacroDeck.Plugins;

namespace Cs2MacroDeck.Plugin;

internal static class Cs2GsiPluginServer
{
    private static readonly object SyncRoot = new();

    private static GsiHttpServer? server;
    private static MacroDeckPlugin? plugin;

    public static bool Start(MacroDeckPlugin macroDeckPlugin)
    {
        lock (SyncRoot)
        {
            plugin = macroDeckPlugin;

            if (server is not null)
            {
                return true;
            }

            var gsiServer = new GsiHttpServer(GsiDefaults.AuthToken, GsiDefaults.Port)
            {
                StateReceived = state =>
                {
                    var currentPlugin = plugin;
                    if (currentPlugin is not null)
                    {
                        Cs2StateVariablePublisher.PublishConnected(currentPlugin, state);
                    }
                },
                InvalidTokenReceived = () =>
                {
                    var currentPlugin = plugin;
                    if (currentPlugin is not null)
                    {
                        Cs2StateVariablePublisher.PublishTokenInvalid(currentPlugin);
                    }
                },
                RequestFailed = ex => MacroDeckLogger.Error(macroDeckPlugin, $"CS2 GSI request failed: {ex.Message}")
            };

            try
            {
                gsiServer.Start();
                server = gsiServer;
                Cs2StateVariablePublisher.PublishWaitingForCs2(macroDeckPlugin);
                MacroDeckLogger.Info(macroDeckPlugin, $"CS2 GSI server listening on {gsiServer.Prefix}");
                return true;
            }
            catch (HttpListenerException ex)
            {
                gsiServer.Dispose();
                Cs2StateVariablePublisher.PublishPortInUse(macroDeckPlugin);
                MacroDeckLogger.Warning(
                    macroDeckPlugin,
                    $"Could not start CS2 GSI server on {gsiServer.Prefix}: {ex.Message}. Falling back to /state polling.");
                return false;
            }
        }
    }

    public static void Reset()
    {
        lock (SyncRoot)
        {
            var currentPlugin = plugin;
            if (currentPlugin is null)
            {
                return;
            }

            server?.Dispose();
            server = null;
            Cs2StateVariablePublisher.StopPolling();
            Cs2StateVariablePublisher.PublishOffline(currentPlugin, "restarting");

            if (!Start(currentPlugin))
            {
                Cs2StateVariablePublisher.StartPolling(currentPlugin);
            }
        }
    }
}
