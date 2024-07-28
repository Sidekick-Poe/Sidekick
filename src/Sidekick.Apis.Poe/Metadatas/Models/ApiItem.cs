using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Metadatas.Models
{
    public class ApiItem
    {
        public string? Name { get; set; }

        public string? Type { get; set; }

        public string? Text { get; set; }

        [JsonPropertyName("disc")]
        public string? Discriminator { get; set; }

        public ApiItemFlags? Flags { get; set; }
    }
}
