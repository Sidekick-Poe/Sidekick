using System.Diagnostics.CodeAnalysis;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Items.AdditionalInformation;
using Sidekick.Apis.Poe.Trade.Modifiers;

namespace Sidekick.Apis.Poe.Trade.Parser.AdditionalInformation;

public class ClusterJewelSmallPassiveCountProperty(IInvariantModifierProvider invariantModifierProvider)
{
    public bool TryParse(ItemHeader itemHeader, List<ModifierLine> modifierLines, [NotNullWhen(true)] out ClusterJewelInformation? information)
    {
        information = null;

        if (itemHeader.Category != Category.Jewel || itemHeader.Rarity == Rarity.Unique)
        {
            return false;
        }

        var smallPassiveCount = ParseSmallPassiveCount(modifierLines);
        if (smallPassiveCount == 0)
        {
            return false;
        }

        var grant = ParseGrantTexts(modifierLines);
        if (grant == null)
        {
            return false;
        }

        information = new ClusterJewelInformation()
        {
            SmallPassiveCount = smallPassiveCount,
            GrantText = grant,
        };

        return true;
    }

    public int ParseSmallPassiveCount(List<ModifierLine> modifierLines)
    {
        foreach (var modifierLine in modifierLines)
        {
            if (!modifierLine.HasValues)
            {
                continue;
            }

            foreach (var modifier in modifierLine.Modifiers)
            {
                if (modifier.ApiId == invariantModifierProvider.ClusterJewelSmallPassiveCountModifierId)
                {
                    return (int)modifierLine.AverageValue;
                }
            }
        }

        return 0;
    }

    public string? ParseGrantTexts(List<ModifierLine> modifierLines)
    {
        foreach (var modifierLine in modifierLines)
        {
            if (!modifierLine.OptionValue.HasValue)
            {
                continue;
            }

            foreach (var modifier in modifierLine.Modifiers)
            {
                if (modifier.ApiId == invariantModifierProvider.ClusterJewelSmallPassiveGrantModifierId)
                {
                    return invariantModifierProvider.ClusterJewelSmallPassiveGrantOptions[modifierLine.OptionValue.Value].Replace("\n", ", ");
                }
            }
        }

        return null;
    }
}
