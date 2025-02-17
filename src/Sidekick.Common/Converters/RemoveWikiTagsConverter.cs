using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Web;

namespace Sidekick.Common.Converters;

public class RemoveWikiTagsConverter : JsonConverter<string>
{
    private static readonly Regex StripTagsRegex = new Regex(@"\[\[(?(?=File)(?:[^|\]]*)|(?<match>[^|\]]*)).*?\]\]", RegexOptions.Compiled | RegexOptions.Multiline);

    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        value = HttpUtility.HtmlDecode(value);
        if (value == null)
        {
            return string.Empty;
        }

        value = StripTagsRegex.Replace(value, "${match}");
        return value;
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}
