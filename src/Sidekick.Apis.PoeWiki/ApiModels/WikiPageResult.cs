using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Sidekick.Apis.PoeWiki.JsonConverters;

namespace Sidekick.Apis.PoeWiki.ApiModels
{
    public class WikiPageResult
    {
        [JsonPropertyName("parse")]
        public WikiPageParse Parse  { get; set; }
    }

    public class WikiPageParse
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("pageid")]
        public int PageId { get; set; }

        [JsonPropertyName("wikitext")]
        public string WikiText { get; set; }
    }
}
