using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VSTGUIAdvancedDesigner.Models;

namespace VSTGUIAdvancedDesigner.ViewModels;

public partial class ControlPaletteViewModel : ObservableObject
{
    private readonly MainViewModel owner;

    public ControlPaletteViewModel(MainViewModel owner)
    {
        this.owner = owner;
        Controls = new ObservableCollection<ControlPaletteItem>(CreateDefaultControls());
    }

    [ObservableProperty]
    private ObservableCollection<ControlPaletteItem> controls;

    [ObservableProperty]
    private ControlPaletteItem? selectedItem;

    [RelayCommand]
    private void AddSelectedControl()
    {
        if (SelectedItem != null)
        {
            owner.AddControl(SelectedItem.Kind);
        }
    }

    public string CreateControlName(ControlKind kind, LayerModel layer)
    {
        var baseName = kind.ToString();
        var existing = layer.Controls.Count(c => c.Kind == kind);
        return $"{baseName} {existing + 1}";
    }

    private static IEnumerable<ControlPaletteItem> CreateDefaultControls()
    {
        yield return new ControlPaletteItem("View Container", ControlKind.ViewContainer, "Root view container");
        yield return new ControlPaletteItem("Button", ControlKind.Button, "Momentary button");
        yield return new ControlPaletteItem("Toggle Button", ControlKind.ToggleButton, "On/off toggle");
        yield return new ControlPaletteItem("Layered Button", ControlKind.LayeredButton, "PNG layered button");
        yield return new ControlPaletteItem("Knob", ControlKind.Knob, "Standard knob");
        yield return new ControlPaletteItem("Layered Knob", ControlKind.LayeredKnob, "Animated knob using frames");
        yield return new ControlPaletteItem("Slider", ControlKind.Slider, "Continuous slider");
        yield return new ControlPaletteItem("Layered Slider", ControlKind.LayeredSlider, "Slider with bitmap frames");
        yield return new ControlPaletteItem("Switch", ControlKind.Switch, "Multi-state switch");
        yield return new ControlPaletteItem("Step Switch", ControlKind.StepSwitch, "Stepped button");
        yield return new ControlPaletteItem("Check Box", ControlKind.CheckBox, "Check box control");
        yield return new ControlPaletteItem("Radio Button", ControlKind.RadioButton, "Radio button");
        yield return new ControlPaletteItem("Option Menu", ControlKind.OptionMenu, "Drop-down menu");
        yield return new ControlPaletteItem("Text Label", ControlKind.TextLabel, "Static text label");
        yield return new ControlPaletteItem("Text Edit", ControlKind.TextEdit, "Editable text");
        yield return new ControlPaletteItem("Parameter Display", ControlKind.ParameterDisplay, "Displays parameter value");
        yield return new ControlPaletteItem("Gradient View", ControlKind.GradientView, "Gradient background");
        yield return new ControlPaletteItem("Multiline Text", ControlKind.MultiLineText, "Paragraph text");
        yield return new ControlPaletteItem("XY Pad", ControlKind.XYPad, "Two-dimensional controller");
        yield return new ControlPaletteItem("Movie Bitmap", ControlKind.MovieBitmap, "Animated sprite");
        yield return new ControlPaletteItem("Custom View", ControlKind.CustomView, "Custom view placeholder");
    }
}
