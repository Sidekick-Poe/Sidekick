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

public class MoreScarabsProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionMoreScarabs.ToRegexIntProperty();

    private Regex IsAugmentedPattern { get; } = currentGameLanguage.Language.DescriptionMoreScarabs.ToRegexIsAugmented();

    public override string Label => currentGameLanguage.Language.DescriptionMoreScarabs;

    public override void Parse(Item item)
    {
        if (game != GameType.Poe1) return;

        item.Properties.MoreScarabs = GetInt(Pattern, item.Text);
        if (item.Properties.MoreScarabs == 0) return;

        if (GetBool(IsAugmentedPattern, item.Text)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.MoreScarabs));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (game != GameType.Poe1 || item.Properties.MoreScarabs <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new MoreScarabsFilter
        {
            Text = Label,
            Value = item.Properties.MoreScarabs,
            ValuePrefix = "+",
            ValueSuffix = "%",
            Augmented = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.MoreScarabs)),
            AutoSelectSettingKey = $"Trade_Filter_{nameof(MoreScarabsProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = true,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class MoreScarabsFilter : IntPropertyFilter
{
    public MoreScarabsFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.GetOrCreateStatGroup(StatType.And).Filters.Add(new StatFilters()
        {
            Id = "pseudo.pseudo_map_more_scarab_drops",
            Value = new StatFilterValue(this),
        });
    }
}
