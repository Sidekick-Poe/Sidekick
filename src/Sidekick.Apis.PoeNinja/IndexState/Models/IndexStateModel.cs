namespace Sidekick.Apis.PoeNinja.IndexState.Models;

public class IndexStateModel
{
    public List<IndexStateLeague> EconomyLeagues { get; set; } = [];

    public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.Now;
}
