using System.Text.Json;
using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Results;
using Sidekick.Common.Enums;
using Sidekick.Data;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;
using ItemProperties = Sidekick.Data.Items.ItemProperties;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class QualityProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionQuality.ToRegexIntProperty();

    private Regex IsAugmentedPattern { get; } = currentGameLanguage.Language.DescriptionQuality.ToRegexIsAugmented();

    public override string Label => currentGameLanguage.Language.DescriptionQuality;

    public override void Parse(Item item)
    {
        item.Properties.Quality = GetInt(Pattern, item.Text);
        if (item.Properties.Quality == 0) return;

        if (GetBool(IsAugmentedPattern, item.Text)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.Quality));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (!item.ItemClass.IsGem() &&
            !item.ItemClass.IsEquipment() &&
            !item.ItemClass.IsWeapon() &&
            !item.ItemClass.IsFlask()) return Task.FromResult<TradeFilter?>(null);

        var filter = new QualityFilter
        {
            Text = Label,
            Value = item.Properties.Quality,
            ValuePrefix = "+",
            ValueSuffix = "%",
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.Quality)) ? LineContentType.Augmented : LineContentType.Simple,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(QualityProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = true,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class QualityFilter : IntPropertyFilter
{
    public QualityFilter()
    {
        DefaultAutoSelect = new AutoSelectPreferences()
        {
            Mode = AutoSelectMode.Default,
            Rules =
            [
                new()
                {
                    Checked = true,
                    NormalizeBy = 0,
                    FillMinRange = true,
                    FillMaxRange = false,
                    Conditions =
                    [
                        new()
                        {
                            Type = AutoSelectConditionType.Rarity,
                            Comparison = AutoSelectComparisonType.IsContainedIn,
                            Value = JsonSerializer.Serialize(new List<Rarity>()
                            {
                                Rarity.Gem,
                            }, AutoSelectPreferences.JsonSerializerOptions),
                        },
                    ],
                },
                new()
                {
                    Checked = true,
                    NormalizeBy = 0,
                    FillMinRange = true,
                    FillMaxRange = false,
                    Conditions =
                    [
                        new()
                        {
                            Type = AutoSelectConditionType.Quality,
                            Comparison = AutoSelectComparisonType.GreaterThan,
                            Value = 20.ToString(),
                        },
                    ],
                },
            ],
        };
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMiscFilters().Filters.Quality = new StatFilterValue(this);
    }
}
