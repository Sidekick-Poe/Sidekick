using Sidekick.Common.Enums;

namespace Sidekick.Apis.PoeNinja.Api
{
    public enum ItemType
    {
        [EnumValue("oils")]
        Oil,

        [EnumValue("incubators")]
        Incubator,

        [EnumValue("scarabs")]
        Scarab,

        [EnumValue("fossils")]
        Fossil,

        [EnumValue("resonators")]
        Resonator,

        [EnumValue("essences")]
        Essence,

        [EnumValue("divination-cards")]
        DivinationCard,

        [EnumValue("skill-gems")]
        SkillGem,

        [EnumValue("unique-maps")]
        UniqueMap,

        [EnumValue("maps")]
        Map,

        [EnumValue("cluster-jewels")]
        ClusterJewel,

        [EnumValue("unique-jewels")]
        UniqueJewel,

        [EnumValue("unique-flasks")]
        UniqueFlask,

        [EnumValue("unique-weapons")]
        UniqueWeapon,

        [EnumValue("unique-armours")]
        UniqueArmour,

        [EnumValue("unique-accessories")]
        UniqueAccessory,

        [EnumValue("beasts")]
        Beast,

        [EnumValue("currency")]
        Currency,

        [EnumValue("fragments")]
        Fragment,

        [EnumValue("invitations")]
        Invitation,

        [EnumValue("delirium-orbs")]
        DeliriumOrb,

        [EnumValue("blighted-maps")]
        BlightedMap,

        [EnumValue("blight-ravaged-maps")]
        BlightRavagedMap,

        [EnumValue("artifacts")]
        Artifact,

        [EnumValue("kalguuran-runes")]
        KalguuranRune,

        // BaseType, // This is ~13mb of raw data, in memory it eats ~40mb.
        // HelmetEnchant,
    }
}
