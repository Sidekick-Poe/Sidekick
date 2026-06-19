using Sidekick.Apis.Poe.Trade.Trade.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Trade.Trade.Converters;

public class StringOrExplicitModListConverter : JsonConverter<List<ExplicitMod>>
{
    public override List<ExplicitMod> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var list = new List<ExplicitMod>();

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
                list.Add(new ExplicitMod { Description = s });
                continue;
            }

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                using var doc = JsonDocument.ParseValue(ref reader);
                var elem = doc.RootElement;
                try
                {
                    var mod = elem.Deserialize<ExplicitMod>(options);
                    if (mod != null)
                    {
                        list.Add(mod);
                    }
                    else
                    {
                        list.Add(new ExplicitMod { Description = elem.GetRawText() });
                    }
                }
                catch
                {
                    list.Add(new ExplicitMod { Description = elem.GetRawText() });
                }

                continue;
            }

            // Fallback: parse whatever token into a string
            using var fallback = JsonDocument.ParseValue(ref reader);
            list.Add(new ExplicitMod { Description = fallback.RootElement.GetRawText() });
        }

        return list;
    }

    public override void Write(Utf8JsonWriter writer, List<ExplicitMod> value, JsonSerializerOptions options)
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
            if (em.Mods != null && em.Mods.Count > 0)
            {
                writer.WritePropertyName("mods");
                JsonSerializer.Serialize(writer, em.Mods, options);
            }
            writer.WriteEndObject();
        }
        writer.WriteEndArray();
    }
}
