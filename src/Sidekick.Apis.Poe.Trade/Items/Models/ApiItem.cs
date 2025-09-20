using System.Text.Json.Serialization;
using Sidekick.Apis.Poe.Items;

namespace Sidekick.Apis.Poe.Trade.Items.Models;

public class ApiItem
{
    public string? Name { get; init; }

    public string? Type { get; init; }

    public string? Text { get; init; }

    [JsonPropertyName("disc")]
    public string? Discriminator { get; init; }

    public ApiItemFlags? Flags { get; init; }

    [JsonIgnore]
    public string? Id { get; set; }

    [JsonIgnore]
    public GameType Game { get; set; }

    [JsonIgnore]
    public Category? Category { get; set; }

    [JsonIgnore]
    public bool IsUnique => Flags?.Unique ?? false;

    public ItemHeader ToHeader()
    {
        var categoryRarity = Category switch
        {
            Poe.Items.Category.DivinationCard => Rarity.DivinationCard,
            Poe.Items.Category.Gem => Rarity.Gem,
            Poe.Items.Category.Currency => Rarity.Currency,
            _ => Rarity.Unknown
        };

        return new ItemHeader()
        {
            Name = Name,
            Type = Type,
            ApiItemId = Id ?? string.Empty,
            ApiName = Name,
            ApiType = Type,
            ApiDiscriminator = Discriminator,
            ApiText = Text,
            Game = Game,
            Category = Category ?? Poe.Items.Category.Unknown,
            Rarity = IsUnique ? Rarity.Unique : categoryRarity,
        };
    }
}
