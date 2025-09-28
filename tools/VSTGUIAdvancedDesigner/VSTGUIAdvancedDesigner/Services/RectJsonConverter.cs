using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;

namespace VSTGUIAdvancedDesigner.Services;

public sealed class RectJsonConverter : JsonConverter<Rect>
{
    public override Rect Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException("Expected array for Rect");
        }

        reader.Read();
        var x = reader.GetDouble();
        reader.Read();
        var y = reader.GetDouble();
        reader.Read();
        var width = reader.GetDouble();
        reader.Read();
        var height = reader.GetDouble();
        reader.Read(); // EndArray
        return new Rect(x, y, width, height);
    }

    public override void Write(Utf8JsonWriter writer, Rect value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        writer.WriteNumberValue(value.X);
        writer.WriteNumberValue(value.Y);
        writer.WriteNumberValue(value.Width);
        writer.WriteNumberValue(value.Height);
        writer.WriteEndArray();
    }
}
