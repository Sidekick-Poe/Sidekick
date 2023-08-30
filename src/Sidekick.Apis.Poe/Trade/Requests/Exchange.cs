namespace Sidekick.Apis.Poe.Trade.Requests
{
    public class Exchange
    {
        public List<string> Want { get; set; } = new();

        public List<string> Have { get; set; } = new();

        public Status Status { get; set; } = new();
    }
}
