using System.Text.Json.Serialization;

namespace Sidekick.Apis.PoeNinja.Api.Models
{
    public class PoeNinjaItem
    {
        //public int Id { get; set; }

        public string Name { get; set; }

        //public Uri Icon { get; set; }

        public int MapTier { get; set; }

        //public int LevelRequired { get; set; }

        //public string BaseType { get; set; }

        //public string Variant { get; set; }


        //public string ArtFilename { get; set; }

        public int Links { get; set; }

        public int ItemClass { get; set; }

        [JsonPropertyName("Sparkline")]
        public SparkLine SparkLine { get; set; }

        [JsonPropertyName("lowConfidenceSparkline")]
        public SparkLine LowConfidenceSparkLine { get; set; }

        //public List<Modifier> ImplicitModifiers { get; set; }

        //public List<Modifier> ExplicitModifiers { get; set; }

        //public string FlavourText { get; set; }

        public bool Corrupted { get; set; }

        public int GemLevel { get; set; }

        public int GemQuality { get; set; }

        //public string ItemType { get; set; }

        public double ChaosValue { get; set; }

        //public double ExaltedValue { get; set; }

        //public int Count { get; set; }

        public string DetailsId { get; set; }
    }
}
