using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
namespace Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;

public class SearchFilterOption
{
    public SearchFilterOption()
    {
        Option = string.Empty;
    }

    public SearchFilterOption(string option)
    {
        Option = option;
    }

    public SearchFilterOption(TradeFilter filter)
    {
        Option = filter.Checked ? "true" : "false";
    }

    public SearchFilterOption(TriStatePropertyFilter filter)
    {
        Option = filter.Checked == true ? "true" : "false";
    }

    public string Option { get; set; }
}
