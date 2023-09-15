using Sidekick.Common.Enums;

namespace Sidekick.Apis.Poe.Trade.Models
{
    public enum TradeCurrency
    {
        [EnumValue(null)]
        ChaosEquivalent,

        [EnumValue("chaos_divine")]
        ChaosOrDivine,

        [EnumValue("chaos")]
        Chaos,

        [EnumValue("exalted")]
        Exalted,

        [EnumValue("divine")]
        Divine,

        [EnumValue("gold")]
        GoldCoin,

        [EnumValue("blessed")]
        Blessed,

        [EnumValue("chisel")]
        Cartographer,

        [EnumValue("chrome")]
        Chromatic,

        [EnumValue("gcp")]
        Gemcutter,

        [EnumValue("jewellers")]
        Jeweller,

        [EnumValue("scour")]
        Scouring,

        [EnumValue("regret")]
        Regret,

        [EnumValue("fusing")]
        Fusing,

        [EnumValue("chance")]
        Chance,

        [EnumValue("alt")]
        Alteration,

        [EnumValue("alch")]
        Alchemy,

        [EnumValue("regal")]
        Regal,

        [EnumValue("vaal")]
        Vaal,
    }
}
