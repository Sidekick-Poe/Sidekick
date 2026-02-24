using Sidekick.Apis.Poe.Items;
namespace Sidekick.Data.Builder.Repoe.Models.Poe1;

public class RepoeStatTrade
{
    public required string Id { get; set; }

    public required string Text { get; set; }

    public required StatCategory Type { get; set; }

    public string? Option { get; set; }
}
