namespace VSTGUIAdvancedDesigner.Services;

public static class ServiceRegistry
{
    public static DesignerServices CreateDefault()
    {
        var projectPersistence = new ProjectPersistence();
        var uidescSerializer = new UidescSerializer();
        var spriteSheetExporter = new SpriteSheetExporter();
        var pngExportService = new PngExportService();
        var fileDialogService = new FileDialogService();
        var bitmapAssetService = new BitmapAssetService();
        return new DesignerServices(
            projectPersistence,
            uidescSerializer,
            spriteSheetExporter,
            pngExportService,
            fileDialogService,
            bitmapAssetService);
    }
}
