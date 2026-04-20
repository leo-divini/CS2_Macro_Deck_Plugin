using System.Text.Json;
using Cs2Gsi.Core.Config;
using SuchByte.MacroDeck.Logging;
using SuchByte.MacroDeck.Plugins;

namespace Cs2MacroDeck.Plugin;

internal static class Cs2PluginSettingsStore
{
    private const string SettingsKey = "cs2md.settings";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false
    };

    private static readonly object SyncRoot = new();
    private static Cs2PluginSettings current = CreateDefault();

    public static Cs2PluginSettings Current
    {
        get
        {
            lock (SyncRoot)
            {
                return current;
            }
        }
    }

    public static Cs2PluginSettings Load(MacroDeckPlugin plugin)
    {
        lock (SyncRoot)
        {
            try
            {
                var json = PluginConfiguration.GetValue(plugin, SettingsKey);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    current = Normalize(JsonSerializer.Deserialize<Cs2PluginSettings>(json) ?? CreateDefault());
                    return current;
                }
            }
            catch (Exception ex)
            {
                MacroDeckLogger.Warning(plugin, $"Failed to load CS2 plugin settings, using defaults: {ex.Message}");
            }

            current = CreateDefault();
            return current;
        }
    }

    public static bool Save(MacroDeckPlugin plugin, Cs2PluginSettings settings)
    {
        lock (SyncRoot)
        {
            var previousToken = current.AuthToken;
            var previousPort = current.Port;
            current = Normalize(settings);
            PluginConfiguration.SetValue(plugin, SettingsKey, JsonSerializer.Serialize(current, JsonOptions));
            return !string.Equals(previousToken, current.AuthToken, StringComparison.Ordinal) ||
                previousPort != current.Port;
        }
    }

    public static void ResetToDefault(MacroDeckPlugin plugin)
    {
        lock (SyncRoot)
        {
            current = CreateDefault();
            PluginConfiguration.SetValue(plugin, SettingsKey, JsonSerializer.Serialize(current, JsonOptions));
        }
    }

    public static bool IsVariableEnabled(string variableName)
    {
        if (!Cs2VariableCatalog.ByName.TryGetValue(variableName, out var definition))
        {
            return false;
        }

        lock (SyncRoot)
        {
            return current.IsEnabled(definition);
        }
    }

    public static bool IsVariableEnabled(Cs2VariableDefinition definition)
    {
        lock (SyncRoot)
        {
            return current.IsEnabled(definition);
        }
    }

    public static Cs2PluginSettings CreateDefault()
    {
        return new Cs2PluginSettings();
    }

    private static Cs2PluginSettings Normalize(Cs2PluginSettings settings)
    {
        settings.AuthToken = string.IsNullOrWhiteSpace(settings.AuthToken)
            ? GsiDefaults.AuthToken
            : settings.AuthToken.Trim();

        if (settings.Port is < 1 or > 65535)
        {
            settings.Port = GsiDefaults.Port;
        }

        settings.Categories ??= new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        settings.Groups ??= new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        settings.Variables ??= new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

        settings.Categories = new Dictionary<string, bool>(settings.Categories, StringComparer.OrdinalIgnoreCase);
        settings.Groups = new Dictionary<string, bool>(settings.Groups, StringComparer.OrdinalIgnoreCase);
        settings.Variables = new Dictionary<string, bool>(settings.Variables, StringComparer.OrdinalIgnoreCase);

        return settings;
    }
}
