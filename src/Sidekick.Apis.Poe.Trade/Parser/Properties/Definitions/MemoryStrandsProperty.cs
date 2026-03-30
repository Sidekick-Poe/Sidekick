using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;
using Sidekick.Common.Enums;
using Sidekick.Data;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;
using ItemProperties = Sidekick.Data.Items.ItemProperties;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class MemoryStrandsProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionMemoryStrands.ToRegexIntCapture();

    private Regex IsAugmentedPattern { get; } = currentGameLanguage.Language.DescriptionMemoryStrands.ToRegexIsAugmented();

    public override string Label => currentGameLanguage.Language.DescriptionMemoryStrands;

    public override void Parse(Item item)
    {
        var propertyBlock = item.Text.Blocks[1];
        item.Properties.MemoryStrands = GetInt(Pattern, propertyBlock);
        if (item.Properties.MemoryStrands == 0) return;

        propertyBlock.Parsed = true;
        if (GetBool(IsAugmentedPattern, propertyBlock)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.MemoryStrands));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.MemoryStrands <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new MemoryStrandsFilter
        {
            Text = Label,
            Value = item.Properties.MemoryStrands,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.MemoryStrands)) ? LineContentType.Augmented : LineContentType.Simple,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(MemoryStrandsProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = true,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class MemoryStrandsFilter : IntPropertyFilter
{
    public MemoryStrandsFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMiscFilters().Filters.MemoryStrands = new StatFilterValue(this);
    }
}
