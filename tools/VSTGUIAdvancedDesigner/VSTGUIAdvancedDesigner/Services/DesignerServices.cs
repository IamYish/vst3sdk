namespace VSTGUIAdvancedDesigner.Services;

public sealed class DesignerServices
{
    public DesignerServices(
        ProjectPersistence projectPersistence,
        UidescSerializer uidescSerializer,
        SpriteSheetExporter spriteSheetExporter,
        PngExportService pngExportService,
        FileDialogService fileDialogService,
        BitmapAssetService bitmapAssetService)
    {
        ProjectPersistence = projectPersistence;
        UidescSerializer = uidescSerializer;
        SpriteSheetExporter = spriteSheetExporter;
        PngExportService = pngExportService;
        FileDialogService = fileDialogService;
        BitmapAssetService = bitmapAssetService;
    }

    public ProjectPersistence ProjectPersistence { get; }

    public UidescSerializer UidescSerializer { get; }

    public SpriteSheetExporter SpriteSheetExporter { get; }

    public PngExportService PngExportService { get; }

    public FileDialogService FileDialogService { get; }

    public BitmapAssetService BitmapAssetService { get; }
}
