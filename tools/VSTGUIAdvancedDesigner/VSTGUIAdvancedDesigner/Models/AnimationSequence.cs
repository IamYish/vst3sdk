using System.Collections.ObjectModel;
using System.Linq;

namespace VSTGUIAdvancedDesigner.Models;

public partial class AnimationSequence : ObservableObject
{
    public AnimationSequence()
    {
        Frames = new ObservableCollection<AnimationFrame>();
    }

    public AnimationSequence(string name) : this()
    {
        Name = name;
    }

    [ObservableProperty]
    private string name = "Animation";

    [ObservableProperty]
    private bool isLooping = true;

    [ObservableProperty]
    private ObservableCollection<AnimationFrame> frames;

    public double TotalDuration => Frames.Sum(f => f.Duration);
}
