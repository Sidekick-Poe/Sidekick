using Sidekick.Apis.Poe.Modifiers;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Items.AdditionalInformation;

namespace Sidekick.Apis.Poe.Parser.AdditionalInformation
{
    public class ClusterJewelParser
    {
        private readonly IInvariantModifierProvider invariantModifierProvider;

        public ClusterJewelParser(IInvariantModifierProvider invariantModifierProvider)
        {
            this.invariantModifierProvider = invariantModifierProvider;
        }

        public bool TryParse(Item item, out ClusterJewelInformation? information)
        {
            information = null;

            if (item.Header.Class != Class.Jewel || item.Metadata.Rarity == Rarity.Unique)
            {
                return false;
            }

            var smallPassiveCount = ParseSmallPassiveCount(item);
            if (smallPassiveCount == 0)
            {
                return false;
            }

            var grants = ParseGrantTexts(item);
            if (!grants.Any())
            {
                return false;
            }

            information = new ClusterJewelInformation()
            {
                ItemLevel = item.Properties.ItemLevel,
                SmallPassiveCount = smallPassiveCount,
                GrantTexts = grants,
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

        public List<string> ParseGrantTexts(Item item)
        {
            var grants = new List<string>();

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
                        grants.Add(invariantModifierProvider.ClusterJewelSmallPassiveGrantOptions[modifierLine.OptionValue.Value]);
                    }
                }
            }

            return grants;
        }
    }
}
