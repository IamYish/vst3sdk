using System;
using System.Collections.ObjectModel;
using System.Windows;
using VSTGUIAdvancedDesigner.Models;

namespace VSTGUIAdvancedDesigner.ViewModels;

public partial class DesignerCanvasViewModel : ObservableObject
{
    private readonly MainViewModel owner;

    public DesignerCanvasViewModel(MainViewModel owner)
    {
        this.owner = owner;
    }

    [ObservableProperty]
    private LayerModel? selectedLayer;

    [ObservableProperty]
    private ControlModel? selectedControl;

    [ObservableProperty]
    private FrameworkElement? hostElement;

    public GridSettings GridSettings => owner.GridSettings;

    public ObservableCollection<LayerModel> Layers => owner.CurrentProject.Layers;

    public Size CanvasSize => owner.CurrentProject.CanvasSize;

    public void Refresh()
    {
        OnPropertyChanged(nameof(Layers));
        OnPropertyChanged(nameof(GridSettings));
        OnPropertyChanged(nameof(CanvasSize));
    }

    partial void OnSelectedControlChanged(ControlModel? value)
    {
        owner.NotifySelectionChanged(value);
    }

    partial void OnSelectedLayerChanged(LayerModel? value)
    {
        owner.NotifyLayerSelection(value);
    }

    [RelayCommand]
    private void SelectControl(ControlModel control)
    {
        owner.SelectedControl = control;
    }

    [RelayCommand]
    private void SelectLayer(LayerModel layer)
    {
        owner.SelectedLayer = layer;
    }

    [RelayCommand]
    private void DeleteSelection()
    {
        if (SelectedLayer != null && SelectedControl != null)
        {
            SelectedLayer.Controls.Remove(SelectedControl);
            owner.SelectedControl = null;
            owner.UpdateStatus("Control removed");
        }
    }

    public Point Snap(Point position)
    {
        if (!GridSettings.IsSnapEnabled)
            return position;

        var cell = GridSettings.CellSize <= 0 ? 1 : GridSettings.CellSize;
        var snappedX = Math.Round(position.X / cell) * cell;
        var snappedY = Math.Round(position.Y / cell) * cell;
        return new Point(snappedX, snappedY);
    }
}
