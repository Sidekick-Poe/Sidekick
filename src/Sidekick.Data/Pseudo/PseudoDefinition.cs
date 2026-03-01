namespace Sidekick.Data.Pseudo;

public class PseudoDefinition
{
    public string? PseudoStatId { get; init; }

    public string? Text { get; init; }

    public List<PseudoStat> Stats { get; init; } = [];
}
