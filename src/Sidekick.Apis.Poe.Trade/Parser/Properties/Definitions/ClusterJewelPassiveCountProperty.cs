using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Modifiers;
namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class ClusterJewelPassiveCountProperty(IServiceProvider serviceProvider, GameType game) : PropertyDefinition
{
    private IInvariantModifierProvider InvariantModifierProvider { get; } = serviceProvider.GetRequiredService<IInvariantModifierProvider>();

    public override List<ItemClass> ValidItemClasses { get; } = [ItemClass.Jewel];

    public override void ParseAfterModifiers(Item item)
    {
        if (game == GameType.PathOfExile2) return;
        if (item.Properties.Rarity == Rarity.Unique) return;

        var passiveCount = ParseSmallPassiveCount(item.Modifiers);
        if (passiveCount != 0)
        {
            item.Properties.ClusterJewelPassiveCount = passiveCount;
        }

        var grant = ParseGrantTexts(item.Modifiers);
        if (grant != null) item.Properties.ClusterJewelGrantText = grant;
    }

    private int ParseSmallPassiveCount(List<Modifier> modifierLines)
    {
        foreach (var modifierLine in modifierLines)
        {
            if (!modifierLine.HasValues)
            {
                continue;
            }

            foreach (var modifier in modifierLine.ApiInformation)
            {
                if (modifier.ApiId == InvariantModifierProvider.ClusterJewelSmallPassiveCountModifierId)
                {
                    return (int)modifierLine.AverageValue;
                }
            }
        }

        return 0;
    }

    private string? ParseGrantTexts(List<Modifier> modifierLines)
    {
        foreach (var modifierLine in modifierLines)
        {
            if (!modifierLine.OptionValue.HasValue)
            {
                continue;
            }

            foreach (var modifier in modifierLine.ApiInformation)
            {
                if (modifier.ApiId == InvariantModifierProvider.ClusterJewelSmallPassiveGrantModifierId)
                {
                    return InvariantModifierProvider.ClusterJewelSmallPassiveGrantOptions[modifierLine.OptionValue.Value].Replace("\n", ", ");
                }
            }
        }

        return null;
    }
}
