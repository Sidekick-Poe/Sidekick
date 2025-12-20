using Sidekick.Apis.Poe.Trade.Trade.Filters.Definitions;
namespace Sidekick.Apis.Poe.Trade.Trade.Items.Requests;

public class Status
{
    public string Option { get; set; } = PlayerStatusFilter.Online;
}
