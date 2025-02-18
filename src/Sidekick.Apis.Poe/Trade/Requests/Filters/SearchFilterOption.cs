using Sidekick.Apis.Poe.Parser.Properties.Filters;

namespace Sidekick.Apis.Poe.Trade.Requests.Filters;

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

    public SearchFilterOption(BooleanPropertyFilter filter)
    {
        Option = filter.Checked ? "true" : "false";
    }

    public SearchFilterOption(TriStatePropertyFilter filter)
    {
        Option = filter.Checked == true ? "true" : "false";
    }

    public string Option { get; set; }
}
