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

    public override bool CanConfigure => true;

    public override void Enable()
    {
        MacroDeckLogger.Info(this, "CS2 Macro Deck plugin enabled.");
        Cs2PluginSettingsStore.Load(this);
        Cs2StateVariablePublisher.Initialize(this);

        if (!Cs2GsiPluginServer.Start(this))
        {
            Cs2StateVariablePublisher.StartPolling(this);
        }
    }

    public override void OpenConfigurator()
    {
        try
        {
            using var form = new Cs2PluginSettingsForm(Cs2PluginSettingsStore.Current);
            if (form.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            var listenerSettingsChanged = Cs2PluginSettingsStore.Save(this, form.UpdatedSettings);
            Cs2StateVariablePublisher.ApplySettings(this);

            if (listenerSettingsChanged)
            {
                Cs2GsiPluginServer.Reset();
            }

            MacroDeckLogger.Info(this, "CS2 Macro Deck settings saved.");
        }
        catch (Exception ex)
        {
            MacroDeckLogger.Error(this, $"Failed to open CS2 Macro Deck settings: {ex.Message}");
        }
    }
}
