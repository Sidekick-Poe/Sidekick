using System.Collections.Generic;
using System.Text.Json.Serialization;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Trade.Results
{
    public class ResultItem
    {
        public string Name { get; set; }

        public string TypeLine { get; set; }

        public bool Identified { get; set; }

        [JsonPropertyName("ilvl")]
        public int ItemLevel { get; set; }

        [JsonPropertyName("frameType")]
        public Rarity Rarity { get; set; }

        public bool Corrupted { get; set; }

        public bool Fractured { get; set; }

        public bool IsRelic { get; set; }

        public Influences Influences { get; set; } = new Influences();

        public bool Verified { get; set; }

        [JsonPropertyName("w")]
        public int Width { get; set; }

        [JsonPropertyName("h")]
        public int Height { get; set; }

        public string Icon { get; set; }

        public string League { get; set; }

        public string Note { get; set; }

        public List<ResultLineContent> Requirements { get; set; }

        public List<ResultLineContent> Properties { get; set; }

        public List<ResultLineContent> AdditionalProperties { get; set; }

        [JsonPropertyName("implicitMods")]
        public List<string> ImplicitMods { get; set; }

        [JsonPropertyName("craftedMods")]
        public List<string> CraftedMods { get; set; }

        [JsonPropertyName("explicitMods")]
        public List<string> ExplicitMods { get; set; }

        [JsonPropertyName("utilityMods")]
        public List<string> UtilityMods { get; set; }

        [JsonPropertyName("pseudoMods")]
        public List<string> PseudoMods { get; set; }

        [JsonPropertyName("enchantMods")]
        public List<string> EnchantMods { get; set; }

        [JsonPropertyName("fracturedMods")]
        public List<string> FracturedMods { get; set; }

        public List<ResultSocket> Sockets { get; set; } = new List<ResultSocket>();

        public Extended Extended { get; set; } = new Extended();
    }
}
