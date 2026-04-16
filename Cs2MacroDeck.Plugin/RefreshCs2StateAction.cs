using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.Plugins;

namespace Cs2MacroDeck.Plugin;

public sealed class RefreshCs2StateAction : PluginAction
{
    public override string Name => "Refresh CS2 state";

    public override string Description => "Reads the latest CS2 listener state and updates Macro Deck variables.";

    public override void Trigger(string clientId, ActionButton actionButton)
    {
        _ = Cs2StateVariablePublisher.RefreshOnceAsync();
    }
}
