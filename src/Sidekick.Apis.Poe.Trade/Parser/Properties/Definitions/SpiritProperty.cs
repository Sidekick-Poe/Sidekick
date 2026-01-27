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

public class SpiritProperty(
    GameType game,
    IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionSpirit.ToRegexIntCapture();

    private Regex IsAugmentedPattern { get; } = gameLanguageProvider.Language.DescriptionSpirit.ToRegexIsAugmented();

    public override string Label => gameLanguageProvider.Language.DescriptionSpirit;

    public override void Parse(Item item)
    {
        if (!ItemClassConstants.Weapons.Contains(item.Properties.ItemClass) &&
            !ItemClassConstants.Equipment.Contains(item.Properties.ItemClass)) return;

        if (game == GameType.PathOfExile1) return;
        var propertyBlock = item.Text.Blocks[1];
        item.Properties.Spirit = GetInt(Pattern, propertyBlock);
        if (item.Properties.Spirit == 0) return;

        propertyBlock.Parsed = true;
        if (GetBool(IsAugmentedPattern, propertyBlock)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.Spirit));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (game == GameType.PathOfExile1 || item.Properties.Spirit <= 0) return Task.FromResult<TradeFilter?>(null);

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
