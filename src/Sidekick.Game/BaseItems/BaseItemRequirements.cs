using System.Text.Json.Serialization;

namespace Sidekick.Game.BaseItems;

public class BaseItemRequirements
{
    [JsonPropertyName("lvl")]
    public int Level { get; init; }

    [JsonPropertyName("dex")]
    public int Dexterity { get; init; }

    [JsonPropertyName("int")]
    public int Intelligence { get; init; }

    [JsonPropertyName("str")]
    public int Strength { get; init; }

    [JsonIgnore]
    public bool HasAnyValues => Level != 0 || Dexterity != 0 || Intelligence != 0 || Strength != 0;
}
