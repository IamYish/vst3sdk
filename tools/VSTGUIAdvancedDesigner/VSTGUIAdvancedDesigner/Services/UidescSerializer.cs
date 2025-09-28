using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using VSTGUIAdvancedDesigner.Models;

namespace VSTGUIAdvancedDesigner.Services;

public sealed class UidescSerializer
{
    private const string Version = "4.14.3";

    public ProjectModel Import(string path)
    {
        var document = XDocument.Load(path);
        var project = new ProjectModel
        {
            Name = Path.GetFileNameWithoutExtension(path)
        };
        project.Layers.Clear();

        var template = document.Descendants("template").FirstOrDefault();
        if (template == null)
        {
            return project;
        }

        if (template.Attribute("size") is XAttribute sizeAttr)
        {
            project.CanvasSize = ParseSize(sizeAttr.Value);
        }

        var layerViews = template.Element("subviews")?.Elements("view") ?? Enumerable.Empty<XElement>();
        foreach (var layerView in layerViews)
        {
            var layerName = (string?)layerView.Attribute("name") ?? "Layer";
            var layer = new LayerModel(layerName)
            {
                Index = project.Layers.Count
            };
            layer.IsVisible = ParseBool(layerView.Attribute("visible"), true);
            layer.IsLocked = ParseBool(layerView.Attribute("locked"), false);
            project.Layers.Add(layer);

            var controlViews = layerView.Element("subviews")?.Elements("view") ?? Enumerable.Empty<XElement>();
            foreach (var controlView in controlViews)
            {
                var control = ParseControl(controlView);
                control.ParentLayer = layer;
                layer.Controls.Add(control);
            }
        }

        return project;
    }

    public void Export(ProjectModel project, string path)
    {
        var template = new XElement("template",
            new XAttribute("name", project.Name),
            new XAttribute("class", "CViewContainer"),
            new XAttribute("size", FormatRect(new Rect(0, 0, project.CanvasSize.Width, project.CanvasSize.Height))));

        var layersElement = new XElement("subviews");
        foreach (var layer in project.Layers.Select((l, index) => (l, index)))
        {
            var layerElement = new XElement("view",
                new XAttribute("name", layer.l.Name),
                new XAttribute("class", "CViewContainer"),
                new XAttribute("layer-index", layer.index),
                new XAttribute("visible", layer.l.IsVisible),
                new XAttribute("locked", layer.l.IsLocked),
                new XAttribute("size", FormatRect(new Rect(0, 0, project.CanvasSize.Width, project.CanvasSize.Height))));

            var controlsElement = new XElement("subviews");
            foreach (var control in layer.l.Controls)
            {
                controlsElement.Add(CreateControlElement(control));
            }

            layerElement.Add(controlsElement);
            layersElement.Add(layerElement);
        }

        template.Add(layersElement);

        var doc = new XDocument(
            new XElement("uidescription",
                new XAttribute("version", Version),
                new XElement("bitmaps"),
                new XElement("fonts"),
                new XElement("colors"),
                new XElement("gradients"),
                new XElement("custom-attributes"),
                new XElement("control-tags"),
                new XElement("templates", template)));

        doc.Save(path);
    }

    private static ControlModel ParseControl(XElement element)
    {
        var className = (string?)element.Attribute("class") ?? "Custom";
        var control = new ControlModel(MapClassToKind(className), (string?)element.Attribute("name") ?? "Control")
        {
            Bounds = ParseRect((string?)element.Attribute("size") ?? "0,0,64,64"),
            IsVisible = ParseBool(element.Attribute("visible"), true),
            IsLocked = ParseBool(element.Attribute("locked"), false),
            Opacity = ParseDouble(element.Attribute("opacity"), 1.0),
            Rotation = ParseDouble(element.Attribute("rotation"), 0.0),
            ParameterId = (string?)element.Attribute("parameter-id"),
            BitmapAsset = (string?)element.Attribute("bitmap")
        };

        if (element.Element("animation") is XElement animationElement)
        {
            control.Animation = ParseAnimation(animationElement);
        }

        var tagsAttr = element.Attribute("tags");
        if (tagsAttr != null)
        {
            foreach (var tag in tagsAttr.Value.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                control.Tags.Add(tag);
            }
        }

        return control;
    }

    private static XElement CreateControlElement(ControlModel control)
    {
        var element = new XElement("view",
            new XAttribute("name", control.Name),
            new XAttribute("class", MapKindToClass(control.Kind)),
            new XAttribute("size", FormatRect(control.Bounds)),
            new XAttribute("visible", control.IsVisible),
            new XAttribute("locked", control.IsLocked),
            new XAttribute("opacity", control.Opacity.ToString(CultureInfo.InvariantCulture)),
            new XAttribute("rotation", control.Rotation.ToString(CultureInfo.InvariantCulture)));

        if (!string.IsNullOrWhiteSpace(control.ParameterId))
        {
            element.Add(new XAttribute("parameter-id", control.ParameterId));
        }

        if (!string.IsNullOrWhiteSpace(control.BitmapAsset))
        {
            element.Add(new XAttribute("bitmap", control.BitmapAsset));
        }

        if (control.Tags.Any())
        {
            element.Add(new XAttribute("tags", string.Join(';', control.Tags)));
        }

        if (control.Animation != null && control.Animation.Frames.Any())
        {
            element.Add(CreateAnimationElement(control.Animation));
        }

        return element;
    }

    private static XElement CreateAnimationElement(AnimationSequence animation)
    {
        var frames = string.Join(';', animation.Frames.Select(f => f.AssetPath));
        var durations = string.Join(';', animation.Frames.Select(f => f.Duration.ToString(CultureInfo.InvariantCulture)));
        return new XElement("animation",
            new XAttribute("name", animation.Name),
            new XAttribute("loop", animation.IsLooping),
            new XAttribute("frames", frames),
            new XAttribute("durations", durations));
    }

    private static AnimationSequence ParseAnimation(XElement element)
    {
        var animation = new AnimationSequence((string?)element.Attribute("name") ?? "Animation")
        {
            IsLooping = ParseBool(element.Attribute("loop"), true)
        };

        var framePaths = ((string?)element.Attribute("frames") ?? string.Empty)
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var frameDurations = ((string?)element.Attribute("durations") ?? string.Empty)
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(v => ParseDouble(v, 0.033))
            .ToArray();

        for (var i = 0; i < framePaths.Length; i++)
        {
            var duration = i < frameDurations.Length ? frameDurations[i] : frameDurations.LastOrDefault();
            animation.Frames.Add(new AnimationFrame(framePaths[i], duration <= 0 ? 0.033 : duration));
        }

        return animation;
    }

    private static string FormatRect(Rect rect)
    {
        var right = rect.Left + rect.Width;
        var bottom = rect.Top + rect.Height;
        return string.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3}",
            rect.Left, rect.Top, right, bottom);
    }

    private static Rect ParseRect(string value)
    {
        var parts = value.Split(',');
        if (parts.Length != 4)
        {
            return new Rect(0, 0, 64, 64);
        }

        var left = double.Parse(parts[0], CultureInfo.InvariantCulture);
        var top = double.Parse(parts[1], CultureInfo.InvariantCulture);
        var right = double.Parse(parts[2], CultureInfo.InvariantCulture);
        var bottom = double.Parse(parts[3], CultureInfo.InvariantCulture);
        return new Rect(left, top, right - left, bottom - top);
    }

    private static Size ParseSize(string value)
    {
        var rect = ParseRect(value);
        return new Size(rect.Width, rect.Height);
    }

    private static bool ParseBool(XAttribute? attribute, bool defaultValue)
    {
        if (attribute == null)
            return defaultValue;
        return bool.TryParse(attribute.Value, out var result) ? result : defaultValue;
    }

    private static double ParseDouble(XAttribute? attribute, double defaultValue)
    {
        if (attribute == null)
            return defaultValue;
        return double.TryParse(attribute.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result)
            ? result
            : defaultValue;
    }

    private static double ParseDouble(string value, double defaultValue)
    {
        return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result)
            ? result
            : defaultValue;
    }

    private static ControlKind MapClassToKind(string className) => className switch
    {
        "CViewContainer" => ControlKind.ViewContainer,
        "COnOffButton" => ControlKind.ToggleButton,
        "CToggleButton" => ControlKind.ToggleButton,
        "CTextButton" => ControlKind.Button,
        "CKnob" => ControlKind.Knob,
        "CAnimKnob" => ControlKind.LayeredKnob,
        "CSlider" => ControlKind.Slider,
        "CHorizontalSlider" => ControlKind.Slider,
        "CVerticalSlider" => ControlKind.Slider,
        "CAnimSlider" => ControlKind.LayeredSlider,
        "COptionMenu" => ControlKind.OptionMenu,
        "CTextLabel" => ControlKind.TextLabel,
        "CTextEdit" => ControlKind.TextEdit,
        "CParamDisplay" => ControlKind.ParameterDisplay,
        "CGradientView" => ControlKind.GradientView,
        "CMultiLineTextLabel" => ControlKind.MultiLineText,
        "CXYPad" => ControlKind.XYPad,
        "CMovieBitmap" => ControlKind.MovieBitmap,
        "CCheckBox" => ControlKind.CheckBox,
        "CRadioButton" => ControlKind.RadioButton,
        "CSwitch" => ControlKind.Switch,
        "CStepButton" => ControlKind.StepSwitch,
        _ => ControlKind.CustomView
    };

    private static string MapKindToClass(ControlKind kind) => kind switch
    {
        ControlKind.ViewContainer => "CViewContainer",
        ControlKind.LayeredView => "CViewContainer",
        ControlKind.Button => "CTextButton",
        ControlKind.ToggleButton => "CToggleButton",
        ControlKind.LayeredButton => "COnOffButton",
        ControlKind.Knob => "CKnob",
        ControlKind.LayeredKnob => "CAnimKnob",
        ControlKind.Slider => "CSlider",
        ControlKind.LayeredSlider => "CAnimSlider",
        ControlKind.Switch => "CSwitch",
        ControlKind.StepSwitch => "CStepButton",
        ControlKind.CheckBox => "CCheckBox",
        ControlKind.RadioButton => "CRadioButton",
        ControlKind.OptionMenu => "COptionMenu",
        ControlKind.TextLabel => "CTextLabel",
        ControlKind.TextEdit => "CTextEdit",
        ControlKind.ParameterDisplay => "CParamDisplay",
        ControlKind.GradientView => "CGradientView",
        ControlKind.MultiLineText => "CMultiLineTextLabel",
        ControlKind.XYPad => "CXYPad",
        ControlKind.MovieBitmap => "CMovieBitmap",
        ControlKind.AnimControl => "CAnimKnob",
        ControlKind.CustomView => "CView",
        _ => "CView"
    };
}
