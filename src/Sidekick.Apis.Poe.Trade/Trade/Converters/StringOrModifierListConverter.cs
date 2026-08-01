using System.Text.Json;
using System.Text.Json.Serialization;
using Sidekick.Apis.Poe.Trade.Trade.Models;

namespace Sidekick.Apis.Poe.Trade.Trade.Converters;

public class StringOrModifierListConverter : JsonConverter<List<ApiItemModifier>>
{
    public override List<ApiItemModifier>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException("Expected start of array.");
        }

        var list = new List<ApiItemModifier>();

        // 2. Load the entire token into a JsonDocument safely
        using var doc = JsonDocument.ParseValue(ref reader);

        // 3. Loop over elements, handling both plain strings and complex objects
        foreach (var element in doc.RootElement.EnumerateArray())
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.String:
                    list.Add(new ApiItemModifier
                    {
                        Description = element.GetString()
                    });
                    break;

                case JsonValueKind.Object:
                    try
                    {
                        var mod = element.Deserialize<ApiItemModifier>(options);
                        if (mod != null)
                        {
                            list.Add(mod);
                        }
                    }
                    catch
                    {
                        // Fallback: capture raw JSON if deserialization still fails for edge cases
                        list.Add(new ApiItemModifier
                        {
                            Description = element.GetRawText()
                        });
                    }
                    break;

                default:
                    list.Add(new ApiItemModifier
                    {
                        Description = element.GetRawText()
                    });
                    break;
            }
        }

        return list;
    }

    public override void Write(Utf8JsonWriter writer, List<ApiItemModifier> value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartArray();
        foreach (var item in value)
        {
            JsonSerializer.Serialize(writer, item, options);
        }
        writer.WriteEndArray();
    }
}