namespace Sidekick.Apis.Poe.Trade.Parser.Pseudo;

public class PseudoModifierDefinition
(
    string id,
    string type,
    string text,
    double multiplier
)
{
    public string Id { get; } = id;

    public string Type { get; } = type;

    public string Text { get; } = text;

    public double Multiplier { get; } = multiplier;

    public override string ToString()
    {
        return $"{Text} - {Multiplier}x ({Type})";
    }
}
