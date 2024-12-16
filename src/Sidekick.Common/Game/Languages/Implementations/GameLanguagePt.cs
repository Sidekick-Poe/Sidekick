namespace Sidekick.Common.Game.Languages.Implementations;

[GameLanguage("Portuguese", "pt")]
public class GameLanguagePT : IGameLanguage
{
    public string PoeTradeBaseUrl => "https://br.pathofexile.com/trade/";
    public string PoeTradeApiBaseUrl => "https://br.pathofexile.com/api/trade/";
    public string Poe2TradeBaseUrl => "https://br.pathofexile.com/trade2/";
    public string Poe2TradeApiBaseUrl => "https://br.pathofexile.com/api/trade2/";
    public Uri PoeCdnBaseUrl => new("https://web.poecdn.com/");

    public string RarityUnique => "Único";
    public string RarityRare => "Raro";
    public string RarityMagic => "Mágico";
    public string RarityNormal => "Normal";
    public string RarityCurrency => "Moeda";
    public string RarityGem => "Gema";
    public string RarityDivinationCard => "Carta de Adivinhação";

    public string DescriptionUnidentified => "Não Identificado";
    public string DescriptionQuality => "Qualidade";
    public string DescriptionAlternateQuality => "Qualidade Alternativa";
    public string DescriptionLevel => "Nível";
    public string DescriptionCorrupted => "Corrompido";
    public string DescriptionSockets => "Encaixes";
    public string DescriptionItemLevel => "Nível do Item";
    public string DescriptionExperience => "Experiência";
    public string DescriptionPhysicalDamage => "Dano Físico";
    public string DescriptionElementalDamage => "Dano Elemental";
    public string DescriptionFireDamage => "Dano de Fogo";
    public string DescriptionColdDamage => "Dano de Gelo";
    public string DescriptionLightningDamage => "Dano de Raio";
    public string DescriptionChaosDamage => "Dano de Caos";
    public string DescriptionEnergyShield => "Escudo de Energia";
    public string DescriptionArmour => "Armadura";
    public string DescriptionEvasion => "Evasão";
    public string DescriptionChanceToBlock => "Chance de Bloquear";
    public string DescriptionAttacksPerSecond => "Ataques por Segundo";
    public string DescriptionCriticalStrikeChance => "Chance de Crítico";
    public string DescriptionMapTier => "Tier do Mapa";
    public string DescriptionItemQuantity => "Quantidade de Itens";
    public string DescriptionItemRarity => "Raridade de Itens";
    public string DescriptionMonsterPackSize => "Tamanho do Grupo de Monstros";
    public string DescriptionRequirements => "Requisitos";
    public string DescriptionAreaLevel => "Nível da Área";

    public string AffixSuperior => "Superior";
    public string AffixBlighted => "Infestado";
    public string AffixBlightRavaged => "Devastado";
    public string AffixAnomalous => "Anômalo";
    public string AffixDivergent => "Divergente";
    public string AffixPhantasmal => "Fantasmal";

    public string InfluenceShaper => "Item do Criador";
    public string InfluenceElder => "Item do Ancião";
    public string InfluenceCrusader => "Item do Cruzado";
    public string InfluenceHunter => "Item do Caçador";
    public string InfluenceRedeemer => "Item do Redentor";
    public string InfluenceWarlord => "Item do Senhor da Guerra";

    public ClassLanguage Classes { get; } = new()
    {
        Prefix = "Classe do Item",
        DivinationCard = "Cartas de Adivinhação",
        StackableCurrency = "Moedas Empilháveis",
        Jewel = "Joias",
        DelveStackableSocketableCurrency = "Item Monetário Aglomerável e Encaixável Delve",
        MetamorphSample = "Amostras Metamorfas",
        HeistTool = "Ferramentas Heist",
        Amulet = "Amuletos",
        Ring = "Anéis",
        Belt = "Cintos",
        Gloves = "Luvas",
        Boots = "Botas",
        BodyArmours = "Peitorais",
        Helmets = "Elmos",
        Shields = "Escudos",
        Quivers = "Aljavas",
        LifeFlasks = "Frascos de Vida",
        ManaFlasks = "Frascos de Mana",
        HybridFlasks = "Frascos Híbridos",
        UtilityFlasks = "Frascos de Utilidade",
        ActiveSkillGems = "Gemas de Habilidades",
        SupportSkillGems = "Gemas de Suporte",
        Maps = "Mapas",
        MapFragments = "Fragmentos de Mapas",
        Contract = "Contratos",
        Blueprint = "Plantas",
        MiscMapItems = "Itens Mapas Variados",
        Claws = "Garras",
        Daggers = "Adagas",
        Wands = "Varinhas",
        OneHandSwords = "Espadas de Uma Mão",
        ThrustingOneHandSwords = "Espadas de Estocada de Uma Mão",
        OneHandAxes = "Machados de Uma Mão",
        OneHandMaces = "Maças de Uma Mão",
        Bows = "Arcos",
        Staves = "Cajados",
        TwoHandSwords = "Espadas de Duas Mãos",
        TwoHandAxes = "Machados de Duas Mãos",
        TwoHandMaces = "Maças de Duas Mãos",
        Sceptres = "Cetros",
        RuneDaggers = "Adagas Rúnicas",
        Warstaves = "Cajados de Guerra",
        FishingRods = "Varas de Pescar",
        HeistGear = "Equipamento Heist",
        HeistBrooch = "Broches Heist",
        HeistTarget = "Alvos Heist",
        HeistCloak = "Capas Heist",
        AbyssJewel = "Joias Abissais",
        Trinkets = "Adornos",
        Logbooks = "Diários de Bordo Expedition",
        MemoryLine = "Memórias",
        SanctumRelics = "Relíquias",
        Tinctures = "Tintura",
        Corpses = "Patuá",
        SanctumResearch = "Pesquisa Sanctum",
    };
}

