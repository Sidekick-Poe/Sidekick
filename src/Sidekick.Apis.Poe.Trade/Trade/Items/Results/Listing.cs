namespace Sidekick.Apis.Poe.Trade.Trade.Items.Results;

public class Listing
{
    public DateTimeOffset Indexed { get; set; }
    public string? Whisper { get; set; }
    public required Account Account { get; set; }
    public required Price Price { get; set; }
}
