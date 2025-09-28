using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace VSTGUIAdvancedDesigner.Models;

public partial class ProjectModel : ObservableObject
{
    public ProjectModel()
    {
        Layers = new ObservableCollection<LayerModel>();
        Grid = new GridSettings();
        CanvasSize = new Size(1024, 768);
        DpiScale = 1.0;
        Layers.Add(new LayerModel("Layer 1") { Index = 0 });
    }

    [ObservableProperty]
    private string name = "Untitled";

    [ObservableProperty]
    private Size canvasSize;

    [ObservableProperty]
    private double dpiScale;

    [ObservableProperty]
    private GridSettings grid;

    [ObservableProperty]
    private ObservableCollection<LayerModel> layers;

    public LayerModel EnsureLayer(string name)
    {
        var layer = Layers.FirstOrDefault(l => l.Name == name);
        if (layer == null)
        {
            layer = new LayerModel(name)
            {
                Index = Layers.Count
            };
            Layers.Add(layer);
        }
        return layer;
    }
}
