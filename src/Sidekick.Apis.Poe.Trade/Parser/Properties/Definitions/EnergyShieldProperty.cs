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

public class EnergyShieldProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionEnergyShield.ToRegexIntProperty();

    private Regex? AlternatePattern { get; } =
        !string.IsNullOrEmpty(currentGameLanguage.Language.DescriptionEnergyShieldAlternate)
            ? currentGameLanguage.Language.DescriptionEnergyShieldAlternate.ToRegexIntProperty()
            : null;

    private Regex IsAugmentedPattern { get; } = currentGameLanguage.Language.DescriptionEnergyShield.ToRegexIsAugmented();

    private Regex? AlternateIsAugmentedPattern { get; } =
        !string.IsNullOrEmpty(currentGameLanguage.Language.DescriptionEnergyShieldAlternate)
            ? currentGameLanguage.Language.DescriptionEnergyShieldAlternate.ToRegexIsAugmented()
            : null;

    public override string Label => currentGameLanguage.Language.DescriptionEnergyShield;

    public override void Parse(Item item)
    {
        if (!item.ItemClass.IsEquipment()) return;

        item.Properties.EnergyShield = GetInt(Pattern, item.Text);
        if (item.Properties.EnergyShield <= 0 && AlternatePattern != null) item.Properties.EnergyShield = GetInt(AlternatePattern, item.Text);
        if (item.Properties.EnergyShield == 0) return;

        if (GetBool(IsAugmentedPattern, item.Text)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.EnergyShield));
        else if (AlternateIsAugmentedPattern != null && GetBool(AlternateIsAugmentedPattern, item.Text)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.EnergyShield));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.EnergyShield <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new EnergyShieldFilter(game)
        {
            Text = Label,
            Value = item.Properties.EnergyShieldWithQuality,
            OriginalValue = item.Properties.EnergyShield,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.EnergyShield)) ? LineContentType.Augmented : LineContentType.Simple,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(EnergyShieldProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = true,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class EnergyShieldFilter : IntPropertyFilter
{
    public EnergyShieldFilter(GameType game)
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
            case GameType.PathOfExile1: query.Filters.GetOrCreateArmourFilters().Filters.EnergyShield = new StatFilterValue(this); break;
            case GameType.PathOfExile2: query.Filters.GetOrCreateEquipmentFilters().Filters.EnergyShield = new StatFilterValue(this); break;
        }
    }
}
