using System;

namespace Sidekick.Common.Game.Languages.Implementations
{
    [GameLanguage("Spanish", "es")]
    public class GameLanguageES : IGameLanguage
    {
        public string LanguageCode => "es";

        public Uri PoeTradeSearchBaseUrl => new("https://es.pathofexile.com/trade/search/");
        public Uri PoeTradeExchangeBaseUrl => new("https://es.pathofexile.com/trade/exchange/");
        public Uri PoeTradeApiBaseUrl => new("https://es.pathofexile.com/api/trade/");
        public Uri PoeCdnBaseUrl => new("https://web.poecdn.com/");

        public string RarityUnique => "Único";
        public string RarityRare => "Raro";
        public string RarityMagic => "Mágico";
        public string RarityNormal => "Normal";
        public string RarityCurrency => "Objetos Monetarios";
        public string RarityGem => "Gema";
        public string RarityDivinationCard => "Carta de Adivinación";

        public string DescriptionUnidentified => "Sin identificar";
        public string DescriptionQuality => "Calidad";
        public string DescriptionAlternateQuality => "Calidad alternativa";
        public string DescriptionIsRelic => "Reliquia única";
        public string DescriptionCorrupted => "Corrupto";
        public string DescriptionScourged => "de calamidad";
        public string DescriptionSockets => "Engarces";
        public string DescriptionItemLevel => "Nivel de Objeto";
        public string DescriptionExperience => "Experiencia";
        public string DescriptionMapTier => "Grado del Mapa";
        public string DescriptionItemQuantity => "Cantidad de Objetos";
        public string DescriptionItemRarity => "Rareza de Objetos";
        public string DescriptionMonsterPackSize => "Tamaño de Grupos de Monstruos";
        public string DescriptionPhysicalDamage => "__TranslationRequired__";
        public string DescriptionElementalDamage => "__TranslationRequired__";
        public string DescriptionAttacksPerSecond => "__TranslationRequired__";
        public string DescriptionCriticalStrikeChance => "__TranslationRequired__";
        public string DescriptionEnergyShield => "__TranslationRequired__";
        public string DescriptionArmour => "__TranslationRequired__";
        public string DescriptionEvasion => "__TranslationRequired__";
        public string DescriptionChanceToBlock => "__TranslationRequired__";
        public string DescriptionLevel => "__TranslationRequired__";
        public string DescriptionRequirements => "__TranslationRequired__:";

        public string PrefixSuperior => "Superior";
        public string PrefixBlighted => "Infestado";
        public string PrefixAnomalous => "anómala";
        public string PrefixDivergent => "divergente";
        public string PrefixPhantasmal => "fantasmal";

        public string InfluenceShaper => "Creador";
        public string InfluenceElder => "Antiguo";
        public string InfluenceCrusader => "Cruzado";
        public string InfluenceHunter => "Cazador";
        public string InfluenceRedeemer => "Redentora";
        public string InfluenceWarlord => "Jefe de guerra";

        public ClassLanguage Classes => null;
    }
}
