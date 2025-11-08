namespace Sidekick.Apis.Poe.Items;

public static class ItemClassExtensions
{
    public static bool IsGem(this ItemClass itemClass)
    {
        return itemClass switch
        {
            ItemClass.ActiveGem => true,
            ItemClass.SupportGem => true,
            ItemClass.UncutSkillGem => true,
            ItemClass.UncutSupportGem => true,
            ItemClass.UncutSpiritGem => true,
            _ => false,
        };
    }

    public static bool HasModifiers(this ItemClass itemClass)
    {
        return itemClass switch
        {
            ItemClass.Amulet => true,
            ItemClass.Belt => true,
            ItemClass.Ring => true,
            ItemClass.Trinket => true,
            ItemClass.Jewel => true,
            ItemClass.AbyssJewel => true,

            ItemClass.BodyArmour => true,
            ItemClass.Boots => true,
            ItemClass.Gloves => true,
            ItemClass.Helmet => true,
            ItemClass.Quiver => true,
            ItemClass.Shield => true,
            ItemClass.Focus => true,
            ItemClass.Buckler => true,

            ItemClass.Flask => true,
            ItemClass.LifeFlask => true,
            ItemClass.ManaFlask => true,

            ItemClass.Blueprint => true,
            ItemClass.Contract => true,
            ItemClass.Logbook => true,
            ItemClass.Tablet => true,
            ItemClass.Map => true,
            ItemClass.Waystone => true,
            ItemClass.Barya => true,
            ItemClass.Ultimatum => true,
            ItemClass.Idol => true,

            ItemClass.HeistReward => true,
            ItemClass.HeistUtility => true,
            ItemClass.HeistWeapon => true,
            ItemClass.HeistTool => true,

            ItemClass.Bow => true,
            ItemClass.Crossbow => true,
            ItemClass.Claw => true,
            ItemClass.Dagger => true,
            ItemClass.RuneDagger => true,
            ItemClass.OneHandAxe => true,
            ItemClass.OneHandMace => true,
            ItemClass.OneHandSword => true,
            ItemClass.Sceptre => true,
            ItemClass.Staff => true,
            ItemClass.FishingRod => true,
            ItemClass.TwoHandAxe => true,
            ItemClass.TwoHandMace => true,
            ItemClass.TwoHandSword => true,
            ItemClass.Wand => true,
            ItemClass.Warstaff => true,
            ItemClass.Spear => true,
            ItemClass.Tincture => true,
            ItemClass.SanctumRelic => true,
            ItemClass.SanctumResearch => true,

            ItemClass.Graft => true,
            _ => false,
        };
    }

}
