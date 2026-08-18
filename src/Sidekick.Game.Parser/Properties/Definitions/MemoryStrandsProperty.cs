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

public class MemoryStrandsProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionMemoryStrands.ToRegexIntProperty();

    private Regex IsAugmentedPattern { get; } = currentGameLanguage.Language.DescriptionMemoryStrands.ToRegexIsAugmented();

    public override string Label => currentGameLanguage.Language.DescriptionMemoryStrands;

    public override void Parse(Item item)
    {
        item.Properties.MemoryStrands = GetInt(Pattern, item.Text);
        if (item.Properties.MemoryStrands == 0) return;

        if (GetBool(IsAugmentedPattern, item.Text)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.MemoryStrands));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.MemoryStrands <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new MemoryStrandsFilter
        {
            Text = Label,
            Value = item.Properties.MemoryStrands,
            Augmented = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.MemoryStrands)),
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
