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

public class CriticalHitChanceProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = game is GameType.PathOfExile1
        ? currentGameLanguage.Language.DescriptionCriticalStrikeChance.ToRegexDoubleProperty()
        : currentGameLanguage.Language.DescriptionCriticalHitChance.ToRegexDoubleProperty();

    private Regex IsAugmentedPattern { get; } = game is GameType.PathOfExile1
        ? currentGameLanguage.Language.DescriptionCriticalStrikeChance.ToRegexIsAugmented()
        : currentGameLanguage.Language.DescriptionCriticalHitChance.ToRegexIsAugmented();

    public override string Label => game == GameType.PathOfExile1 ? currentGameLanguage.Language.DescriptionCriticalStrikeChance : currentGameLanguage.Language.DescriptionCriticalHitChance;

    public override void Parse(Item item)
    {
        if (!item.ItemClass.IsWeapon()) return;

        item.Properties.CriticalHitChance = GetDouble(Pattern, item.Text);
        if (item.Properties.CriticalHitChance == 0) return;

        if (GetBool(IsAugmentedPattern, item.Text)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.CriticalHitChance));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.CriticalHitChance <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new CriticalHitChanceFilter(game)
        {
            Text = Label,
            Value = item.Properties.CriticalHitChance,
            ValueSuffix = "%",
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.CriticalHitChance)) ? LineContentType.Augmented : LineContentType.Simple,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(CriticalHitChanceProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = true,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class CriticalHitChanceFilter : DoublePropertyFilter
{
    public CriticalHitChanceFilter(GameType game)
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
            case GameType.PathOfExile1: query.Filters.GetOrCreateWeaponFilters().Filters.CriticalHitChance = new StatFilterValue(this); break;
            case GameType.PathOfExile2: query.Filters.GetOrCreateEquipmentFilters().Filters.CriticalHitChance = new StatFilterValue(this); break;
        }
    }
}
