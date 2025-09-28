namespace VSTGUIAdvancedDesigner.Models;

public partial class AnimationFrame : ObservableObject
{
    public AnimationFrame()
    {
    }

    public AnimationFrame(string assetPath, double duration)
    {
        AssetPath = assetPath;
        Duration = duration;
    }

    [ObservableProperty]
    private string assetPath = string.Empty;

    [ObservableProperty]
    private double duration = 0.033;
}
