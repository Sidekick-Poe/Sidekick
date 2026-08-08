using System.Text.Json;
using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Data;
using Sidekick.Data.Items;
using Sidekick.Data.Texts;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class RarityProperty(
    GameType game,
    GameTextProvider gameTextProvider) : PropertyDefinition
{
    private Dictionary<Rarity, Regex> RarityPatterns { get; } = new()
    {
        {
            Rarity.Normal, gameTextProvider.Texts.ItemPropertyRarityNormal.ToRegexEndOfLine()
        },
        {
            Rarity.Magic, gameTextProvider.Texts.ItemPropertyRarityMagic.ToRegexEndOfLine()
        },
        {
            Rarity.Rare, gameTextProvider.Texts.ItemPropertyRarityRare.ToRegexEndOfLine()
        },
        {
            Rarity.Unique, gameTextProvider.Texts.ItemPropertyRarityUnique.ToRegexEndOfLine()
        },
        {
            Rarity.Currency, gameTextProvider.Texts.ItemPropertyRarityCurrency.ToRegexEndOfLine()
        },
        {
            Rarity.Gem, gameTextProvider.Texts.ItemPropertyRarityGem.ToRegexEndOfLine()
        },
        {
            Rarity.DivinationCard, gameTextProvider.Texts.ItemPropertyRarityDivinationCard.ToRegexEndOfLine()
        }
    };

    public override string Label => gameTextProvider.Texts.ItemPropertyRarity;

    public override void Parse(Item item)
    {
        item.Properties.Rarity = Rarity.Unknown;

        var propertyBlock = item.Text.Blocks[0];
        foreach (var pattern in RarityPatterns)
        {
            if (!GetBool(pattern.Value, propertyBlock)) continue;

            item.Text.Blocks[0].Parsed = true;
            item.Properties.Rarity = pattern.Key;
            break;
        }
    }

    public override void ParseAfterStats(Item item)
    {
        if (item.Definition.IsUnique) item.Properties.Rarity = Rarity.Unique;

        base.ParseAfterStats(item);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.Rarity is not (Rarity.Rare or Rarity.Magic or Rarity.Normal or Rarity.Unique)) return Task.FromResult<TradeFilter?>(null);

        var rarityLabel = item.Properties.Rarity switch
        {
            Rarity.Currency => gameTextProvider.Texts.ItemPropertyRarityCurrency,
            Rarity.Normal => gameTextProvider.Texts.ItemPropertyRarityNormal,
            Rarity.Magic => gameTextProvider.Texts.ItemPropertyRarityMagic,
            Rarity.Rare => gameTextProvider.Texts.ItemPropertyRarityRare,
            Rarity.Unique => gameTextProvider.Texts.ItemPropertyRarityUnique,
            _ => null
        };
        if (rarityLabel == null) return Task.FromResult<TradeFilter?>(null);

        if (item.Properties.Rarity == Rarity.Unique)
        {
            return Task.FromResult<TradeFilter?>(new UniqueRarityFilter());
        }

        var filter = new RarityFilter(item.Game)
        {
            Text = gameTextProvider.Texts.ItemPropertyRarity,
            Value = rarityLabel,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(RarityProperty)}_{game.GetValueAttribute()}",
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class RarityFilter : StringPropertyFilter
{
    public RarityFilter(GameType game)
    {
        if (game == GameType.PathOfExile1)
        {
            DefaultAutoSelect = AutoSelectPreferences.Create(false);
        }
        else
        {
            DefaultAutoSelect = new AutoSelectPreferences
            {
                Mode = AutoSelectMode.Default,
                Rules =
                [
                    new AutoSelectRule()
                    {
                        Checked = true,
                        Conditions =
                        [
                            new AutoSelectCondition()
                            {
                                Type = AutoSelectConditionType.Rarity,
                                Comparison = AutoSelectComparisonType.IsContainedIn,
                                Value = JsonSerializer.Serialize(new List<Rarity>()
                                {
                                    Rarity.Normal,
                                    Rarity.Magic,
                                }, AutoSelectPreferences.JsonSerializerOptions),
                            },
                        ],
                    },
                ],
            };

        }
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        var rarity = item.Properties.Rarity switch
        {
            Rarity.Normal => "normal",
            Rarity.Magic => "magic",
            Rarity.Rare => "rare",
            Rarity.Unique => "unique",
            _ => "nonunique",
        };

        query.Filters.GetOrCreateTypeFilters().Filters.Rarity = new SearchFilterOption(rarity);
    }
}

public class UniqueRarityFilter : HiddenFilter
{
    public UniqueRarityFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(true);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        query.Filters.GetOrCreateTypeFilters().Filters.Rarity = new SearchFilterOption("unique");
    }
}
