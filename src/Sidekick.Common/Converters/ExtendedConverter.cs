using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sidekick.Common.Converters;

public class ObjectOrArrayConverter<TObject> : JsonConverter<TObject?>
    where TObject : class
{
    public override TObject? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Handle case where it's an empty array
        if (reader.TokenType == JsonTokenType.StartArray)
        {
            // Consume the array (assuming it's empty and you only need to handle the array case where there are no elements)
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray) {}

            return null;
        }

        // Handle case where it's an object
        if (reader.TokenType == JsonTokenType.StartObject)
        {
            return JsonSerializer.Deserialize<TObject>(ref reader, options);
        }

        // If neither, return null or handle accordingly
        return null;
    }

    public override void Write(Utf8JsonWriter writer, TObject? value, JsonSerializerOptions options)
    {
        if (value != null)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
        else
        {
            writer.WriteStartArray();
            writer.WriteEndArray();
        }
    }
}
