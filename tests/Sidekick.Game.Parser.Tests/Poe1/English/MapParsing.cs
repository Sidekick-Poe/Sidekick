using Sidekick.Game.ItemClasses;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Stats;
using Xunit;
namespace Sidekick.Game.Parser.Tests.Poe1.English;

[Collection(Collections.Poe1EnglishFixture)]
public class MapParsing(Poe1EnglishFixture fixture)
{
    private readonly ItemParser parser = fixture.Parser;

    [Fact]
    public void ArmouredMap()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Magic
Armoured Map (Tier 2)
--------
Item Quantity: +13% (augmented)
Item Rarity: +8% (augmented)
Monster Pack Size: +5% (augmented)
--------
Item Level: 71
--------
Monster Level: 69
--------
+20% Monster Physical Damage Reduction
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.

");

        Assert.Equal(ItemClass.Map, actual.ItemClass.Type);
        Assert.Equal(Rarity.Magic, actual.Properties.Rarity);
        Assert.Null(actual.TradeItem?.Name);
        Assert.Equal("Map", actual.TradeItem?.Type);
        Assert.Equal(2, actual.Properties.MapTier);

        fixture.AssertHasStat(actual, StatCategory.Explicit, "+#% Monster Physical Damage Reduction", 20);
    }

    [Fact]
    public void OlmecSanctum()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Unique
Olmec's Sanctum
Map (Tier 6)
--------
Item Quantity: +179% (augmented)
--------
Item Level: 73
--------
Monster Level: 73
--------
42% more Monster Life
37% increased Monster Damage
Final Boss drops higher Level Items
--------
They flew, and leapt, and clambered over,
They crawled, and swam, and slithered under.
Still its ancient secrets await unclaimed
And of this hidden temple, only legends remain.
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
");

        Assert.Equal(ItemClass.Map, actual.ItemClass.Type);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Olmec's Sanctum", actual.TradeItem?.Name);
        Assert.Equal("Map", actual.TradeItem?.Type);
        Assert.Equal(6, actual.Properties.MapTier);

        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% more Monster Life", 42);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Monster Damage", 37);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "Final Boss drops higher Level Items");
    }

    [Fact]
    public void MalevolentSecrets()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Rare
Malevolent Secrets
Map (Tier 16)
--------
Item Quantity: +101% (augmented)
Item Rarity: +62% (augmented)
Monster Pack Size: +39% (augmented)
More Maps: +110% (augmented)
More Threads: +35% (augmented)
More Currency: +45% (augmented)
--------
Item Level: 85
--------
Monster Level: 83
--------
Area is Influenced by the Originator's Memories (implicit)
--------
+40% Monster Physical Damage Reduction
Monsters have +1 to Maximum Endurance Charges
Area has patches of Consecrated Ground
Monsters gain an Endurance Charge when hit
Monsters inflict 2 Grasping Vines on Hit
Monsters gain 73% of Maximum Life as Extra Maximum Energy Shield
Monsters have +60% chance to Suppress Spell Damage
Players are targeted by a Meteor when they use a Flask
Players have 27% less Defences
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
--------
Corrupted
");

        Assert.Equal(ItemClass.Map, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Null(actual.TradeItem?.Name);
        Assert.Equal("Map", actual.TradeItem?.Type);
        Assert.Equal(16, actual.Properties.MapTier);
        Assert.Equal(101, actual.Properties.ItemQuantity);
        Assert.Equal(62, actual.Properties.ItemRarity);
        Assert.Equal(39, actual.Properties.MonsterPackSize);
        Assert.Equal(110, actual.Properties.MoreMaps);
        Assert.Equal(35, actual.Properties.MoreThreads);
        Assert.Equal(45, actual.Properties.MoreCurrency);
        Assert.True(actual.Properties.Corrupted);
    }

    [Fact]
    public void MapQualityCurrency()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Normal
Map (Tier 16)
--------
More Currency: +50% (augmented)
Quality (Currency): +20% (augmented)
--------
Item Level: 83
--------
Monster Level: 83
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
");

        Assert.Equal(ItemClass.Map, actual.ItemClass.Type);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Null(actual.TradeItem?.Name);
        Assert.Equal("Map", actual.TradeItem?.Type);
        Assert.Equal(16, actual.Properties.MapTier);
        Assert.Equal(16, actual.Properties.MapTier);
        Assert.Equal(50, actual.Properties.MoreCurrency);
        Assert.Equal(20, actual.Properties.QualityCurrency);
    }

    [Fact]
    public void MapQualityScarabs()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Normal
Map (Tier 16)
--------
More Threads: +50% (augmented)
Quality (Scarabs): +20% (augmented)
--------
Item Level: 83
--------
Monster Level: 83
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
");

        Assert.Equal(ItemClass.Map, actual.ItemClass.Type);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Null(actual.TradeItem?.Name);
        Assert.Equal("Map", actual.TradeItem?.Type);
        Assert.Equal(16, actual.Properties.MapTier);
        Assert.Equal(16, actual.Properties.MapTier);
        Assert.Equal(50, actual.Properties.MoreThreads);
        Assert.Equal(20, actual.Properties.QualityScarabs);
    }

    [Fact]
    public void MapQualityCards()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Normal
Map (Tier 16)
--------
More Divination Cards: +50% (augmented)
Quality (Divination Cards): +20% (augmented)
--------
Item Level: 83
--------
Monster Level: 83
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
");

        Assert.Equal(ItemClass.Map, actual.ItemClass.Type);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Null(actual.TradeItem?.Name);
        Assert.Equal("Map", actual.TradeItem?.Type);
        Assert.Equal(16, actual.Properties.MapTier);
        Assert.Equal(16, actual.Properties.MapTier);
        Assert.Equal(50, actual.Properties.MoreCards);
        Assert.Equal(20, actual.Properties.QualityCards);
    }

    [Fact]
    public void MapQualityPackSize()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Normal
Map (Tier 16)
--------
Monster Pack Size: +10% (augmented)
Quality (Pack Size): +20% (augmented)
--------
Item Level: 83
--------
Monster Level: 83
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
");

        Assert.Equal(ItemClass.Map, actual.ItemClass.Type);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Null(actual.TradeItem?.Name);
        Assert.Equal("Map", actual.TradeItem?.Type);
        Assert.Equal(16, actual.Properties.MapTier);
        Assert.Equal(16, actual.Properties.MapTier);
        Assert.Equal(10, actual.Properties.MonsterPackSize);
        Assert.Equal(20, actual.Properties.QualityPackSize);
    }

    [Fact]
    public void MapQualityRarity()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Normal
Map (Tier 16)
--------
Item Rarity: +40% (augmented)
Quality (Rarity): +20% (augmented)
--------
Item Level: 83
--------
Monster Level: 83
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
");

        Assert.Equal(ItemClass.Map, actual.ItemClass.Type);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Null(actual.TradeItem?.Name);
        Assert.Equal("Map", actual.TradeItem?.Type);
        Assert.Equal(16, actual.Properties.MapTier);
        Assert.Equal(16, actual.Properties.MapTier);
        Assert.Equal(40, actual.Properties.ItemRarity);
        Assert.Equal(20, actual.Properties.QualityRarity);
    }

    [Fact]
    public void TerrorGrind()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Rare
Terror Grind
Nightmare Map
--------
Item Quantity: +78% (augmented)
Item Rarity: +45% (augmented)
Monster Pack Size: +41% (augmented)
More Currency: +47% (augmented)
--------
Item Level: 83
--------
Monster Level: 83
--------
93% more Monster Life
Monsters reflect 18% of Physical Damage
Monsters gain a Power Charge on Hit
Monsters take 36% reduced Extra Damage from Critical Strikes
Monsters' Attacks have 60% chance to Impale on Hit
Rare monsters in area Temporarily Revive on death
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
--------
Modifiable only with Chaos Orbs, Vaal Orbs, Delirium Orbs and Chisels
");

        Assert.Equal(ItemClass.Map, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Null(actual.TradeItem?.Name);
        Assert.Equal("Nightmare Map", actual.TradeItem?.Type);
        Assert.Equal(78, actual.Properties.ItemQuantity);
        Assert.Equal(45, actual.Properties.ItemRarity);
        Assert.Equal(41, actual.Properties.MonsterPackSize);
        Assert.Equal(47, actual.Properties.MoreCurrency);

        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% more Monster Life", 93);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "Monsters reflect #% of Physical Damage", 18);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "Monsters have #% chance to gain a Power Charge on Hit");
        fixture.AssertHasStat(actual, StatCategory.Explicit, "Monsters take #% reduced Extra Damage from Critical Strikes", 36);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "Monsters' Attacks have #% chance to Impale on Hit", 60);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "Rare monsters in area Temporarily Revive on death");
    }

    [Fact]
    public void FoePortent()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Rare
Foe Portent
Map (Tier 16)
--------
Item Quantity: +113% (augmented)
Item Rarity: +117% (augmented)
Monster Pack Size: +72% (augmented)
More Maps: +35% (augmented)
More Currency: +64% (augmented)
--------
Item Level: 85
--------
Monster Level: 83
--------
Area is Influenced by the Originator's Memories (implicit)
--------
Area has patches of Chilled Ground
Monsters have 100% increased Area of Effect
34% increased Monster Damage
+50% Monster Physical Damage Reduction
Monsters gain an Endurance Charge on Hit
Debuffs on Monsters expire 100% faster
+35% Monster Chaos Resistance
+55% Monster Elemental Resistances
Rare monsters in area are Shaper-Touched
Area contains Drowning Orbs
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
--------
Corrupted
");

        Assert.Equal(ItemClass.Map, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Null(actual.TradeItem?.Name);
        Assert.Equal("Map", actual.TradeItem?.Type);
        Assert.Equal(16, actual.Properties.MapTier);
        Assert.Equal(113, actual.Properties.ItemQuantity);
        Assert.Equal(117, actual.Properties.ItemRarity);
        Assert.Equal(72, actual.Properties.MonsterPackSize);
        Assert.Equal(35, actual.Properties.MoreMaps);
        Assert.Equal(64, actual.Properties.MoreCurrency);

        fixture.AssertHasStat(actual, StatCategory.Implicit, "Area is Influenced by the Originator's Memories");
    }

    [Fact]
    public void FilthyAbode()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Rare
Filthy Abode
Valdo Map
--------
Map Area: Arsenal
Reward: Foil The Dancing Dervish
Item Rarity: +50% (augmented)
Monster Pack Size: +26% (augmented)
--------
Chance for dropped Maps to convert to:
Scarab: 18% (augmented)
--------
Item Level: 100
--------
Monster Level: 84
--------
Slaying Enemies in a kill streak grants Rampage bonuses
All Strongboxes in Area must be opened to claim Reward
Area contains 12 additional packs of Untainted Wild Animals
Players gain Rare Monster Modifiers for 20 seconds on Kill
Rare Monsters each have 3 additional Modifiers
Area contains 10 additional guarded Exquisite Vaal Vessels
Area contains enemies of all influence types
--------
Travel to this Map by using it in a personal Map Device. Maps can only be used once. Defeat 90% of all monsters in this Map, including all Rare and Unique enemies to obtain the Reward. The area created is not affected by your Atlas Passive Tree, and cannot be augmented via the Map Device.
--------
Unmodifiable
--------
Foil (Celestial Amethyst)
");

        Assert.Equal(ItemClass.Map, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Null(actual.TradeItem?.Name);
        Assert.Equal("Valdo Map", actual.TradeItem?.Type);
        Assert.Equal("Foil The Dancing Dervish", actual.Properties.Reward);
        Assert.Equal(50, actual.Properties.ItemRarity);
        Assert.Equal(26, actual.Properties.MonsterPackSize);
    }

    [Fact]
    public void BlightedMapTier13()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Normal
Blighted Map (Tier 13)
--------
Map Area: Necropolis
--------
Item Level: 83
--------
Monster Level: 80
--------
Area is infested with Fungal Growths (implicit)
Map's Item Quantity Modifiers also affect Blight Chest count at 25% value (implicit)
Can be Anointed up to 3 times (implicit)
Natural inhabitants of this area have been removed (implicit)
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
");

        Assert.Equal(ItemClass.Map, actual.ItemClass.Type);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Null(actual.TradeItem?.Name);
        Assert.Equal("Map", actual.TradeItem?.Type);
        Assert.True(actual.Properties.Blighted);
        Assert.False(actual.Properties.BlightRavaged);
        Assert.Equal(13, actual.Properties.MapTier);
    }

    [Fact]
    public void BlightedMapTier3Map()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Normal
Blighted Map (Tier 3)
--------
Map Area: Tropical Island
--------
Item Level: 79
--------
Monster Level: 70
--------
Area is infested with Fungal Growths (implicit)
Map's Item Quantity Modifiers also affect Blight Chest count at 25% value (implicit)
Can be Anointed up to 3 times (implicit)
Natural inhabitants of this area have been removed (implicit)
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
--------
Note: ~b/o 1 chaos
");

        Assert.Equal(ItemClass.Map, actual.ItemClass.Type);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Null(actual.TradeItem?.Name);
        Assert.Equal("Map", actual.TradeItem?.Type);
        Assert.True(actual.Properties.Blighted);
        Assert.False(actual.Properties.BlightRavaged);
        Assert.Equal(3, actual.Properties.MapTier);
    }

    [Fact]
    public void BlightRavagedMapTier16()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Normal
Blight-ravaged Map (Tier 16)
--------
Map Area: Foundry
--------
Item Level: 82
--------
Monster Level: 85
--------
{ Implicit Modifier }
Monster Level: 85
200% more Monster Life
20% increased Monster Movement Speed
Area is infested with Fungal Growths
Map's Item Quantity Modifiers also affect Blight Chest count at 50% value
Can be Anointed up to 9 times
Natural inhabitants of this area have been removed
--------
Travel to a Map of this tier or lower by using this in a personal Map Device. Maps can only be used once.
");

        Assert.Equal(ItemClass.Map, actual.ItemClass.Type);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Null(actual.TradeItem?.Name);
        Assert.Equal("Map", actual.TradeItem?.Type);
        Assert.False(actual.Properties.Blighted);
        Assert.True(actual.Properties.BlightRavaged);
        Assert.Equal(16, actual.Properties.MapTier);
    }

    [Fact]
    public void TortureDirection()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Rare
Torture Direction
Map (Tier 6)
--------
Atlas Drops: +52% (augmented)
Item Rarity: +31% (augmented)
Monster Pack Size: +20% (augmented)
--------
Item Level: 74
--------
Monster Level: 73
--------
{ Prefix Modifier ""Antagonist's"" (Tier: 1) }
26(20-30)% increased number of Rare Monsters
{ Prefix Modifier ""Overlord's"" (Tier: 1) — Attack, Caster, Speed }
Unique Boss deals 20% increased Damage
Unique Boss has 25% increased Attack and Cast Speed
{ Rare-only (Tier: 1) }
Area contains a buried Stalker Sentinel — Unscalable Value
{ Suffix Modifier ""of Transience"" (Tier: 1) }
Buffs on Players expire 50% faster
--------
Travel to a Map of this tier or lower by using this in a personal Map Device. Maps can only be used once.
");

        Assert.Equal(ItemClass.Map, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Null(actual.TradeItem?.Name);
        Assert.Equal("Map", actual.TradeItem?.Type);
        Assert.Equal(6, actual.Properties.MapTier);

        fixture.AssertHasStat(actual, StatCategory.RareOnly, "Area contains a buried Stalker Sentinel");
    }

    [Fact]
    public void PerniciousSolitude()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Rare
Pernicious Solitude
Map (Tier 16)
--------
Atlas Drops: +75% (augmented)
Item Rarity: +45% (augmented)
Monster Pack Size: +29% (augmented)
--------
Item Level: 83
--------
Monster Level: 83
--------
{ Implicit Modifier }
Map contains Drox's Citadel(...) — Unscalable Value
--------
{ Prefix Modifier ""Feral"" (Tier: 1) }
Area is inhabited by Animals — Unscalable Value
{ Prefix Modifier ""Fecund"" (Tier: 1) — Life }
41(40-49)% more Monster Life
{ Prefix Modifier ""Unstoppable"" (Tier: 1) — Speed }
Monsters cannot be Taunted — Unscalable Value
Monsters' Action Speed cannot be modified to below Base Value
Monsters' Movement Speed cannot be modified to below Base Value — Unscalable Value
{ Suffix Modifier ""of Fatigue"" (Tier: 1) }
Players have 40% less Cooldown Recovery Rate
{ Suffix Modifier ""of Rust"" (Tier: 1) — Defences, Armour }
Players have 30% less Armour
Players have 40% reduced Chance to Block
{ Suffix Modifier ""of Insulation"" (Tier: 1) — Elemental, Fire, Cold, Lightning, Ailment }
Monsters have 70% chance to Avoid Elemental Ailments
(Elemental Ailments are Ignited, Scorched, Chilled, Frozen, Brittle, Shocked, and Sapped)
--------
Travel to a Map of this tier or lower by using this in a personal Map Device. Maps can only be used once.
");

        Assert.Equal(ItemClass.Map, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Null(actual.TradeItem?.Name);
        Assert.Equal("Map", actual.TradeItem?.Type);
        Assert.Equal(16, actual.Properties.MapTier);

        fixture.AssertHasStat(actual, StatCategory.Implicit, "Map contains Drox's Citadel");
    }

    [Fact]
    public void HiddenRealm()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Rare
Hidden Realm
Map (Tier 16)
--------
Item Quantity: +116% (augmented)
Item Rarity: +68% (augmented)
Monster Pack Size: +44% (augmented)
--------
Item Level: 84
--------
Monster Level: 83
--------
Monsters have 363% increased Critical Strike Chance
+45% to Monster Critical Strike Multiplier
Monsters fire 2 additional Projectiles
Monsters deal 104% extra Physical Damage as Cold
Monsters deal 100% extra Physical Damage as Lightning
+25% Monster Chaos Resistance
+40% Monster Elemental Resistances
Monsters Blind on Hit
Players have 60% reduced effect of Non-Curse Auras from Skills
Players have 25% less Area of Effect
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
--------
Corrupted
");

        Assert.Equal(ItemClass.Map, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Null(actual.TradeItem?.Name);
        Assert.Equal("Map", actual.TradeItem?.Type);
        Assert.Equal(16, actual.Properties.MapTier);
        actual.AssertHasNinjaStash("map-tier-16-t0-gen-24");
    }

    [Fact]
    public void CruelStone()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Rare
Cruel Stone
Nightmare Map
--------
Item Quantity: +110% (augmented)
Item Rarity: +111% (augmented)
Monster Pack Size: +57% (augmented)
More Maps: +35% (augmented)
More Currency: +47% (augmented)
--------
Item Level: 83
--------
Monster Level: 83
--------
Players are Cursed with Vulnerability
126% more Monster Life
33% increased Monster Damage
Monsters cannot be Stunned
Monsters have +1 to Maximum Frenzy Charges
Monsters gain a Frenzy Charge on Hit
Monsters cannot be Taunted
Monsters' Action Speed cannot be modified to below Base Value
Monsters' Movement Speed cannot be modified to below Base Value
Players cannot Regenerate Life, Mana or Energy Shield
The Maven interferes with Players
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
--------
Corrupted
--------
Modifiable only with Chaos Orbs, Vaal Orbs, Delirium Orbs and Chisels
");

        Assert.Equal(ItemClass.Map, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Null(actual.TradeItem?.Name);
        Assert.Equal("Nightmare Map", actual.TradeItem?.Type);
        actual.AssertHasNinjaStash("nightmare-map-t0-gen-24");
    }

    [Fact]
    public void BlightRavagedMapRelicChambers()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Normal
Blight-ravaged Map (Tier 16)
--------
Map Area: Relic Chambers
--------
Item Level: 83
--------
Monster Level: 85
--------
Monster Level: 85 (implicit)
200% more Monster Life (implicit)
20% increased Monster Movement Speed (implicit)
Area is infested with Fungal Growths (implicit)
Map's Item Quantity Modifiers also affect Blight Chest count at 50% value (implicit)
Can be Anointed up to 9 times (implicit)
Natural inhabitants of this area have been removed (implicit)
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
");

        Assert.Equal(ItemClass.Map, actual.ItemClass.Type);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Null(actual.TradeItem?.Name);
        Assert.Equal("Map", actual.TradeItem?.Type);
        Assert.False(actual.Properties.Blighted);
        Assert.True(actual.Properties.BlightRavaged);
        Assert.Equal(16, actual.Properties.MapTier);
        actual.AssertHasNinjaStash("blight-ravaged-map-tier-16-t0-gen-24");
    }

    [Fact]
    public void BloodBearings()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Rare
Blood Bearings
Map (Tier 16)
--------
Item Quantity: +68% (augmented)
Item Rarity: +41% (augmented)
Monster Pack Size: +26% (augmented)
--------
Item Level: 83
--------
Monster Level: 83
--------
Map contains Drox's Citadel (implicit)
Item Quantity increases amount of Rewards Drox drops by 20% of its value (implicit)
--------
23% increased number of Rare Monsters
Players have -10% to all maximum Resistances
Monsters have a 20% chance to Ignite, Freeze and Shock on Hit
Monsters have +60% chance to Suppress Spell Damage
Monsters steal Power, Frenzy and Endurance charges on Hit
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
");

        Assert.Equal(ItemClass.Map, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Null(actual.TradeItem?.Name);
        Assert.Equal("Map", actual.TradeItem?.Type);
        Assert.Equal(16, actual.Properties.MapTier);
        actual.AssertHasNinjaStash("drox-map-tier-16-t0-gen-24");
    }

    [Fact]
    public void ArcaneArtifice()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Rare
Arcane Artifice
Shaper Guardian Map
--------
Item Quantity: +104% (augmented)
Item Rarity: +61% (augmented)
Monster Pack Size: +40% (augmented)
--------
Item Level: 83
--------
Monster Level: 83
--------
Area is influenced by The Shaper (implicit)
--------
Players are Cursed with Enfeeble
Players are Cursed with Elemental Weakness
Monsters deal 102% extra Physical Damage as Fire
Monsters deal 106% extra Physical Damage as Cold
All Monster Damage from Hits always Ignites
Monsters Maim on Hit with Attacks
Monsters gain 32% of their Physical Damage as Extra Chaos Damage
Monsters Hinder on Hit with Spells
Monsters Inflict Withered for 2 seconds on Hit
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
--------
Corrupted
");

        Assert.Equal(ItemClass.Map, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Null(actual.TradeItem?.Name);
        Assert.Equal("Shaper Guardian Map", actual.TradeItem?.Type);
        actual.AssertHasNinjaStash("shaper-guardian-map-t0-gen-24");
    }

    [Fact]
    public void PoorjoyAsylum()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Unique
Poorjoy's Asylum
Map (Tier 16)
--------
Item Quantity: +100% (augmented)
Item Rarity: +250% (augmented)
--------
Item Level: 85
--------
Monster Level: 83
--------
150% increased Experience gain
149% more Monster Life
144% increased Monster Damage
25% increased Monster Movement Speed
25% increased Monster Attack Speed
25% increased Monster Cast Speed
Area is a large Maze
Unique Boss drops 13 additional Rare Two Hand Swords
--------
Laughs of all the twisted
Echo through these halls of gold.
In this cold forgotten void
They wander, forever uncontrolled.
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
");

        Assert.Equal(ItemClass.Map, actual.ItemClass.Type);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Poorjoy's Asylum", actual.TradeItem?.Name);
        Assert.Equal("Map", actual.TradeItem?.Type);
        actual.AssertHasNinjaStash("poorjoys-asylum-t0");
    }

    [Fact]
    public void DivineVessel()
    {
        var actual = parser.ParseItem(@"Item Class: Map Fragments
Rarity: Normal
Divine Vessel
--------
Unique Boss deals 10% increased Damage
Unique Boss has 10% increased Attack and Cast Speed
Unique Boss has 10% increased Life
Unique Boss has 20% increased Area of Effect
--------
Power is a curious thing. 
It can be contained, hidden, locked away, 
and yet it always breaks free.
--------
Can be used in a personal Map Device, allowing you to capture the Soul of the Map's Boss. The Vessel containing the captured Soul can be retrieved from the Map Device. You must be in the Map when the boss is defeated.
--------
Note: ~price 1 chaos
");

        Assert.Equal(ItemClass.MapFragments, actual.ItemClass.Type);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Equal("Divine Vessel", actual.TradeItem?.Type);
    }

    [Fact]
    public async Task ChronicleofAtzoatl()
    {
        var actual = parser.ParseItem(@"Item Class: Misc Map Items
Rarity: Currency
Chronicle of Atzoatl
--------
Area Level: 81
--------
Open Rooms:
Sparring Room (Tier 1)
Glittering Halls (Tier 3)
Locus of Corruption (Tier 3)
Demolition Lab (Tier 2)
Temple Nexus (Tier 3)
Apex of Ascension (Tier 3)
Tempest Generator (Tier 1)
Automaton Lab (Tier 2)
Cultivar Chamber (Tier 2)
Pools of Restoration (Tier 1)
Obstructed Rooms:
Banquet Hall
Apex of Atzoatl
--------
""Atzoatl was a locus of Corruption, a temple dedicated to the worship of the unspeakable."" - Icius Perandus, Antiquities Collection, Eroded Vaal Orb
--------
Can be used in a personal Map Device to open portals to the Temple of Atzoatl in the present day.
--------
Note: ~price 1.29 exalted
");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Chronicle of Atzoatl", actual.TradeItem?.Type);
        Assert.Equal(81, actual.Properties.AreaLevel);

        fixture.AssertHasStat(actual, StatCategory.Pseudo, "Has Room: Locus of Corruption (Tier 3)", "Open Room");
        fixture.AssertDoesNotHaveStat(actual, StatCategory.Pseudo, "Has Room: Locus of Corruption (Tier 3)", "Closed Room");

        var filters = await fixture.GetStatFilters(actual);
        Assert.Equal(10, filters.Count);
        var index = -1;
        Assert.IsType<StatFilter>(filters[++index]);
        Assert.IsType<StatFilter>(filters[++index]);
        Assert.IsType<StatFilter>(filters[++index]);
        Assert.IsType<StatFilter>(filters[++index]);
        Assert.IsType<StatFilter>(filters[++index]);
        Assert.IsType<StatFilter>(filters[++index]);
        Assert.IsType<StatFilter>(filters[++index]);
        Assert.IsType<StatFilter>(filters[++index]);
        Assert.IsType<StatFilter>(filters[++index]);
        Assert.IsType<StatFilter>(filters[++index]);

        Assert.Contains(filters, x => x.Text == "Locus of Corruption (Tier 3)");
        Assert.DoesNotContain(filters, x => x.Text == "Apex of Atzoatl");
    }

    [Fact]
    public void Curiosity()
    {
        var actual = parser.ParseItem(@"Item Class: Pieces
Rarity: Unique
Curiosity
Vaal Aspect
--------
Item Level: 84
--------
--------
Delving into the mysteries of existence, they deciphered
the secret laws of nature... and then, harnessed them.
--------
This item will transform when correctly arranged with other items.
");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Curiosity", actual.TradeItem?.Name);
        Assert.Equal("Vaal Aspect", actual.TradeItem?.Type);
        Assert.Equal(84, actual.Properties.ItemLevel);
    }

    [Fact]
    public async Task ChronicleofAtzoatlRoomFilters()
    {
        var item = parser.ParseItem(@"Item Class: Misc Map Items
Rarity: Currency
Chronicle of Atzoatl
--------
Area Level: 72
--------
Open Rooms:
Hall of Mettle (Tier 1)
Doryani's Institute (Tier 3)
Jeweller's Workshop (Tier 1)
Toxic Grove (Tier 3)
Chasm
Factory (Tier 3)
Locus of Corruption (Tier 3)
Storage Room (Tier 1)
Pools of Restoration (Tier 1)
Tempest Generator (Tier 1)
Shrine of Unmaking (Tier 3)
Apex of Atzoatl
--------
""The Vaal had little concern for the externalities of their engineering. The toxins produced by their manufacturing may have poisoned the entirety of Atzoatl."" - Icius Perandus, Antiquities Collection, Desiccated Corpse of a Vaal Citizen
--------
Right click this item to view the Temple of Atzoatl.
Can be used in a personal Map Device to open portals to the Temple of Atzoatl in the present day.
");

        var filters = await fixture.StatParser.GetFilters(item);
        var expandableFilter = Assert.IsType<ExpandableFilter>(Assert.Single(filters));
        var statFilters = expandableFilter.Filters.OfType<StatFilter>().ToList();
        Assert.Equal(12, statFilters.Count);
        Assert.Contains(statFilters, x => x.Text == "Doryani's Institute (Tier 3)");
        Assert.Contains(statFilters, x => x.Text == "Apex of Atzoatl");
    }

    [Fact]
    public void TributeToTheGoddess()
    {
        var actual = parser.ParseItem(@"Item Class: Map Fragments
Rarity: Normal
Tribute to the Goddess
--------
The Labyrinth's rewards have been enriched.
--------
You may appeal to the Goddess for another verdict,
but justice favours only the truly worthy.
--------
Travel to the Aspirants' Plaza and spend this item to open the Eternal Labyrinth of Fortune.
");

        Assert.Equal(ItemClass.MapFragments, actual.ItemClass.Type);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Equal("Tribute to the Goddess", actual.TradeItem?.Type);
    }

    [Fact]
    public void UltimatumThread()
    {
        var actual = parser.ParseItem(@"Item Class: Map Fragments
Rarity: Normal
Ultimatum Thread
--------
Stack Size: 34/100
--------
Threads an Ultimatum Encounter into an Area
--------
Can be used in a personal Map Device to add content to a Map.
");

        Assert.Equal(ItemClass.MapFragments, actual.ItemClass.Type);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Equal("Ultimatum Thread", actual.TradeItem?.Type);
        actual.AssertHasNinjaExchange("ultimatum-thread");
    }

    [Fact]
    public void RustedReliquaryScarab()
    {
        var actual = parser.ParseItem(@"Item Class: Map Fragments
Rarity: Normal
Rusted Reliquary Scarab
--------
50% more Unique Items found in Area
--------
The Maraketh left you to die alone in the desert, young Sumei, but we
saw the potential in you. The Order of the Djinn is your akhara now.
--------
Can be used in a personal Map Device to add modifiers to a Map.
--------
Note: ~b/o .50 chaos
");

        Assert.Equal(ItemClass.MapFragments, actual.ItemClass.Type);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Equal("Rusted Reliquary Scarab", actual.TradeItem?.Type);
    }

    [Fact]
    public void ChronicleofAtzoatlMap()
    {
        var actual = parser.ParseItem(@"Item Class: Misc Map Items
Rarity: Currency
Chronicle of Atzoatl
--------
Area Level: 81
--------
Open Rooms:
Sparring Room (Tier 1)
Glittering Halls (Tier 3)
Locus of Corruption (Tier 3)
Demolition Lab (Tier 2)
Temple Nexus (Tier 3)
Apex of Ascension (Tier 3)
Tempest Generator (Tier 1)
Automaton Lab (Tier 2)
Cultivar Chamber (Tier 2)
Pools of Restoration (Tier 1)
Obstructed Rooms:
Banquet Hall
Apex of Atzoatl
--------
""Atzoatl was a locus of Corruption, a temple dedicated to the worship of the unspeakable."" - Icius Perandus, Antiquities Collection, Eroded Vaal Orb
--------
Can be used in a personal Map Device to open portals to the Temple of Atzoatl in the present day.
--------
Note: ~price 1.29 exalted
");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Null(actual.TradeItem?.Name);
        Assert.Equal("Chronicle of Atzoatl", actual.TradeItem?.Type);
        Assert.Equal(81, actual.Properties.AreaLevel);

        fixture.AssertHasStat(actual, StatCategory.Pseudo, "Has Room: Locus of Corruption (Tier 3)");
    }

    [Fact]
    public void IncandescentInvitation()
    {
        var actual = parser.ParseItem(@"Item Class: Misc Map Items
Rarity: Normal
Incandescent Invitation
--------
Item Level: 83
--------
Modifiers to Item Quantity affect the amount of rewards dropped by the boss (implicit)
--------
From the corona of an ancient star,
the Searing Exarch seeks to master the Atlas.
--------
Open portals to Absence of Patience and Wisdom by using this item in a personal Map Device. While viewing your Atlas, Right-click this item to pinpoint its location.
");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Equal("Incandescent Invitation", actual.TradeItem?.Type);
        actual.AssertHasNinjaStash("incandescent-invitation");
    }

    [Fact]
    public void ScreamingInvitation()
    {
        var actual = parser.ParseItem(@"Item Class: Misc Map Items
Rarity: Normal
Screaming Invitation
--------
Item Level: 83
--------
{ Implicit Modifier }
Modifiers to Item Quantity affect the amount of rewards dropped by the boss
--------
From the heart of the Tangle, the Eater of Worlds
reaches out for control of the Atlas.
--------
Open portals to Absence of Symmetry and Harmony by using this item in a personal Map Device. While viewing your Atlas, Right-click this item to pinpoint its location.
--------
Note: ~b/o 112 chaos
");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Equal("Screaming Invitation", actual.TradeItem?.Type);
        actual.AssertHasNinjaStash("screaming-invitation");
    }

}
