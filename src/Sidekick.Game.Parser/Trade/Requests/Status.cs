using Sidekick.Game.Parser.Filters.Definitions;
namespace Sidekick.Game.Parser.Trade.Requests;

public class Status
{
    public string Option { get; set; } = PlayerStatusFilterFactory.Online;
}
