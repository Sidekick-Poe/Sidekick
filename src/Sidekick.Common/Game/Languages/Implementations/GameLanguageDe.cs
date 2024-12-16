namespace Sidekick.Common.Game.Languages.Implementations;

[GameLanguage("German", "de")]
public class GameLanguageDE : IGameLanguage
{
    public string PoeTradeBaseUrl => "https://de.pathofexile.com/trade/";
    public string PoeTradeApiBaseUrl => "https://de.pathofexile.com/api/trade/";
    public string Poe2TradeBaseUrl => "https://de.pathofexile.com/trade2/";
    public string Poe2TradeApiBaseUrl => "https://de.pathofexile.com/api/trade2/";
    public Uri PoeCdnBaseUrl => new("https://web.poecdn.com/");

    public string RarityUnique => "Einzigartig";
    public string RarityRare => "Selten";
    public string RarityMagic => "Magisch";
    public string RarityNormal => "Normal";
    public string RarityCurrency => "Währung";
    public string RarityGem => "Gemme";
    public string RarityDivinationCard => "Weissagungskarte";

    public string DescriptionUnidentified => "Nicht identifiziert";
    public string DescriptionQuality => "Qualität";
    public string DescriptionAlternateQuality => "Alternative Qualität";
    public string DescriptionLevel => "Stufe";
    public string DescriptionCorrupted => "Verderbt";
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
    public string DescriptionArmour => "Rüstung";
    public string DescriptionEvasion => "Ausweichwert";
    public string DescriptionChanceToBlock => "Chance auf Blocken";
    public string DescriptionAttacksPerSecond => "Angriffe pro Sekunde";
    public string DescriptionCriticalStrikeChance => "Kritische Trefferchance";
    public string DescriptionMapTier => "Kartenlevel";
    public string DescriptionItemQuantity => "Gegenstandsmenge";
    public string DescriptionItemRarity => "Gegenstandsseltenheit";
    public string DescriptionMonsterPackSize => "Monstergruppengröße";
    public string DescriptionRequirements => "Anforderungen";
    public string DescriptionAreaLevel => "Gebietsstufe";

    public string AffixSuperior => "(hochwertig)";
    public string AffixBlighted => "Befallene";
    public string AffixBlightRavaged => "Extrem befallene";
    public string AffixAnomalous => "(anormal)";
    public string AffixDivergent => "(abweichend)";
    public string AffixPhantasmal => "(illusorisch)";

    public string InfluenceShaper => "Schöpfer-Gegenstand";
    public string InfluenceElder => "Ältesten-Gegenstand";
    public string InfluenceCrusader => "Kreuzritter-Gegenstand";
    public string InfluenceHunter => "Jäger-Gegenstand";
    public string InfluenceRedeemer => "Erlöserin-Gegenstand";
    public string InfluenceWarlord => "Kriegsfürst-Gegenstand";

    public ClassLanguage Classes { get; } = new()
    {
        Prefix = "Gegenstandsklasse",
        DivinationCard = "Weissagungskarten",
        StackableCurrency = "Stapelbare Währung",
        Jewel = "Juwelen",
        DelveStackableSocketableCurrency = "Delve: Stapelbare, einfassbare Währung",
        MetamorphSample = "Metamorph-Proben",
        HeistTool = "Heist-Werkzeug",
        Amulet = "Amulette",
        Ring = "Ringe",
        Belt = "Gürtel",
        Gloves = "Handschuhe",
        Boots = "Stiefel",
        BodyArmours = "Körperrüstungen",
        Helmets = "Helme",
        Shields = "Schilde",
        Quivers = "Köcher",
        LifeFlasks = "Lebensfläschchen",
        ManaFlasks = "Manafläschchen",
        HybridFlasks = "Hybridfläschchen",
        UtilityFlasks = "Hilfsfläschchen",
        ActiveSkillGems = "Fertigkeitengemmen",
        SupportSkillGems = "Unterstützungsgemmen",
        Maps = "Karten",
        MapFragments = "Kartenfragmente",
        Contract = "Aufträge",
        Blueprint = "Grundrisse",
        MiscMapItems = "Sonstige Kartengegenstände",
        Claws = "Klauen",
        Daggers = "Dolche",
        Wands = "Zauberstäbe",
        OneHandSwords = "Einhandschwerter",
        ThrustingOneHandSwords = "Einhandstichschwerter",
        OneHandAxes = "Einhandäxte",
        OneHandMaces = "Einhandstreitkolben",
        Bows = "Bögen",
        Staves = "Stäbe",
        TwoHandSwords = "Zweihandschwerter",
        TwoHandAxes = "Zweihandäxte",
        TwoHandMaces = "Zweihandstreitkolben",
        Sceptres = "Zepter",
        RuneDaggers = "Runendolche",
        Warstaves = "Kriegsstäbe",
        FishingRods = "Angelruten",
        HeistGear = "Heist-Ausrüstung",
        HeistBrooch = "Heist-Broschen",
        HeistTarget = "Auftragsziele",
        HeistCloak = "Heist-Umhänge",
        AbyssJewel = "Abgrund-Juwelen",
        Trinkets = "Schmuckstücke",
        Logbooks = "Expeditions-Logbücher",
        MemoryLine = "Erinnerungen",
        SanctumRelics = "Relikte",
        Tinctures = "Tinkturen",
        Corpses = "Leichen",
        SanctumResearch = "Sanktum-Forschungsauftrag",
    };
}

