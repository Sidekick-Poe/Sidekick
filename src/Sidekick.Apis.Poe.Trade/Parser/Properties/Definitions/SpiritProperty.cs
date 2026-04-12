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

public class SpiritProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionSpirit.ToRegexIntProperty();

    private Regex IsAugmentedPattern { get; } = currentGameLanguage.Language.DescriptionSpirit.ToRegexIsAugmented();

    public override string Label => currentGameLanguage.Language.DescriptionSpirit;

    public override void Parse(Item item)
    {
        if (!item.ItemClass.IsWeapon() &&
            !item.ItemClass.IsEquipment()) return;

        if (game == GameType.PathOfExile1) return;

        item.Properties.Spirit = GetInt(Pattern, item.Text);
        if (item.Properties.Spirit == 0) return;

        if (GetBool(IsAugmentedPattern, item.Text)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.Spirit));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.Spirit <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new SpiritFilter
        {
            Text = Label,
            Value = item.Properties.Spirit,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.Spirit)) ? LineContentType.Augmented : LineContentType.Simple,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(SpiritProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = true,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class SpiritFilter : IntPropertyFilter
{
    public SpiritFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateEquipmentFilters().Filters.Spirit = new StatFilterValue(this);
    }
}
