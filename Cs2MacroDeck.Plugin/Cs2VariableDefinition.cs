using SuchByte.MacroDeck.Variables;

namespace Cs2MacroDeck.Plugin;

internal sealed record Cs2VariableDefinition(
    string Name,
    VariableType Type,
    string Category,
    string Group,
    bool EnabledByDefault,
    bool Required = false);
