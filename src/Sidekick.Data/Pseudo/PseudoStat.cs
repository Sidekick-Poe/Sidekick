namespace Sidekick.Data.Pseudo;

public class PseudoStat
(
    string id,
    string text,
    double multiplier
)
{
    public string Id { get; } = id;

    public string Text { get; } = text;

    public double Multiplier { get; } = multiplier;

    public override string ToString()
    {
        return $"{Text} - {Multiplier}x";
    }
}
