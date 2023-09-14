namespace Sidekick.Apis.Poe.Trade.Filters
{
    internal class RequirementFilterGroup
    {
        public bool Disabled { get; set; }
        public RequirementFilter Filters { get; set; } = new();
    }
}
