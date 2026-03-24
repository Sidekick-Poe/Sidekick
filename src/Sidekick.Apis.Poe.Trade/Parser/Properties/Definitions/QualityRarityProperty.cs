using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;
using Sidekick.Common.Enums;
using Sidekick.Data;
using Sidekick.Data.Languages;
using ItemProperties = Sidekick.Apis.Poe.Items.ItemProperties;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class QualityRarityProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionQualityRarity.ToRegexIntCapture();

    private Regex IsAugmentedPattern { get; } = currentGameLanguage.Language.DescriptionQualityRarity.ToRegexIsAugmented();

    public override string Label => currentGameLanguage.Language.DescriptionQualityRarity;

    public override void Parse(Item item)
    {
        if (game != GameType.PathOfExile1) return;
        if (!ItemClassConstants.Areas.Contains(item.ItemClass)) return;

        var propertyBlock = item.Text.Blocks[1];
        item.Properties.QualityRarity = GetInt(Pattern, propertyBlock);
        if (item.Properties.QualityRarity == 0) return;

        propertyBlock.Parsed = true;
        if (GetBool(IsAugmentedPattern, propertyBlock)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.QualityRarity));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (game != GameType.PathOfExile1 || item.Properties.QualityRarity <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new QualityRarityFilter
        {
            Text = Label,
            Value = item.Properties.QualityRarity,
            ValuePrefix = "+",
            ValueSuffix = "%",
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.QualityRarity)) ? LineContentType.Augmented : LineContentType.Simple,
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
