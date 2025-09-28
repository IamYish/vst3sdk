using VSTGUIAdvancedDesigner.Models;

namespace VSTGUIAdvancedDesigner.ViewModels;

public sealed class ControlPaletteItem
{
    public ControlPaletteItem(string name, ControlKind kind, string description)
    {
        Name = name;
        Kind = kind;
        Description = description;
    }

    public string Name { get; }

    public ControlKind Kind { get; }

    public string Description { get; }
}
