using Cs2Gsi.Core.Config;

namespace Cs2Gsi.Listener.Config;

public class ListenerConfig
{
    public string Token { get; set; } = GsiDefaults.AuthToken;
    public int Port { get; set; } = GsiDefaults.Port;
}
