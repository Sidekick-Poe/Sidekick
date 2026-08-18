using System.Text.Json;
using System.Text.RegularExpressions;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings.Languages;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Trade.Requests;
using Sidekick.Game.Parser.Trade.Requests.Filters;
using ItemProperties = Sidekick.Game.Parser.Items.ItemProperties;

namespace Sidekick.Game.Parser.Properties.Definitions;

public class QualityProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionQuality.ToRegexIntProperty();

    private Regex IsAugmentedPattern { get; } = currentGameLanguage.Language.DescriptionQuality.ToRegexIsAugmented();

    public override string Label => currentGameLanguage.Language.DescriptionQuality;

    public override void Parse(Item item)
    {
        if (item.Properties.Rarity != Rarity.Normal &&
            item.Properties.Rarity != Rarity.Magic &&
            item.Properties.Rarity != Rarity.Rare &&
            item.Properties.Rarity != Rarity.Unique &&
            item.Properties.Rarity != Rarity.Gem) return;

        item.Properties.Quality = GetInt(Pattern, item.Text);
        if (item.Properties.Quality == 0) return;

        if (GetBool(IsAugmentedPattern, item.Text)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.Quality));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.Rarity != Rarity.Normal &&
            item.Properties.Rarity != Rarity.Magic &&
            item.Properties.Rarity != Rarity.Rare &&
            item.Properties.Rarity != Rarity.Unique &&
            item.Properties.Rarity != Rarity.Gem) return Task.FromResult<TradeFilter?>(null);

        var filter = new QualityFilter
        {
            Text = Label,
            Value = item.Properties.Quality,
            ValuePrefix = "+",
            ValueSuffix = "%",
            Augmented = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.Quality)),
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
