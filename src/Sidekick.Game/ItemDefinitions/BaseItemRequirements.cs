using System.Text.Json.Serialization;

namespace Sidekick.Game.ItemDefinitions;

public class BaseItemRequirements
{
    public int Level { get; init; }
    public int Dexterity { get; init; }
    public int Intelligence { get; init; }
    public int Strength { get; init; }

    [JsonIgnore]
    public bool HasAnyValues => Level != 0 || Dexterity != 0 || Intelligence != 0 || Strength != 0;
}
