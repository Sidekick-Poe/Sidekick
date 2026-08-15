
namespace Sidekick.Game.Languages.Implementations;

public class GameLanguageDe : IGameLanguage
{
    public string Code => "de";
    public string Label => "German";

    public string PoeTradeBaseUrl => "https://de.pathofexile.com/trade/";
    public string PoeTradeApiBaseUrl => "https://de.pathofexile.com/api/trade/";
    public string Poe2TradeBaseUrl => "https://de.pathofexile.com/trade2/";
    public string Poe2TradeApiBaseUrl => "https://de.pathofexile.com/api/trade2/";

    public string RarityUnique => "Einzigartig";
    public string RarityRare => "Selten";
    public string RarityMagic => "Magisch";
    public string RarityNormal => "Normal";
    public string RarityCurrency => "Währung";
    public string RarityGem => "Gemme";
    public string RarityDivinationCard => "Weissagungskarte";

    public string DescriptionRarity => "Seltenheit";
    public string DescriptionUnidentified => "Nicht identifiziert";
    public string DescriptionQuality => "Qualität";
    public string DescriptionLevel => "Stufe";
    public string DescriptionCorrupted => "Verderbt";
    public string DescriptionSplit => "Geteilt";
    public string DescriptionMirrored => "Gespiegelt";
    public string DescriptionSockets => "Fassungen";
    public string DescriptionItemLevel => "Gegenstandsstufe";
    public string DescriptionExperience => "Erfahrung";
    public string DescriptionPhysicalDamage => "Physischer Schaden";
    public string DescriptionElementalDamage => "Elementarschaden";
    public string DescriptionFireDamage => "Feuerschaden";
    public string DescriptionColdDamage => "Kälteschaden";
    public string DescriptionLightningDamage => "Blitzschaden";
    public string DescriptionChaosDamage => "Chaosschaden";
    public string DescriptionEnergyShield => "Energieschild";
    public string DescriptionEnergyShieldAlternate => "";
    public string DescriptionArmour => "Rüstung";
    public string DescriptionEvasion => "Ausweichwert";
    public string DescriptionChanceToBlock => "Chance auf Blocken";
    public string DescriptionBlockChance => "Blockchance";
    public string DescriptionSpirit => "Wille";
    public string DescriptionAttacksPerSecond => "Angriffe pro Sekunde";
    public string DescriptionCriticalStrikeChance => "Kritische Trefferchance";
    public string DescriptionCriticalHitChance => "Kritische Trefferchance";
    public string DescriptionMapTier => "Kartenlevel";
    public string DescriptionReward => "Belohnung";
    public string DescriptionItemQuantity => "Gegenstandsmenge";
    public string DescriptionItemRarity => "Gegenstandsseltenheit";
    public string DescriptionMonsterPackSize => "Monstergruppengröße";
    public string DescriptionMoreMaps => "Mehr Karten";
    public string DescriptionMoreScarabs => "Mehr Skarabäen";
    public string DescriptionMoreCurrency => "Mehr Währung";
    public string DescriptionMoreCards => "Mehr Weissagungskarten";
    public string DescriptionQualityCurrency => "Qualität (Währung)";
    public string DescriptionQualityScarabs => "Qualität (Skarabäen)";
    public string DescriptionQualityCards => "Qualität (Weissagungskarten)";
    public string DescriptionQualityPackSize => "Qualität (Gruppengröße)";
    public string DescriptionQualityRarity => "Qualität (Seltenheit)";
    public string DescriptionMagicMonsters => "Magische Monster";
    public string DescriptionRareMonsters => "Seltene Monster";
    public string DescriptionRevivesAvailable => "Wiederbelebungen verfügbar";
    public string DescriptionWaystoneDropChance => "Chance auf fallen gelassene Wegsteine";
    public string DescriptionAreaLevel => "Gebietsstufe";
    public string DescriptionMemoryStrands => "Erinnerungsstränge";
    public string DescriptionUnusable => "Du kannst diesen Gegenstand nicht benutzen. Seine Eigenschaften werden ignoriert.";
    public string DescriptionRequirements => "Anforderungen";
    public string DescriptionRequires => "Erfordert";
    public string DescriptionRequiresLevel => "Stufe";
    public string DescriptionRequiresStr => "Str";
    public string DescriptionRequiresDex => "Ges";
    public string DescriptionRequiresInt => "Int";
    public string DescriptionHeistWings => "Aufgedeckte Gebäudetrakte";
    public string DescriptionHeistRoutes => "Aufgedeckte Fluchtwege";
    public string DescriptionHeistRooms => "Aufgedeckte Belohnungsräume";
    public string DescriptionHeistLockpicking => "Erfordert Schlossknacken (Stufe #)";
    public string DescriptionHeistDemolition => "Erfordert Sprengung (Stufe #)";
    public string DescriptionHeistAgility => "Erfordert Agilität (Stufe #)";
    public string DescriptionHeistCounterThaumaturgy => "Erfordert Thaumaturgie-Abwehr (Stufe #)";
    public string DescriptionHeistTrap => "Erfordert Fallenentschärfung (Stufe #)";
    public string DescriptionHeistPerception => "Erfordert Wahrnehmung (Stufe #)";
    public string DescriptionHeistBruteForce => "Erfordert Brachialgewalt (Stufe #)";
    public string DescriptionHeistDeception => "Erfordert Täuschung (Stufe #)";
    public string DescriptionHeistEngineering => "Erfordert Ingenieurwesen (Stufe #)";
    public string DescriptionHeistModerateValue => "Mäßiger Wert";
    public string DescriptionHeistHighValue => "Hoher Wert";
    public string DescriptionHeistPrecious => "Kostbar";
    public string DescriptionHeistPriceless => "Unschätzbar";

    public string AffixSuperior => "(hochwertig)";
    public string AffixBlighted => "Befallene";
    public string AffixBlightRavaged => "Extrem befallene";
}

