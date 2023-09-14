namespace Sidekick.Apis.Poe.Trade.Filters
{
    internal class SocketFilterGroup
    {
        public bool Disabled { get; set; }
        public SocketFilter Filters { get; set; } = new();
    }
}
