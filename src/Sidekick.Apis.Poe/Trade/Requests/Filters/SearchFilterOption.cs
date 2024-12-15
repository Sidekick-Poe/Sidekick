using Sidekick.Apis.Poe.Trade.Models;

namespace Sidekick.Apis.Poe.Trade.Requests.Filters
{
    internal class SearchFilterOption
    {
        public SearchFilterOption(string option)
        {
            Option = option;
        }

        public SearchFilterOption(PropertyFilter filter)
        {
            Option = filter.Checked == true ? "true" : "false";
        }

        public string Option { get; set; }

        internal class AlternateGemQualityOptions
        {
            public const string Anomalous = "1";
            public const string Divergent = "2";
            public const string Phantasmal = "3";
        }
    }
}
