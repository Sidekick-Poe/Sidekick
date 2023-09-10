namespace Sidekick.Apis.PoeNinja.Api
{
    public record PoeNinjaCacheItem<T>
    {
        public string? Type { get; init; }

        public List<T> Items { get; init; } = new();
    }
}
