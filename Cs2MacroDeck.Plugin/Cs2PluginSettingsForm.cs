using System.Text;
using System.Windows.Forms;

namespace Cs2MacroDeck.Plugin;

internal sealed class Cs2PluginSettingsForm : Form
{
    private readonly TextBox tokenTextBox = new();
    private readonly NumericUpDown portInput = new();
    private readonly TreeView variablesTree = new();
    private bool suppressNodeEvents;

    public Cs2PluginSettingsForm(Cs2PluginSettings settings)
    {
        UpdatedSettings = settings.Clone();
        InitializeComponent();
        LoadSettingsIntoControls();
    }

    public Cs2PluginSettings UpdatedSettings { get; private set; }

    private void InitializeComponent()
    {
        Text = "CS2 GSI for Macro Deck Settings";
        Width = 900;
        Height = 700;
        StartPosition = FormStartPosition.CenterParent;
        MinimizeBox = false;
        MaximizeBox = true;

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(12)
        };
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var endpointGroup = new GroupBox
        {
            Text = "Listener",
            Dock = DockStyle.Top,
            AutoSize = true
        };
        var endpointLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 2,
            Padding = new Padding(8),
            AutoSize = true
        };
        endpointLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        endpointLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        endpointLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        endpointLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));

        tokenTextBox.Dock = DockStyle.Fill;
        portInput.Minimum = 1;
        portInput.Maximum = 65535;
        portInput.Width = 90;

        endpointLayout.Controls.Add(new Label { Text = "Token", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
        endpointLayout.Controls.Add(tokenTextBox, 1, 0);
        endpointLayout.Controls.Add(new Label { Text = "Port", AutoSize = true, Anchor = AnchorStyles.Left }, 2, 0);
        endpointLayout.Controls.Add(portInput, 3, 0);
        var listenerNoteLabel = new Label
        {
            Text = "Changing token or port restarts the local listener after save.",
            AutoSize = true,
            Dock = DockStyle.Fill
        };
        endpointLayout.Controls.Add(listenerNoteLabel, 1, 1);
        endpointLayout.SetColumnSpan(listenerNoteLabel, 3);
        endpointGroup.Controls.Add(endpointLayout);

        var toolbar = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Padding = new Padding(0, 8, 0, 8)
        };
        toolbar.Controls.Add(CreateButton("Base defaults", (_, _) => ApplyDefaultVariables()));
        toolbar.Controls.Add(CreateButton("Enable all", (_, _) => SetAllVariables(true)));
        toolbar.Controls.Add(CreateButton("Disable all optional", (_, _) => SetAllVariables(false)));
        toolbar.Controls.Add(CreateButton("Expand all", (_, _) => variablesTree.ExpandAll()));
        toolbar.Controls.Add(CreateButton("Copy CS2 config", (_, _) => CopyCs2Config()));

        variablesTree.CheckBoxes = true;
        variablesTree.Dock = DockStyle.Fill;
        variablesTree.AfterCheck += VariablesTreeAfterCheck;

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true
        };
        var saveButton = CreateButton("Save", (_, _) => SaveAndClose());
        var cancelButton = CreateButton("Cancel", (_, _) => DialogResult = DialogResult.Cancel);
        buttons.Controls.Add(saveButton);
        buttons.Controls.Add(cancelButton);

        root.Controls.Add(endpointGroup, 0, 0);
        root.Controls.Add(toolbar, 0, 1);
        root.Controls.Add(variablesTree, 0, 2);
        root.Controls.Add(buttons, 0, 3);
        Controls.Add(root);
        AcceptButton = saveButton;
        CancelButton = cancelButton;
    }

    private static Button CreateButton(string text, EventHandler click)
    {
        var button = new Button
        {
            Text = text,
            AutoSize = true,
            Margin = new Padding(4)
        };
        button.Click += click;
        return button;
    }

    private void LoadSettingsIntoControls()
    {
        tokenTextBox.Text = UpdatedSettings.AuthToken;
        portInput.Value = Math.Clamp(UpdatedSettings.Port, 1, 65535);
        BuildTree();
    }

    private void BuildTree()
    {
        suppressNodeEvents = true;
        variablesTree.BeginUpdate();
        variablesTree.Nodes.Clear();

        foreach (var categoryGroup in Cs2VariableCatalog.Definitions.GroupBy(definition => definition.Category))
        {
            var categoryNode = new TreeNode(categoryGroup.Key)
            {
                Tag = SettingsNodeTag.CreateCategory(categoryGroup.Key),
                Checked = categoryGroup.Any(UpdatedSettings.IsEnabled)
            };

            foreach (var group in categoryGroup.GroupBy(definition => definition.Group))
            {
                var groupNode = new TreeNode(group.Key)
                {
                    Tag = SettingsNodeTag.CreateGroup(categoryGroup.Key, group.Key),
                    Checked = group.Any(UpdatedSettings.IsEnabled)
                };

                foreach (var definition in group.OrderBy(definition => definition.Name))
                {
                    var variableNode = new TreeNode($"{definition.Name} ({definition.Type})")
                    {
                        Tag = SettingsNodeTag.CreateVariable(definition),
                        Checked = UpdatedSettings.IsEnabled(definition),
                        ForeColor = definition.Required ? System.Drawing.SystemColors.GrayText : System.Drawing.SystemColors.WindowText
                    };
                    groupNode.Nodes.Add(variableNode);
                }

                categoryNode.Nodes.Add(groupNode);
            }

            variablesTree.Nodes.Add(categoryNode);
        }

        variablesTree.EndUpdate();
        suppressNodeEvents = false;
        variablesTree.ExpandAll();
    }

    private void VariablesTreeAfterCheck(object? sender, TreeViewEventArgs e)
    {
        if (e.Node is null)
        {
            return;
        }

        if (suppressNodeEvents)
        {
            return;
        }

        if (e.Node.Tag is SettingsNodeTag { Kind: SettingsNodeKind.Variable, Required: true } && !e.Node.Checked)
        {
            suppressNodeEvents = true;
            e.Node.Checked = true;
            suppressNodeEvents = false;
            return;
        }

        if (e.Node.Nodes.Count == 0)
        {
            return;
        }

        suppressNodeEvents = true;
        SetChildrenChecked(e.Node, e.Node.Checked);
        suppressNodeEvents = false;
    }

    private static void SetChildrenChecked(TreeNode node, bool isChecked)
    {
        foreach (TreeNode child in node.Nodes)
        {
            if (child.Tag is SettingsNodeTag { Kind: SettingsNodeKind.Variable, Required: true })
            {
                child.Checked = true;
            }
            else
            {
                child.Checked = isChecked;
            }

            SetChildrenChecked(child, isChecked);
        }
    }

    private void ApplyDefaultVariables()
    {
        UpdatedSettings.Categories.Clear();
        UpdatedSettings.Groups.Clear();
        UpdatedSettings.Variables.Clear();
        BuildTree();
    }

    private void SetAllVariables(bool enabled)
    {
        UpdatedSettings.Categories.Clear();
        UpdatedSettings.Groups.Clear();
        UpdatedSettings.Variables.Clear();

        foreach (var definition in Cs2VariableCatalog.Definitions)
        {
            UpdatedSettings.Variables[definition.Name] = definition.Required || enabled;
        }

        BuildTree();
    }

    private void CopyCs2Config()
    {
        var settings = CollectSettings();
        Clipboard.SetText(BuildCs2Config(settings));
    }

    private void SaveAndClose()
    {
        UpdatedSettings = CollectSettings();
        DialogResult = DialogResult.OK;
    }

    private Cs2PluginSettings CollectSettings()
    {
        var settings = UpdatedSettings.Clone();
        settings.AuthToken = tokenTextBox.Text.Trim();
        settings.Port = (int)portInput.Value;
        settings.Categories.Clear();
        settings.Groups.Clear();
        settings.Variables.Clear();

        foreach (TreeNode categoryNode in variablesTree.Nodes)
        {
            if (categoryNode.Tag is not SettingsNodeTag categoryTag)
            {
                continue;
            }

            settings.Categories[categoryTag.Category] = categoryNode.Checked;
            foreach (TreeNode groupNode in categoryNode.Nodes)
            {
                if (groupNode.Tag is not SettingsNodeTag groupTag)
                {
                    continue;
                }

                settings.Groups[Cs2VariableCatalog.GroupKey(groupTag.Category, groupTag.Group)] = groupNode.Checked;
                foreach (TreeNode variableNode in groupNode.Nodes)
                {
                    if (variableNode.Tag is not SettingsNodeTag variableTag || variableTag.VariableName is null)
                    {
                        continue;
                    }

                    settings.Variables[variableTag.VariableName] = variableTag.Required || variableNode.Checked;
                }
            }
        }

        return settings;
    }

    private static string BuildCs2Config(Cs2PluginSettings settings)
    {
        var builder = new StringBuilder();
        builder.AppendLine("\"CS2 Macro Deck GSI\"");
        builder.AppendLine("{");
        builder.AppendLine($"    \"uri\"       \"http://127.0.0.1:{settings.Port}/\"");
        builder.AppendLine("    \"timeout\"   \"5.0\"");
        builder.AppendLine("    \"buffer\"    \"0.1\"");
        builder.AppendLine("    \"throttle\"  \"0.5\"");
        builder.AppendLine("    \"heartbeat\" \"10.0\"");
        builder.AppendLine("    \"auth\"");
        builder.AppendLine("    {");
        builder.AppendLine($"        \"token\" \"{settings.AuthToken}\"");
        builder.AppendLine("    }");
        builder.AppendLine("    \"output\"");
        builder.AppendLine("    {");
        builder.AppendLine("        \"precision_time\"     \"3\"");
        builder.AppendLine("        \"precision_position\" \"1\"");
        builder.AppendLine("        \"precision_vector\"   \"3\"");
        builder.AppendLine("    }");
        builder.AppendLine("    \"data\"");
        builder.AppendLine("    {");
        builder.AppendLine("        \"provider\"               \"1\"");
        builder.AppendLine("        \"map\"                    \"1\"");
        builder.AppendLine("        \"map_round_wins\"         \"1\"");
        builder.AppendLine("        \"round\"                  \"1\"");
        builder.AppendLine("        \"player_id\"              \"1\"");
        builder.AppendLine("        \"player_state\"           \"1\"");
        builder.AppendLine("        \"player_weapons\"         \"1\"");
        builder.AppendLine("        \"player_match_stats\"     \"1\"");
        builder.AppendLine("        \"player_position\"        \"1\"");
        builder.AppendLine("        \"bomb\"                   \"1\"");
        builder.AppendLine("        \"phase_countdowns\"       \"1\"");
        builder.AppendLine("        \"allplayers_id\"          \"1\"");
        builder.AppendLine("        \"allplayers_state\"       \"1\"");
        builder.AppendLine("        \"allplayers_match_stats\" \"1\"");
        builder.AppendLine("        \"allplayers_weapons\"     \"1\"");
        builder.AppendLine("        \"allplayers_position\"    \"1\"");
        builder.AppendLine("        \"allgrenades\"            \"1\"");
        builder.AppendLine("    }");
        builder.AppendLine("}");
        return builder.ToString();
    }

    private sealed record SettingsNodeTag(
        SettingsNodeKind Kind,
        string Category,
        string Group,
        string? VariableName,
        bool Required)
    {
        public static SettingsNodeTag CreateCategory(string category)
        {
            return new SettingsNodeTag(SettingsNodeKind.Category, category, "", null, false);
        }

        public static SettingsNodeTag CreateGroup(string category, string group)
        {
            return new SettingsNodeTag(SettingsNodeKind.Group, category, group, null, false);
        }

        public static SettingsNodeTag CreateVariable(Cs2VariableDefinition definition)
        {
            return new SettingsNodeTag(
                SettingsNodeKind.Variable,
                definition.Category,
                definition.Group,
                definition.Name,
                definition.Required);
        }
    }

    private enum SettingsNodeKind
    {
        Category,
        Group,
        Variable
    }
}
