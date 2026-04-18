using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.Plugins;

namespace Cs2MacroDeck.Plugin;

public sealed class ResetCs2ListenerAction : PluginAction
{
    public override string Name => "Reset CS2 listener";

    public override string Description => "Restarts the local CS2 GSI listener and updates connection status variables.";

    public override void Trigger(string clientId, ActionButton actionButton)
    {
        Cs2GsiPluginServer.Reset();
    }
}
