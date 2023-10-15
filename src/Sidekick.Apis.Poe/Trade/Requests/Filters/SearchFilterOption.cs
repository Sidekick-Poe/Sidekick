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
            Option = filter.Enabled == true ? "true" : "false";
        }

        public string Option { get; set; }
    }
}
