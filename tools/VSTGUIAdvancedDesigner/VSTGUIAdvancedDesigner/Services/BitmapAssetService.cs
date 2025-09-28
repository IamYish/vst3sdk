using System.IO;

namespace VSTGUIAdvancedDesigner.Services;

public sealed class BitmapAssetService
{
    public string ImportAsset(string sourcePath, string projectDirectory)
    {
        if (!File.Exists(sourcePath))
        {
            throw new FileNotFoundException("Bitmap asset not found", sourcePath);
        }

        Directory.CreateDirectory(projectDirectory);
        var fileName = Path.GetFileName(sourcePath);
        var destination = Path.Combine(projectDirectory, fileName);
        File.Copy(sourcePath, destination, overwrite: true);
        return destination;
    }
}
