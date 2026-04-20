using Cs2Gsi.Core.Config;

namespace Cs2MacroDeck.Plugin;

internal sealed class Cs2PluginSettings
{
    public int SchemaVersion { get; set; } = 1;

    public string AuthToken { get; set; } = GsiDefaults.AuthToken;

    public int Port { get; set; } = GsiDefaults.Port;

    public Dictionary<string, bool> Categories { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public Dictionary<string, bool> Groups { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public Dictionary<string, bool> Variables { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public bool IsEnabled(Cs2VariableDefinition definition)
    {
        if (definition.Required)
        {
            return true;
        }

        if (Variables.TryGetValue(definition.Name, out var variableEnabled))
        {
            return variableEnabled;
        }

        if (Groups.TryGetValue(Cs2VariableCatalog.GroupKey(definition.Category, definition.Group), out var groupEnabled))
        {
            return groupEnabled;
        }

        if (Categories.TryGetValue(definition.Category, out var categoryEnabled))
        {
            return categoryEnabled;
        }

        return definition.EnabledByDefault;
    }

    public Cs2PluginSettings Clone()
    {
        return new Cs2PluginSettings
        {
            SchemaVersion = SchemaVersion,
            AuthToken = AuthToken,
            Port = Port,
            Categories = new Dictionary<string, bool>(Categories, StringComparer.OrdinalIgnoreCase),
            Groups = new Dictionary<string, bool>(Groups, StringComparer.OrdinalIgnoreCase),
            Variables = new Dictionary<string, bool>(Variables, StringComparer.OrdinalIgnoreCase)
        };
    }
}
