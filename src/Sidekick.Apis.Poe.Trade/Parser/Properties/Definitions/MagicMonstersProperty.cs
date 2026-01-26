using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;
using Sidekick.Common.Enums;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class MagicMonstersProperty(
    GameType game,
    IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionMagicMonsters.ToRegexIntCapture();

    private Regex IsAugmentedPattern { get; } = gameLanguageProvider.Language.DescriptionMagicMonsters.ToRegexIsAugmented();

    public override List<ItemClass> ValidItemClasses { get; } =
    [
        ..ItemClassConstants.Areas,
    ];

    public override string Label => gameLanguageProvider.Language.DescriptionMagicMonsters;

    public override void Parse(Item item)
    {
        var propertyBlock = item.Text.Blocks[1];
        item.Properties.MagicMonsters = GetInt(Pattern, propertyBlock);
        if (item.Properties.MagicMonsters == 0) return;

        propertyBlock.Parsed = true;
        if (GetBool(IsAugmentedPattern, propertyBlock)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.MagicMonsters));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.MagicMonsters <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new MagicMonstersFilter
        {
            Text = Label,
            Value = item.Properties.MagicMonsters,
            ValuePrefix = "+",
            ValueSuffix = "%",
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.MagicMonsters)) ? LineContentType.Augmented : LineContentType.Simple,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(MagicMonstersProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = true,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class MagicMonstersFilter : IntPropertyFilter
{
    public MagicMonstersFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMapFilters().Filters.MagicMonsters = new StatFilterValue(this);
    }
}
