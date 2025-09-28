using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VSTGUIAdvancedDesigner.Services;

public sealed class PngExportService
{
    public void Export(FrameworkElement element, string outputPath)
    {
        if (element == null) throw new ArgumentNullException(nameof(element));

        var width = (int)Math.Ceiling(element.ActualWidth);
        var height = (int)Math.Ceiling(element.ActualHeight);

        if (width <= 0 || height <= 0)
        {
            throw new InvalidOperationException("Element has no size to render.");
        }

        var renderTarget = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
        renderTarget.Render(element);

        using var stream = File.Open(outputPath, FileMode.Create, FileAccess.Write);
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(renderTarget));
        encoder.Save(stream);
    }
}
