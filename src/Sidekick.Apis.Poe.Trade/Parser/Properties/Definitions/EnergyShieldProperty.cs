using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class EnergyShieldProperty
(
    IGameLanguageProvider gameLanguageProvider,
    GameType game
) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionEnergyShield.ToRegexIntCapture();

    private Regex? AlternatePattern { get; } =
        !string.IsNullOrEmpty(gameLanguageProvider.Language.DescriptionEnergyShieldAlternate)
            ? gameLanguageProvider.Language.DescriptionEnergyShieldAlternate.ToRegexIntCapture()
            : null;

    private Regex IsAugmentedPattern { get; } = gameLanguageProvider.Language.DescriptionEnergyShield.ToRegexIsAugmented();

    private Regex? AlternateIsAugmentedPattern { get; } =
        !string.IsNullOrEmpty(gameLanguageProvider.Language.DescriptionEnergyShieldAlternate)
            ? gameLanguageProvider.Language.DescriptionEnergyShieldAlternate.ToRegexIsAugmented()
            : null;

    public override List<ItemClass> ValidItemClasses { get; } = [
        ..ItemClassConstants.Equipment,
    ];

    public override void Parse(Item item)
    {
        var propertyBlock = item.Text.Blocks[1];
        item.Properties.EnergyShield = GetInt(Pattern, propertyBlock);
        if (item.Properties.EnergyShield <= 0 && AlternatePattern != null) item.Properties.EnergyShield = GetInt(AlternatePattern, propertyBlock);
        if (item.Properties.EnergyShield == 0) return;

        propertyBlock.Parsed = true;
        if (GetBool(IsAugmentedPattern, propertyBlock)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.EnergyShield));
        else if (AlternateIsAugmentedPattern != null && GetBool(AlternateIsAugmentedPattern, propertyBlock)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.EnergyShield));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.EnergyShield <= 0) return Task.FromResult<TradeFilter?>(null);

        var text = gameLanguageProvider.Language.DescriptionEnergyShield;
        if (!string.IsNullOrEmpty(gameLanguageProvider.Language.DescriptionEnergyShieldAlternate) && item.Game == GameType.PathOfExile2)
        {
            text = gameLanguageProvider.Language.DescriptionEnergyShieldAlternate;
        }

        var filter = new EnergyShieldFilter(game)
        {
            Text = text,
            NormalizeEnabled = true,
            Value = item.Properties.EnergyShieldWithQuality,
            OriginalValue = item.Properties.EnergyShield,
            Checked = false,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.EnergyShield)) ? LineContentType.Augmented : LineContentType.Simple,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class EnergyShieldFilter(GameType game) : IntPropertyFilter
{
    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        switch (game)
        {
            case GameType.PathOfExile1: query.Filters.GetOrCreateArmourFilters().Filters.EnergyShield = new StatFilterValue(this); break;
            case GameType.PathOfExile2: query.Filters.GetOrCreateEquipmentFilters().Filters.EnergyShield = new StatFilterValue(this); break;
        }
    }
}
