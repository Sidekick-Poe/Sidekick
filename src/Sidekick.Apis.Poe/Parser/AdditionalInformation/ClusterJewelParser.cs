using Sidekick.Apis.Poe.Modifiers;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Items.AdditionalInformation;

namespace Sidekick.Apis.Poe.Parser.AdditionalInformation;

public class ClusterJewelParser(IInvariantModifierProvider invariantModifierProvider)
{
    public bool TryParse(Item item, out ClusterJewelInformation? information)
    {
        information = null;

        if (item.Header.Category != Category.Jewel || item.Header.Rarity == Rarity.Unique)
        {
            return false;
        }

        var smallPassiveCount = ParseSmallPassiveCount(item);
        if (smallPassiveCount == 0)
        {
            return false;
        }

        var grant = ParseGrantTexts(item);
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

    public int ParseSmallPassiveCount(Item item)
    {
        foreach (var modifierLine in item.ModifierLines)
        {
            if (!modifierLine.HasValues)
            {
                continue;
            }

            foreach (var modifier in modifierLine.Modifiers)
            {
                if (modifier.Id == invariantModifierProvider.ClusterJewelSmallPassiveCountModifierId)
                {
                    return (int)modifierLine.Values.First();
                }
            }
        }

        return 0;
    }

    public string? ParseGrantTexts(Item item)
    {
        foreach (var modifierLine in item.ModifierLines)
        {
            if (!modifierLine.OptionValue.HasValue)
            {
                continue;
            }

            foreach (var modifier in modifierLine.Modifiers)
            {
                if (modifier.Id == invariantModifierProvider.ClusterJewelSmallPassiveGrantModifierId)
                {
                    return invariantModifierProvider.ClusterJewelSmallPassiveGrantOptions[modifierLine.OptionValue.Value].Replace("\n", ", ");
                }
            }
        }

        return null;
    }
}
