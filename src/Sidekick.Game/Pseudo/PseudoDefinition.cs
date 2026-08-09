namespace Sidekick.Game.Pseudo;

public class PseudoDefinition
{
    public string? PseudoStatId { get; init; }

    public string? Text { get; init; }

    public List<PseudoStat> Stats { get; init; } = [];

    public override string ToString()
    {
        return Text ?? string.Empty;
    }
}
