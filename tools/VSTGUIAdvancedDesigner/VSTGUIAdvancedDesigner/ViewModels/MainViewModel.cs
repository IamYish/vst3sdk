using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using VSTGUIAdvancedDesigner.Models;
using VSTGUIAdvancedDesigner.Services;

namespace VSTGUIAdvancedDesigner.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly DesignerServices services;
    private string? projectFilePath;

    public MainViewModel() : this(ServiceRegistry.CreateDefault())
    {
    }

    public MainViewModel(DesignerServices services)
    {
        this.services = services;
        LayerPanel = new LayerPanelViewModel(this);
        PropertiesPanel = new PropertiesPanelViewModel(this);
        ControlPalette = new ControlPaletteViewModel(this);
        DesignerCanvas = new DesignerCanvasViewModel(this);
        CurrentProject = new ProjectModel();
        StatusMessage = "Ready";
    }

    [ObservableProperty]
    private ProjectModel currentProject;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    [ObservableProperty]
    private string selectionSummary = "No selection";

    public GridSettings GridSettings => CurrentProject.Grid;

    public LayerPanelViewModel LayerPanel { get; }

    public PropertiesPanelViewModel PropertiesPanel { get; }

    public ControlPaletteViewModel ControlPalette { get; }

    public DesignerCanvasViewModel DesignerCanvas { get; }

    internal DesignerServices Services => services;

    public ControlModel? SelectedControl
    {
        get => PropertiesPanel.SelectedControl;
        set
        {
            if (PropertiesPanel.SelectedControl != value)
            {
                PropertiesPanel.SelectedControl = value;
                DesignerCanvas.SelectedControl = value;
                UpdateSelectionSummary();
            }
        }
    }

    public LayerModel? SelectedLayer
    {
        get => LayerPanel.SelectedLayer;
        set
        {
            if (LayerPanel.SelectedLayer != value)
            {
                LayerPanel.SelectedLayer = value;
                DesignerCanvas.SelectedLayer = value;
                UpdateSelectionSummary();
            }
        }
    }

    [RelayCommand]
    private void NewProject()
    {
        CurrentProject = new ProjectModel();
        projectFilePath = null;
        LayerPanel.Refresh(CurrentProject);
        DesignerCanvas.Refresh();
        PropertiesPanel.SelectedControl = null;
        StatusMessage = "New project created";
        UpdateSelectionSummary();
    }

    [RelayCommand]
    private void OpenProject()
    {
        var path = services.FileDialogService.ShowOpenDialog("VSTGUI Designer Project (*.vstguidesign)|*.vstguidesign");
        if (string.IsNullOrEmpty(path))
            return;

        CurrentProject = services.ProjectPersistence.Load(path);
        projectFilePath = path;
        LayerPanel.Refresh(CurrentProject);
        DesignerCanvas.Refresh();
        StatusMessage = $"Loaded project '{CurrentProject.Name}'";
        UpdateSelectionSummary();
    }

    [RelayCommand]
    private void SaveProject()
    {
        if (string.IsNullOrEmpty(projectFilePath))
        {
            projectFilePath = services.FileDialogService.ShowSaveDialog("VSTGUI Designer Project (*.vstguidesign)|*.vstguidesign", ".vstguidesign");
            if (string.IsNullOrEmpty(projectFilePath))
                return;
        }

        services.ProjectPersistence.Save(CurrentProject, projectFilePath);
        StatusMessage = $"Project saved to '{projectFilePath}'";
    }

    [RelayCommand]
    private void ImportUidesc()
    {
        var path = services.FileDialogService.ShowOpenDialog("VSTGUI UI Description (*.uidesc)|*.uidesc");
        if (string.IsNullOrEmpty(path))
            return;

        CurrentProject = services.UidescSerializer.Import(path);
        projectFilePath = null;
        LayerPanel.Refresh(CurrentProject);
        DesignerCanvas.Refresh();
        StatusMessage = $"Imported UIDESC '{Path.GetFileName(path)}'";
    }

    [RelayCommand]
    private void ExportUidesc()
    {
        var path = services.FileDialogService.ShowSaveDialog("VSTGUI UI Description (*.uidesc)|*.uidesc", ".uidesc");
        if (string.IsNullOrEmpty(path))
            return;

        services.UidescSerializer.Export(CurrentProject, path);
        StatusMessage = $"Exported UIDESC to '{Path.GetFileName(path)}'";
    }

    [RelayCommand]
    private void ExportSpriteSheet()
    {
        if (SelectedControl?.Animation == null || SelectedControl.Animation.Frames.Count == 0)
        {
            StatusMessage = "Select an animated control with frames";
            return;
        }

        var path = services.FileDialogService.ShowSaveDialog("PNG Image (*.png)|*.png", ".png");
        if (string.IsNullOrEmpty(path))
            return;

        services.SpriteSheetExporter.Export(SelectedControl.Animation, path);
        StatusMessage = "Sprite sheet exported";
    }

    [RelayCommand]
    private void ExportPng()
    {
        if (DesignerCanvas.HostElement == null)
        {
            StatusMessage = "Canvas not ready";
            return;
        }

        var path = services.FileDialogService.ShowSaveDialog("PNG Image (*.png)|*.png", ".png");
        if (string.IsNullOrEmpty(path))
            return;

        services.PngExportService.Export(DesignerCanvas.HostElement, path);
        StatusMessage = "Canvas exported as PNG";
    }

    [RelayCommand]
    private void Exit()
    {
        Application.Current.Shutdown();
    }

    [RelayCommand(CanExecute = nameof(CanUndo))]
    private void Undo()
    {
        // TODO: integrate undo stack
    }

    private bool CanUndo() => false;

    [RelayCommand(CanExecute = nameof(CanRedo))]
    private void Redo()
    {
        // TODO: integrate redo stack
    }

    private bool CanRedo() => false;

    internal void UpdateStatus(string message)
    {
        StatusMessage = message;
    }

    internal void NotifySelectionChanged(ControlModel? control)
    {
        SelectedControl = control;
    }

    internal void NotifyLayerSelection(LayerModel? layer)
    {
        SelectedLayer = layer;
    }

    internal void AddControl(ControlKind kind)
    {
        var layer = SelectedLayer ?? CurrentProject.Layers.FirstOrDefault() ?? AddDefaultLayer();
        var control = new ControlModel(kind, ControlPalette.CreateControlName(kind, layer))
        {
            Bounds = new Rect(12, 12, 120, 40)
        };
        layer.Controls.Add(control);
        control.ParentLayer = layer;
        SelectedLayer = layer;
        SelectedControl = control;
        DesignerCanvas.Refresh();
        StatusMessage = $"Added {control.Name}";
    }

    private LayerModel AddDefaultLayer()
    {
        var layer = new LayerModel("Layer 1") { Index = CurrentProject.Layers.Count };
        CurrentProject.Layers.Add(layer);
        LayerPanel.Refresh(CurrentProject);
        return layer;
    }

    private void UpdateSelectionSummary()
    {
        if (SelectedControl != null)
        {
            SelectionSummary = $"Selected: {SelectedControl.Name} ({SelectedControl.Kind})";
        }
        else if (SelectedLayer != null)
        {
            SelectionSummary = $"Layer: {SelectedLayer.Name}";
        }
        else
        {
            SelectionSummary = "No selection";
        }
    }
}
