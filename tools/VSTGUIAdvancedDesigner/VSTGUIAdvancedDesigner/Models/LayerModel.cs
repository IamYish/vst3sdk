using System.Collections.ObjectModel;

namespace VSTGUIAdvancedDesigner.Models;

public partial class LayerModel : ObservableObject
{
    public LayerModel()
    {
        Controls = new ObservableCollection<ControlModel>();
    }

    public LayerModel(string name) : this()
    {
        Name = name;
    }

    [ObservableProperty]
    private string name = "Layer";

    [ObservableProperty]
    private bool isVisible = true;

    [ObservableProperty]
    private bool isLocked;

    [ObservableProperty]
    private ObservableCollection<ControlModel> controls;

    public int Index { get; set; }
}
