namespace Sidekick.Game.Parser.Trade.Requests.Models;

public class TypeDiscriminator
{
    public required string? Option { get; init; }

    public string? Discriminator { get; init; }
}
