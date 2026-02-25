using System.Text.Json.Serialization;
namespace Sidekick.Data.Builder.Repoe.Models.Poe1;

public class RepoeStatLanguage
{
    [JsonPropertyName("string")]
    public string? Text { get; set; }

    [JsonPropertyName("reminder_text")]
    public string? ReminderText { get; set; }

    [JsonPropertyName("condition")]
    public List<RepoeStatLanguageCondition>? Conditions { get; set; }

    [JsonPropertyName("format")]
    public List<string>? Format { get; set; }

    [JsonPropertyName("index_handlers")]
    public List<List<string>>? Handlers { get; set; }
}
