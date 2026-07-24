using Sidekick.Apis.Poe.Trade.Trade.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Trade.Trade.Converters;

public class StringOrModifierListConverter : JsonConverter<List<ApiItemModifier>>
{
    public override List<ApiItemModifier> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var list = new List<ApiItemModifier>();

        if (reader.TokenType == JsonTokenType.Null)
        {
            return list;
        }

        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException("Expected start of array for explicitMods.");
        }

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
            {
                break;
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                var s = reader.GetString();
                list.Add(new ApiItemModifier { Description = s });
                continue;
            }

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                using var doc = JsonDocument.ParseValue(ref reader);
                var elem = doc.RootElement;
                try
                {
                    var mod = elem.Deserialize<ApiItemModifier>(options);
                    if (mod != null)
                    {
                        list.Add(mod);
                    }
                    else
                    {
                        list.Add(new ApiItemModifier { Description = elem.GetRawText() });
                    }
                }
                catch
                {
                    list.Add(new ApiItemModifier { Description = elem.GetRawText() });
                }

                continue;
            }

            // Fallback: parse whatever token into a string
            using var fallback = JsonDocument.ParseValue(ref reader);
            list.Add(new ApiItemModifier { Description = fallback.RootElement.GetRawText() });
        }

        return list;
    }

    public override void Write(Utf8JsonWriter writer, List<ApiItemModifier> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var em in value)
        {
            writer.WriteStartObject();
            if (em.Description != null)
            {
                writer.WriteString("description", em.Description);
            }
            if (em.Hash != null)
            {
                writer.WriteString("hash", em.Hash);
            }
            if (em.Details != null && em.Details.Count > 0)
            {
                writer.WritePropertyName("mods");
                JsonSerializer.Serialize(writer, em.Details, options);
            }
            writer.WriteEndObject();
        }
        writer.WriteEndArray();
    }
}
