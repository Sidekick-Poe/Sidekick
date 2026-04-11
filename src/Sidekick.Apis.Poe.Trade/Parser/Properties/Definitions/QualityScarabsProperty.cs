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

public class QualityScarabsProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionQualityScarabs.ToRegexIntProperty();

    private Regex IsAugmentedPattern { get; } = currentGameLanguage.Language.DescriptionQualityScarabs.ToRegexIsAugmented();

    public override string Label => currentGameLanguage.Language.DescriptionQualityScarabs;

    public override void Parse(Item item)
    {
        if (game != GameType.PathOfExile1) return;

        item.Properties.QualityScarabs = GetInt(Pattern, item.Text);
        if (item.Properties.QualityScarabs == 0) return;

        if (GetBool(IsAugmentedPattern, item.Text)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.QualityScarabs));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (game != GameType.PathOfExile1 || item.Properties.QualityScarabs <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new QualityScarabsFilter
        {
            Text = Label,
            Value = item.Properties.QualityScarabs,
            ValuePrefix = "+",
            ValueSuffix = "%",
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.QualityScarabs)) ? LineContentType.Augmented : LineContentType.Simple,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(QualityScarabsProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = true,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class QualityScarabsFilter : IntPropertyFilter
{
    public QualityScarabsFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.GetOrCreateStatGroup(StatType.And).Filters.Add(new StatFilters()
        {
            Id = "pseudo.pseudo_map_quality_scarabs",
            Value = new StatFilterValue(this),
        });
    }
}
