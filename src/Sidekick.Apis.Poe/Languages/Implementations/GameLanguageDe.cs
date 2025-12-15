
namespace Sidekick.Apis.Poe.Languages.Implementations;

[GameLanguage("German", "de")]
public class GameLanguageDe : IGameLanguage
{
    public string Code => "de";

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

    public string DescriptionRarity => "Seltenheit";
    public string DescriptionUnidentified => "Nicht identifiziert";
    public string DescriptionQuality => "Qualität";
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
    public string DescriptionMagicMonsters => "Magische Monster";
    public string DescriptionRareMonsters => "Seltene Monster";
    public string DescriptionRevivesAvailable => "Wiederbelebungen verfügbar";
    public string DescriptionWaystoneDropChance => "Chance auf fallen gelassene Wegsteine";
    public string DescriptionAreaLevel => "Gebietsstufe";
    public string DescriptionUnusable => "Du kannst diesen Gegenstand nicht benutzen. Seine Eigenschaften werden ignoriert.";
    public string DescriptionRequirements => "Anforderungen";
    public string DescriptionRequires => "Erfordert";
    public string DescriptionRequiresLevel => "Stufe";
    public string DescriptionRequiresStr => "Str";
    public string DescriptionRequiresDex => "Ges";
    public string DescriptionRequiresInt => "Int";

    public string AffixSuperior => "(hochwertig)";
    public string AffixBlighted => "Befallene";
    public string AffixBlightRavaged => "Extrem befallene";

    public string InfluenceShaper => "Schöpfer-Gegenstand";
    public string InfluenceElder => "Ältesten-Gegenstand";
    public string InfluenceCrusader => "Kreuzritter-Gegenstand";
    public string InfluenceHunter => "Jäger-Gegenstand";
    public string InfluenceRedeemer => "Erlöserin-Gegenstand";
    public string InfluenceWarlord => "Kriegsfürst-Gegenstand";

    public string RegexIncreased => "erhöhte|Erhöhte|erhöhter|Erhöhter";
    public string RegexReduced => "verringerte|Verringerte|verringerten|Verringerten|verringerungen|Verringerungen";
    public string RegexMore => "mehr|Mehr";
    public string RegexLess => "weniger|Weniger";
    public string RegexFaster => "schnellerer|Schnellerer|schneller|Schneller|schnellere|Schnellere";
    public string RegexSlower => "verlangsamende|Verlangsamende|verlangsamungen|Verlangsamungen";

    public ClassLanguage Classes { get; } = new()
    {
        Prefix = "Gegenstandsklasse",
        DivinationCard = "Weissagungskarten",
        StackableCurrency = "Stapelbare Währung",
        Socketable = "Einfassbar",
        Omen = "Omen",
        Jewel = "Juwelen",
        DelveStackableSocketableCurrency = "Delve: Stapelbare, einfassbare Währung",
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
        Focus = "Fokusse",
        LifeFlasks = "Lebensfläschchen",
        ManaFlasks = "Manafläschchen",
        HybridFlasks = "Hybridfläschchen",
        UtilityFlasks = "Hilfsfläschchen",
        ActiveSkillGems = "Fertigkeitengemmen",
        SupportSkillGems = "Unterstützungsgemmen",
        UncutSkillGems = "Ungeschnittene Fertigkeitengemme",
        UncutSupportGems = "Ungeschnittene Unterstützungsgemme",
        UncutSpiritGems = "Ungeschnittene Willegemme",
        Maps = "Karten",
        MapFragments = "Kartenfragmente",
        Contract = "Aufträge",
        Blueprint = "Grundrisse",
        MiscMapItems = "Sonstige Kartengegenstände",
        Waystone = "Wegsteine",
        Barya = "Prüfungsmünzen",
        Ultimatum = "Inschrift des Ultimatums",
        Tablet = "Tafel",
        Breachstone = "Riss-Steine",
        BossKey = "Zinnenschhlüssel",
        Claws = "Klauen",
        Daggers = "Dolche",
        Wands = "Zauberstäbe",
        OneHandSwords = "Einhandschwerter",
        ThrustingOneHandSwords = "Einhandstichschwerter",
        OneHandAxes = "Einhandäxte",
        OneHandMaces = "Einhandstreitkolben",
        Bows = "Bögen",
        Crossbows = "Armbrüste",
        Staves = "Stäbe",
        TwoHandSwords = "Zweihandschwerter",
        TwoHandAxes = "Zweihandäxte",
        TwoHandMaces = "Zweihandstreitkolben",
        Sceptres = "Zepter",
        RuneDaggers = "Runendolche",
        Warstaves = "Kriegsstäbe",
        Quarterstaves = "Kampfstäbe",
        Spears = "Speere",
        Bucklers = "Faustschild",
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
        Grafts = "Transplantate",
        Wombgifts = "Fruchtkeime",
        Talismans = "Talismane",
    };
}

