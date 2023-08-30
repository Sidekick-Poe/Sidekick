namespace Sidekick.Apis.Poe.Trade.Results
{
    public class Listing
    {
        public string? Method { get; set; }
        public DateTimeOffset Indexed { get; set; }
        public Stash? Stash { get; set; }
        public string? Whisper { get; set; }
        public Account? Account { get; set; }
        public Price? Price { get; set; }
    }
}
