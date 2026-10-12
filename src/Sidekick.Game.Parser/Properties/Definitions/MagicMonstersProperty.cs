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

public class MagicMonstersProperty(
    GameType game,
    GameTextProvider gameTextProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameTextProvider.Texts.ItemPropertyMagicMonsters.ToRegexIntProperty();

    private Regex IsAugmentedPattern { get; } = gameTextProvider.Texts.ItemPropertyMagicMonsters.ToRegexIsAugmented();

    public override string Label => gameTextProvider.Texts.ItemPropertyMagicMonsters;

    public override void Parse(Item item)
    {
        item.Properties.MagicMonsters = GetInt(Pattern, item.Text);
        if (item.Properties.MagicMonsters == 0) return;

        if (GetBool(IsAugmentedPattern, item.Text)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.MagicMonsters));
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
            Augmented = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.MagicMonsters)),
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
