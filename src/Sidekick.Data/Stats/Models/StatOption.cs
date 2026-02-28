namespace Sidekick.Data.Stats.Models;

public class StatOption
{
    public required int Id { get; set; }

    public required string Text { get; set; }

    public override string ToString() => Text;
}
