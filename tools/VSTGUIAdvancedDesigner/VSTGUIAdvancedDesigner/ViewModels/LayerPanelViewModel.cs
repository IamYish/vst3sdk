using System;
using System.Collections.ObjectModel;
using System.Linq;
using VSTGUIAdvancedDesigner.Models;

namespace VSTGUIAdvancedDesigner.ViewModels;

public partial class LayerPanelViewModel : ObservableObject
{
    private readonly MainViewModel owner;

    public LayerPanelViewModel(MainViewModel owner)
    {
        this.owner = owner;
        Layers = owner.CurrentProject.Layers;
    }

    [ObservableProperty]
    private ObservableCollection<LayerModel> layers;

    [ObservableProperty]
    private LayerModel? selectedLayer;

    partial void OnSelectedLayerChanged(LayerModel? value)
    {
        owner.NotifyLayerSelection(value);
    }

    public void Refresh(ProjectModel project)
    {
        Layers = project.Layers;
        SelectedLayer = Layers.FirstOrDefault();
    }

    [RelayCommand]
    private void AddLayer()
    {
        var layer = new LayerModel($"Layer {Layers.Count + 1}")
        {
            Index = Layers.Count
        };
        Layers.Add(layer);
        owner.SelectedLayer = layer;
    }

    [RelayCommand(CanExecute = nameof(CanRemoveLayer))]
    private void RemoveLayer()
    {
        if (SelectedLayer != null)
        {
            var index = Layers.IndexOf(SelectedLayer);
            Layers.Remove(SelectedLayer);
            owner.SelectedLayer = Layers.ElementAtOrDefault(Math.Max(0, index - 1));
        }
    }

    private bool CanRemoveLayer() => Layers.Count > 1;

    [RelayCommand(CanExecute = nameof(CanMoveUp))]
    private void MoveUp()
    {
        if (SelectedLayer == null)
            return;

        var index = Layers.IndexOf(SelectedLayer);
        if (index <= 0) return;
        Layers.Move(index, index - 1);
        ReindexLayers();
    }

    private bool CanMoveUp() => SelectedLayer != null && Layers.IndexOf(SelectedLayer) > 0;

    [RelayCommand(CanExecute = nameof(CanMoveDown))]
    private void MoveDown()
    {
        if (SelectedLayer == null)
            return;

        var index = Layers.IndexOf(SelectedLayer);
        if (index < 0 || index >= Layers.Count - 1) return;
        Layers.Move(index, index + 1);
        ReindexLayers();
    }

    private bool CanMoveDown() => SelectedLayer != null && Layers.IndexOf(SelectedLayer) < Layers.Count - 1;

    private void ReindexLayers()
    {
        for (var i = 0; i < Layers.Count; i++)
        {
            Layers[i].Index = i;
        }
    }
}
