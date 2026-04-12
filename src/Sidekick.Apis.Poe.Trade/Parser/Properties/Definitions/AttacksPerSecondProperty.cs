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

public class AttacksPerSecondProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionAttacksPerSecond.ToRegexDoubleProperty();

    private Regex IsAugmentedPattern { get; } = currentGameLanguage.Language.DescriptionAttacksPerSecond.ToRegexIsAugmented();

    public override string Label => currentGameLanguage.Language.DescriptionAttacksPerSecond;

    public override void Parse(Item item)
    {
        if (!item.ItemClass.IsWeapon()) return;

        item.Properties.AttacksPerSecond = GetDouble(Pattern, item.Text);
        if (item.Properties.AttacksPerSecond == 0) return;

        if (GetBool(IsAugmentedPattern, item.Text)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.AttacksPerSecond));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.AttacksPerSecond <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new AttacksPerSecondFilter(game)
        {
            Text = Label,
            Value = item.Properties.AttacksPerSecond,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.AttacksPerSecond)) ? LineContentType.Augmented : LineContentType.Simple,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(AttacksPerSecondProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = true,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class AttacksPerSecondFilter : DoublePropertyFilter
{
    public AttacksPerSecondFilter(GameType game)
    {
        Game = game;
        DefaultAutoSelect = AutoSelectPreferences.Create(false);
    }

    private GameType Game { get; }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        switch (Game)
        {
            case GameType.PathOfExile1: query.Filters.GetOrCreateWeaponFilters().Filters.AttacksPerSecond = new StatFilterValue(this); break;
            case GameType.PathOfExile2: query.Filters.GetOrCreateEquipmentFilters().Filters.AttacksPerSecond = new StatFilterValue(this); break;
        }
    }
}
