namespace Sidekick.Apis.Poe.Trade.Requests.Models;

public class TypeDiscriminator
{
    public required string? Option { get; init; }

    public string? Discriminator { get; init; }
}
