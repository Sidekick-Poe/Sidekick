using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class RarityProperty(
    GameType game,
    ISettingsService settingsService,
    IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
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

    public override List<ItemClass> ValidItemClasses { get; } = [];

    public override string Label => gameLanguageProvider.Language.DescriptionRarity;

    public override void Parse(Item item)
    {
        if (item.ApiInformation != null! && item.ApiInformation.IsUnique)
        {
            item.Properties.Rarity = Rarity.Unique;
            return;
        }

        if (item.Properties.ItemClass == ItemClass.DivinationCard)
        {
            item.Properties.Rarity = Rarity.DivinationCard;
            return;
        }

        if (ItemClassConstants.Gems.Contains(item.Properties.ItemClass))
        {
            item.Properties.Rarity = Rarity.Gem;
            return;
        }

        foreach (var pattern in RarityPatterns)
        {
            if (!pattern.Value.IsMatch(item.Text.Blocks[0].Lines[1].Text)) continue;

            item.Text.Blocks[0].Lines[1].Parsed = true;
            item.Properties.Rarity = pattern.Key;
            return;
        }

        if (item.Properties.ItemClass == ItemClass.Currency)
        {
            item.Properties.Rarity = Rarity.Currency;
            return;
        }

        item.Properties.Rarity = Rarity.Unknown;
    }

    public override async Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.Rarity is not (Rarity.Rare or Rarity.Magic or Rarity.Normal or Rarity.Unique)) return null;

        var rarityLabel = item.Properties.Rarity switch
        {
            Rarity.Currency => gameLanguageProvider.Language.RarityCurrency,
            Rarity.Normal => gameLanguageProvider.Language.RarityNormal,
            Rarity.Magic => gameLanguageProvider.Language.RarityMagic,
            Rarity.Rare => gameLanguageProvider.Language.RarityRare,
            Rarity.Unique => gameLanguageProvider.Language.RarityUnique,
            _ => null
        };
        if (rarityLabel == null) return null;

        var autoSelectKey = $"Trade_Filter_{nameof(RarityProperty)}_{game.GetValueAttribute()}";
        if (item.Properties.Rarity == Rarity.Unique)
        {
            return new UniqueRarityFilter
            {
                AutoSelectSettingKey = autoSelectKey,
                AutoSelect = await settingsService.GetObject<AutoSelectPreferences>(autoSelectKey, () => null),
            };
        }

        var filter = new RarityFilter
        {
            Text = gameLanguageProvider.Language.DescriptionRarity,
            Value = rarityLabel,
            AutoSelectSettingKey = autoSelectKey,
            AutoSelect = await settingsService.GetObject<AutoSelectPreferences>(autoSelectKey, () => null),
        };
        return filter;
    }
}

public class RarityFilter : StringPropertyFilter
{
    public RarityFilter()
    {
        DefaultAutoSelect = new AutoSelectPreferences()
        {
            Mode = AutoSelectMode.Never,
        };
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
        DefaultAutoSelect = new AutoSelectPreferences()
        {
            Mode = AutoSelectMode.Always,
        };
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        query.Filters.GetOrCreateTypeFilters().Filters.Rarity = new SearchFilterOption("unique");
    }
}
