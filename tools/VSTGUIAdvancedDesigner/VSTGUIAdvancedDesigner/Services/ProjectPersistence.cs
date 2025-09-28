using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using VSTGUIAdvancedDesigner.Models;

namespace VSTGUIAdvancedDesigner.Services;

public sealed class ProjectPersistence
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Converters =
        {
            new JsonStringEnumConverter(),
            new RectJsonConverter()
        }
    };

    public void Save(ProjectModel project, string path)
    {
        var json = JsonSerializer.Serialize(project, Options);
        File.WriteAllText(path, json);
    }

    public ProjectModel Load(string path)
    {
        var json = File.ReadAllText(path);
        var project = JsonSerializer.Deserialize<ProjectModel>(json, Options);
        if (project == null)
        {
            throw new InvalidDataException("Unable to deserialize project file.");
        }

        for (var i = 0; i < project.Layers.Count; i++)
        {
            var layer = project.Layers[i];
            layer.Index = i;
            foreach (var control in layer.Controls)
            {
                control.ParentLayer = layer;
                control.Tags ??= new System.Collections.ObjectModel.ObservableCollection<string>();
                if (control.Animation != null && control.Animation.Frames == null)
                {
                    control.Animation.Frames = new System.Collections.ObjectModel.ObservableCollection<AnimationFrame>();
                }
            }
        }

        return project;
    }
}
