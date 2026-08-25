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

public class QualityRarityProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionQualityRarity.ToRegexIntProperty();

    private Regex IsAugmentedPattern { get; } = currentGameLanguage.Language.DescriptionQualityRarity.ToRegexIsAugmented();

    public override string Label => currentGameLanguage.Language.DescriptionQualityRarity;

    public override void Parse(Item item)
    {
        if (game != GameType.Poe1) return;

        item.Properties.QualityRarity = GetInt(Pattern, item.Text);
        if (item.Properties.QualityRarity == 0) return;

        if (GetBool(IsAugmentedPattern, item.Text)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.QualityRarity));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (game != GameType.Poe1 || item.Properties.QualityRarity <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new QualityRarityFilter
        {
            Text = Label,
            Value = item.Properties.QualityRarity,
            ValuePrefix = "+",
            ValueSuffix = "%",
            Augmented = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.QualityRarity)),
            AutoSelectSettingKey = $"Trade_Filter_{nameof(QualityRarityProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = true,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class QualityRarityFilter : IntPropertyFilter
{
    public QualityRarityFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.GetOrCreateStatGroup(StatType.And).Filters.Add(new StatFilters()
        {
            Id = "pseudo.pseudo_map_quality_rarity",
            Value = new StatFilterValue(this),
        });
    }
}
