using System;
using System.Collections.ObjectModel;
using System.IO;
using VSTGUIAdvancedDesigner.Models;

namespace VSTGUIAdvancedDesigner.ViewModels;

public partial class PropertiesPanelViewModel : ObservableObject
{
    private readonly MainViewModel owner;

    public PropertiesPanelViewModel(MainViewModel owner)
    {
        this.owner = owner;
    }

    [ObservableProperty]
    private ControlModel? selectedControl;

    [ObservableProperty]
    private bool showAnimationSection;

    partial void OnSelectedControlChanged(ControlModel? value)
    {
        ShowAnimationSection = value?.Animation != null;
        OnPropertyChanged(nameof(AnimationFrames));
    }

    public ObservableCollection<AnimationFrame>? AnimationFrames => SelectedControl?.Animation?.Frames;

    [RelayCommand]
    private void ToggleVisibility()
    {
        if (SelectedControl != null)
        {
            SelectedControl.IsVisible = !SelectedControl.IsVisible;
        }
    }

    [RelayCommand]
    private void ToggleLock()
    {
        if (SelectedControl != null)
        {
            SelectedControl.IsLocked = !SelectedControl.IsLocked;
        }
    }

    [RelayCommand]
    private void EnsureAnimation()
    {
        if (SelectedControl == null)
            return;

        if (SelectedControl.Animation == null)
        {
            SelectedControl.Animation = new AnimationSequence($"{SelectedControl.Name} Animation");
            ShowAnimationSection = true;
            OnPropertyChanged(nameof(AnimationFrames));
        }
    }

    [RelayCommand(CanExecute = nameof(CanAddFrame))]
    private void AddFrame()
    {
        if (SelectedControl?.Animation == null)
            return;

        var path = owner.Services.FileDialogService.ShowOpenDialog("PNG Image (*.png)|*.png");
        if (string.IsNullOrEmpty(path))
            return;

        var assetPath = owner.Services.BitmapAssetService.ImportAsset(path, GetProjectAssetDirectory());
        SelectedControl.Animation.Frames.Add(new AnimationFrame(assetPath, 0.033));
    }

    private bool CanAddFrame() => SelectedControl?.Animation != null;

    [RelayCommand]
    private void RemoveFrame(AnimationFrame frame)
    {
        if (frame != null)
        {
            SelectedControl?.Animation?.Frames.Remove(frame);
        }
    }

    [RelayCommand]
    private void RemoveAnimation()
    {
        if (SelectedControl == null)
            return;

        SelectedControl.Animation = null;
        ShowAnimationSection = false;
        OnPropertyChanged(nameof(AnimationFrames));
    }

    [RelayCommand]
    private void AssignBitmap()
    {
        if (SelectedControl == null)
            return;

        var path = owner.Services.FileDialogService.ShowOpenDialog("PNG Image (*.png)|*.png");
        if (string.IsNullOrEmpty(path))
            return;

        var assetPath = owner.Services.BitmapAssetService.ImportAsset(path, GetProjectAssetDirectory());
        SelectedControl.BitmapAsset = assetPath;
    }

    private string GetProjectAssetDirectory()
    {
        var projectName = owner.CurrentProject.Name;
        var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "VSTGUIAdvancedDesigner", projectName, "Assets");
        Directory.CreateDirectory(folder);
        return folder;
    }
}
