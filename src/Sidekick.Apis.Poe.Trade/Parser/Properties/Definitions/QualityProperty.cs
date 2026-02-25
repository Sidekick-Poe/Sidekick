using System.Text.Json;
using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;
using Sidekick.Common.Enums;
using Sidekick.Data.Items.Models;
using Sidekick.Data.Languages;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class QualityProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionQuality.ToRegexIntCapture();

    private Regex IsAugmentedPattern { get; } = currentGameLanguage.Language.DescriptionQuality.ToRegexIsAugmented();

    public override string Label => currentGameLanguage.Language.DescriptionQuality;

    public override void Parse(Item item)
    {
        var propertyBlock = item.Text.Blocks[1];
        item.Properties.Quality = GetInt(Pattern, propertyBlock);
        if (item.Properties.Quality == 0) return;

        propertyBlock.Parsed = true;
        if (GetBool(IsAugmentedPattern, propertyBlock)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.Quality));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.Quality <= 0) return Task.FromResult<TradeFilter?>(null);

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
