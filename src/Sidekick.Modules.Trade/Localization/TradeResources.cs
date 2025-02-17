using Microsoft.Extensions.Localization;

namespace Sidekick.Modules.Trade.Localization;

public class TradeResources(IStringLocalizer<TradeResources> localizer)
{
    public string LoadMoreData => localizer["LoadMoreData"];
    public string Stock => localizer["Stock"];
    public string Trade => localizer["Trade"];
}
