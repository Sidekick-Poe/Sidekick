namespace Sidekick.Game.Pseudo;

public class PseudoStat
{
    public required string Id { get; init; }

    public required string Text { get; init; }

    public double Multiplier { get; init; }

    public override string ToString()
    {
        return $"{Text} - {Multiplier}x";
    }
}
