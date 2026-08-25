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

public class QualityPackSizeProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionQualityPackSize.ToRegexIntProperty();

    private Regex IsAugmentedPattern { get; } = currentGameLanguage.Language.DescriptionQualityPackSize.ToRegexIsAugmented();

    public override string Label => currentGameLanguage.Language.DescriptionQualityPackSize;

    public override void Parse(Item item)
    {
        if (game != GameType.Poe1) return;

        item.Properties.QualityPackSize = GetInt(Pattern, item.Text);
        if (item.Properties.QualityPackSize == 0) return;

        if (GetBool(IsAugmentedPattern, item.Text)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.QualityPackSize));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (game != GameType.Poe1 || item.Properties.QualityPackSize <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new QualityPackSizeFilter
        {
            Text = Label,
            Value = item.Properties.QualityPackSize,
            ValuePrefix = "+",
            ValueSuffix = "%",
            Augmented = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.QualityPackSize)),
            AutoSelectSettingKey = $"Trade_Filter_{nameof(QualityPackSizeProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = true,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class QualityPackSizeFilter : IntPropertyFilter
{
    public QualityPackSizeFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.GetOrCreateStatGroup(StatType.And).Filters.Add(new StatFilters()
        {
            Id = "pseudo.pseudo_map_quality_pack_size",
            Value = new StatFilterValue(this),
        });
    }
}
