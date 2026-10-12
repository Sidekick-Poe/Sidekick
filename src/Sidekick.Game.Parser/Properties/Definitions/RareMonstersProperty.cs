using System.Text.RegularExpressions;
using Sidekick.Common.Enums;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Trade.Requests;
using Sidekick.Game.Parser.Trade.Requests.Filters;
using Sidekick.Game.Providers;
using ItemProperties = Sidekick.Game.Parser.Items.ItemProperties;

namespace Sidekick.Game.Parser.Properties.Definitions;

public class RareMonstersProperty(
    GameType game,
    GameTextProvider gameTextProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameTextProvider.Texts.ItemPropertyRareMonsters.ToRegexIntProperty();

    private Regex IsAugmentedPattern { get; } = gameTextProvider.Texts.ItemPropertyRareMonsters.ToRegexIsAugmented();

    public override string Label => gameTextProvider.Texts.ItemPropertyRareMonsters;

    public override void Parse(Item item)
    {
        item.Properties.RareMonsters = GetInt(Pattern, item.Text);
        if (item.Properties.RareMonsters == 0) return;

        if (GetBool(IsAugmentedPattern, item.Text)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.RareMonsters));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.RareMonsters <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new RareMonstersFilter
        {
            Text = Label,
            Value = item.Properties.RareMonsters,
            ValuePrefix = "+",
            ValueSuffix = "%",
            Augmented = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.RareMonsters)),
            AutoSelectSettingKey = $"Trade_Filter_{nameof(RareMonstersProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = true,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class RareMonstersFilter : IntPropertyFilter
{
    public RareMonstersFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMapFilters().Filters.RareMonsters = new StatFilterValue(this);
    }
}
