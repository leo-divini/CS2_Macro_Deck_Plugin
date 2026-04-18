using SuchByte.MacroDeck.Logging;
using SuchByte.MacroDeck.Plugins;

namespace Cs2MacroDeck.Plugin;

public sealed class Cs2MacroDeckPlugin : MacroDeckPlugin
{
    public Cs2MacroDeckPlugin()
    {
        Actions = new List<PluginAction>
        {
            new RefreshCs2StateAction(),
            new ResetCs2ListenerAction()
        };
    }

    public override void Enable()
    {
        MacroDeckLogger.Info(this, "CS2 Macro Deck plugin enabled.");
        Cs2StateVariablePublisher.Initialize(this);

        if (!Cs2GsiPluginServer.Start(this))
        {
            Cs2StateVariablePublisher.StartPolling(this);
        }
    }
}
