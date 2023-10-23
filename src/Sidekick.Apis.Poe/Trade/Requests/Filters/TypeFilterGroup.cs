namespace Sidekick.Apis.Poe.Trade.Requests.Filters
{
    internal class TypeFilterGroup
    {
        public bool Disabled { get; set; }
        public TypeFilter Filters { get; set; } = new();
    }
}
