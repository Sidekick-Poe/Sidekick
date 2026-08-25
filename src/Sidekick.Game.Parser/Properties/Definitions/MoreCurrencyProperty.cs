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

public class MoreCurrencyProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionMoreCurrency.ToRegexIntProperty();

    private Regex IsAugmentedPattern { get; } = currentGameLanguage.Language.DescriptionMoreCurrency.ToRegexIsAugmented();

    public override string Label => currentGameLanguage.Language.DescriptionMoreCurrency;

    public override void Parse(Item item)
    {
        if (game != GameType.Poe1) return;

        item.Properties.MoreCurrency = GetInt(Pattern, item.Text);
        if (item.Properties.MoreCurrency == 0) return;

        if (GetBool(IsAugmentedPattern, item.Text)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.MoreCurrency));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (game != GameType.Poe1 || item.Properties.MoreCurrency <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new MoreCurrencyFilter
        {
            Text = Label,
            Value = item.Properties.MoreCurrency,
            ValuePrefix = "+",
            ValueSuffix = "%",
            Augmented = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.MoreCurrency)),
            AutoSelectSettingKey = $"Trade_Filter_{nameof(MoreCurrencyProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = true,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class MoreCurrencyFilter : IntPropertyFilter
{
    public MoreCurrencyFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.GetOrCreateStatGroup(StatType.And).Filters.Add(new StatFilters()
        {
            Id = "pseudo.pseudo_map_more_currency_drops",
            Value = new StatFilterValue(this),
        });
    }
}
