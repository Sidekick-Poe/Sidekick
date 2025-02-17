using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sidekick.Common.Converters;

public class StringListToListOfStringsJsonConverter : JsonConverter<List<string>>
{
    public override List<string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString()?.Split(',').ToList() ?? new();
    }

    public override void Write(Utf8JsonWriter writer, List<string> value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(string.Join(",", value));
    }
}
