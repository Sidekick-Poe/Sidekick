using Sidekick.Apis.Poe.Trade.Filters.Definitions;
namespace Sidekick.Apis.Poe.Trade.Trade.Requests;

public class Status
{
    public string Option { get; set; } = PlayerStatusFilterFactory.Online;
}
