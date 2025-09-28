using CommunityToolkit.Mvvm.ComponentModel;

namespace VSTGUIAdvancedDesigner.Models;

public partial class GridSettings : ObservableObject
{
    [ObservableProperty]
    private bool isVisible = true;

    [ObservableProperty]
    private bool isSnapEnabled = true;

    [ObservableProperty]
    private double cellSize = 12;

    [ObservableProperty]
    private double snapStrength = 1;
}
