using System.Text.Json.Serialization;
namespace Sidekick.Data.Builder.Repoe.Models.Poe1;

public class RepoeStatTranslation
{
    public List<string> Ids { get; set; } = [];

    [JsonPropertyName("trade_stats")]
    public List<RepoeStatTrade> TradeStats { get; set; } = [];

    [JsonPropertyName("English")]
    public List<RepoeStatLanguage>? English { get; set; }

    [JsonPropertyName("French")]
    public List<RepoeStatLanguage>? French { get; set; }

    [JsonPropertyName("German")]
    public List<RepoeStatLanguage>? German { get; set; }

    [JsonPropertyName("Japanese")]
    public List<RepoeStatLanguage>? Japanese { get; set; }

    [JsonPropertyName("Korean")]
    public List<RepoeStatLanguage>? Korean { get; set; }

    [JsonPropertyName("Portuguese")]
    public List<RepoeStatLanguage>? Portuguese { get; set; }

    [JsonPropertyName("Russian")]
    public List<RepoeStatLanguage>? Russian { get; set; }

    [JsonPropertyName("Spanish")]
    public List<RepoeStatLanguage>? Spanish { get; set; }

    [JsonPropertyName("Thai")]
    public List<RepoeStatLanguage>? Thai { get; set; }

    [JsonPropertyName("Traditional_Chinese")]
    public List<RepoeStatLanguage>? TraditionalChinese { get; set; }

    [JsonIgnore]
    public List<RepoeStatLanguage>? Languages
    {
        get
        {
            if (English != null) return English;
            if (French != null) return French;
            if (German != null) return German;
            if (Japanese != null) return Japanese;
            if (Korean != null) return Korean;
            if (Portuguese != null) return Portuguese;
            if (Russian != null) return Russian;
            if (Spanish != null) return Spanish;
            if (Thai != null) return Thai;
            if (TraditionalChinese != null) return TraditionalChinese;

            return null;
        }
    }
}
