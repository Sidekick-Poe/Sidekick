using Sidekick.Apis.Poe.Modifiers;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Items.AdditionalInformation;

namespace Sidekick.Apis.Poe.Parser.AdditionalInformation
{
    public class ClusterJewelParser : IAdditionalInformationParser<ClusterJewelInformation>
    {
        private readonly IInvariantModifierProvider invariantModifierProvider;

        public ClusterJewelParser(IInvariantModifierProvider invariantModifierProvider)
        {
            this.invariantModifierProvider = invariantModifierProvider;
        }

        public void Parse(Item item)
        {
            if (item.Metadata.Class != Class.Jewel || item.Metadata.Rarity == Rarity.Unique)
            {
                return;
            }

            var result = new ClusterJewelInformation()
            {
                ItemLevel = item.Properties.ItemLevel,
            };

            foreach (var modifierLine in item.ModifierLines)
            {
                if (!modifierLine.HasValues)
                {
                    continue;
                }

                foreach (var modifier in modifierLine.Modifiers)
                {
                    if (modifier.Id == invariantModifierProvider.ClusterJewelSmallPassiveModifierId)
                    {
                        result.SmallPassiveCount = (int)modifierLine.Values.First();
                    }
                }
            }

            item.AdditionalInformation = result;
        }
    }
}
