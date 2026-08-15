namespace Sidekick.Game.Pseudo;

public class PseudoStat
{
    public string Id { get; init; }

    public string Text { get; init; }

    public double Multiplier { get; init; }

    public override string ToString()
    {
        return $"{Text} - {Multiplier}x";
    }
}
