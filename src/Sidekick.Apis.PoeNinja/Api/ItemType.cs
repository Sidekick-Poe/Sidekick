namespace Sidekick.Apis.PoeNinja.Api
{
    public enum ItemType
    {
        Oil,
        Incubator,
        Scarab,
        Fossil,
        Resonator,
        Essence,
        DivinationCard,
        SkillGem,
        UniqueMap,
        Map,
        UniqueJewel,
        UniqueFlask,
        UniqueWeapon,
        UniqueArmour,
        UniqueAccessory,
        Beast,
        Currency,
        Fragment,
        Invitation,
        DeliriumOrb,
        BlightedMap,
        BlightRavagedMap,
        Artifact,
        // BaseType, // This is ~13mb of raw data, in memory it eats ~40mb.
        // HelmetEnchant,
    }

    /// <summary>
    /// Poe.ninja uses a different uri for each item type, always in English.
    /// </summary>
    public static class ItemTypeExtensions
    {
        private static Dictionary<ItemType, string> DetailUriSlugs = new()
        {
            { ItemType.Oil, "oils" },
            { ItemType.Incubator, "incubators" },
            { ItemType.Scarab, "scarabs" },
            { ItemType.Fossil, "fossils" },
            { ItemType.Resonator, "resonators" },
            { ItemType.Essence, "essences" },
            { ItemType.DivinationCard, "divination-cards" },
            { ItemType.SkillGem, "skill-gems" },
            { ItemType.UniqueMap, "unique-maps" },
            { ItemType.Map, "maps" },
            { ItemType.UniqueJewel, "unique-jewels" },
            { ItemType.UniqueFlask, "unique-flasks" },
            { ItemType.UniqueWeapon, "unique-weapons" },
            { ItemType.UniqueArmour, "unique-armours" },
            { ItemType.UniqueAccessory, "unique-accessories" },
            { ItemType.Beast, "beasts" },
            { ItemType.Currency, "currency" },
            { ItemType.Fragment, "fragments" },
            { ItemType.Invitation, "invitations" },
            { ItemType.DeliriumOrb, "delirium-orbs" },
            { ItemType.BlightedMap, "blighted-maps" },
            { ItemType.BlightRavagedMap, "blight-ravaged-maps" },
            { ItemType.Artifact, "artifacts" },
        };

        public static string? GetDetailUri(ItemType type)
        {
            if (!DetailUriSlugs.TryGetValue(type, out var value))
            {
                return null;
            }

            return value;
        }
    }
}
