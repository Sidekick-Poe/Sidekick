namespace Sidekick.Apis.Poe.Trade.Filters
{
    public class SocketFilterGroup
    {
        public bool Disabled { get; set; }
        public SocketFilter Filters { get; set; } = new();
    }
}
