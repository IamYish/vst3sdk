using System;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using VSTGUIAdvancedDesigner.Models;

namespace VSTGUIAdvancedDesigner.Services;

public sealed class SpriteSheetExporter
{
    public void Export(AnimationSequence animation, string outputPath, int columns = 4)
    {
        if (animation.Frames.Count == 0)
        {
            throw new InvalidOperationException("Animation has no frames to export.");
        }

        var bitmaps = animation.Frames
            .Select(frame => LoadBitmap(frame.AssetPath))
            .ToArray();

        var frameWidth = bitmaps.Max(b => b.PixelWidth);
        var frameHeight = bitmaps.Max(b => b.PixelHeight);

        var rows = (int)Math.Ceiling(bitmaps.Length / (double)columns);
        var sheetWidth = frameWidth * columns;
        var sheetHeight = frameHeight * rows;

        var sheet = new WriteableBitmap(sheetWidth, sheetHeight, 96, 96, System.Windows.Media.PixelFormats.Pbgra32, null);

        for (var index = 0; index < bitmaps.Length; index++)
        {
            var source = bitmaps[index];
            var column = index % columns;
            var row = index / columns;
            var rect = new System.Windows.Int32Rect(column * frameWidth, row * frameHeight, source.PixelWidth, source.PixelHeight);
            var stride = source.PixelWidth * (source.Format.BitsPerPixel / 8);
            var pixels = new byte[source.PixelHeight * stride];
            source.CopyPixels(pixels, stride, 0);
            sheet.WritePixels(rect, pixels, stride, 0);
        }

        using var stream = File.Open(outputPath, FileMode.Create, FileAccess.Write);
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(sheet));
        encoder.Save(stream);
    }

    private static BitmapSource LoadBitmap(string assetPath)
    {
        if (!File.Exists(assetPath))
        {
            throw new FileNotFoundException("Missing animation frame asset", assetPath);
        }

        var image = new BitmapImage();
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.UriSource = new Uri(Path.GetFullPath(assetPath));
        image.EndInit();
        image.Freeze();
        return image;
    }
}
