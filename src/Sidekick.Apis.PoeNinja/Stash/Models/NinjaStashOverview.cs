namespace Sidekick.Apis.PoeNinja.Stash.Models;

public class NinjaStashOverview
{
    public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.Now;

    public List<NinjaStashLine> Lines { get; set; } = [];
}