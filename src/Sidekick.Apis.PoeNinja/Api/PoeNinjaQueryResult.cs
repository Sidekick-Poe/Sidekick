namespace Sidekick.Apis.PoeNinja.Api
{
    internal record PoeNinjaQueryResult<T>
    {
        public List<T> Lines { get; init; } = new();
    }
}
