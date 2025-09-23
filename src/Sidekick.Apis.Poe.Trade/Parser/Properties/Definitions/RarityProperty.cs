using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class RarityProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Dictionary<Rarity, Regex> RarityPatterns { get; } = new()
    {
        {
            Rarity.Normal, gameLanguageProvider.Language.RarityNormal.ToRegexEndOfLine()
        },
        {
            Rarity.Magic, gameLanguageProvider.Language.RarityMagic.ToRegexEndOfLine()
        },
        {
            Rarity.Rare, gameLanguageProvider.Language.RarityRare.ToRegexEndOfLine()
        },
        {
            Rarity.Unique, gameLanguageProvider.Language.RarityUnique.ToRegexEndOfLine()
        },
        {
            Rarity.Currency, gameLanguageProvider.Language.RarityCurrency.ToRegexEndOfLine()
        },
        {
            Rarity.Gem, gameLanguageProvider.Language.RarityGem.ToRegexEndOfLine()
        },
        {
            Rarity.DivinationCard, gameLanguageProvider.Language.RarityDivinationCard.ToRegexEndOfLine()
        }
    };

    public override List<Category> ValidCategories { get; } = [];

    public override void Parse(Item item)
    {
        if (item.Header != null!)
        {
            if (item.Header.IsUnique)
            {
                item.Properties.Rarity = Rarity.Unique;
                return;
            }

            item.Properties.Rarity = item.Header.Category switch
            {
                Category.DivinationCard => Rarity.DivinationCard,
                Category.Gem => Rarity.Gem,
                Category.Currency => Rarity.Currency,
                _ => Rarity.Unknown,
            };
        }

        if (item.Properties.Rarity != Rarity.Unknown) return;

        foreach (var pattern in RarityPatterns)
        {
            if (!pattern.Value.IsMatch(item.Text.Blocks[0].Lines[1].Text)) continue;

            item.Text.Blocks[0].Lines[1].Parsed = true;
            item.Properties.Rarity = pattern.Key;
            return;
        }
    }

    public override Task<PropertyFilter?> GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        if (item.Properties.Rarity is not (Rarity.Rare or Rarity.Magic or Rarity.Normal)) return Task.FromResult<PropertyFilter?>(null);

        var rarityLabel = item.Properties.Rarity switch
        {
            Rarity.Currency => gameLanguageProvider.Language.RarityCurrency,
            Rarity.Normal => gameLanguageProvider.Language.RarityNormal,
            Rarity.Magic => gameLanguageProvider.Language.RarityMagic,
            Rarity.Rare => gameLanguageProvider.Language.RarityRare,
            Rarity.Unique => gameLanguageProvider.Language.RarityUnique,
            _ => null
        };
        if (rarityLabel == null) return Task.FromResult<PropertyFilter?>(null);

        var filter = new StringPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionRarity,
            Value = rarityLabel,
            Checked = false,
        };
        return Task.FromResult<PropertyFilter?>(filter);
    }

    public override void PrepareTradeRequest(Query query, Item item, PropertyFilter filter)
    {
        if (item.Properties.Rarity == Rarity.Unique)
        {
            query.Filters.GetOrCreateTypeFilters().Filters.Rarity = new SearchFilterOption("unique");
            return;
        }

        if (!filter.Checked || filter is not StringPropertyFilter) return;

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
