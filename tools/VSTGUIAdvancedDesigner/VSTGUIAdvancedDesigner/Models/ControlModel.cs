using System;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using System.Windows;

namespace VSTGUIAdvancedDesigner.Models;

public partial class ControlModel : ObservableObject
{
    public ControlModel()
    {
        Id = Guid.NewGuid();
        Tags = new ObservableCollection<string>();
    }

    public ControlModel(ControlKind kind, string name) : this()
    {
        Kind = kind;
        Name = name;
    }

    public Guid Id { get; }

    [ObservableProperty]
    private string name = "Control";

    [ObservableProperty]
    private ControlKind kind = ControlKind.CustomView;

    [ObservableProperty]
    private Rect bounds = new Rect(0, 0, 64, 64);

    public double X
    {
        get => Bounds.X;
        set
        {
            var rect = Bounds;
            rect.X = value;
            Bounds = rect;
        }
    }

    public double Y
    {
        get => Bounds.Y;
        set
        {
            var rect = Bounds;
            rect.Y = value;
            Bounds = rect;
        }
    }

    public double Width
    {
        get => Bounds.Width;
        set
        {
            var rect = Bounds;
            rect.Width = Math.Max(0, value);
            Bounds = rect;
        }
    }

    public double Height
    {
        get => Bounds.Height;
        set
        {
            var rect = Bounds;
            rect.Height = Math.Max(0, value);
            Bounds = rect;
        }
    }

    [ObservableProperty]
    private double rotation;

    [ObservableProperty]
    private double opacity = 1.0;

    [ObservableProperty]
    private bool isVisible = true;

    [ObservableProperty]
    private bool isLocked;

    [ObservableProperty]
    private string? parameterId;

    [ObservableProperty]
    private string? bitmapAsset;

    [ObservableProperty]
    private AnimationSequence? animation;

    [ObservableProperty]
    private ObservableCollection<string> tags;

    [JsonIgnore]
    public LayerModel? ParentLayer { get; set; }

    partial void OnBoundsChanged(Rect value)
    {
        OnPropertyChanged(nameof(X));
        OnPropertyChanged(nameof(Y));
        OnPropertyChanged(nameof(Width));
        OnPropertyChanged(nameof(Height));
    }
}
