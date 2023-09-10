namespace Sidekick.Apis.PoeNinja.Api
{
    public record PoeNinjaQueryResult<T>
    {
        public List<T> Lines { get; init; } = new();
    }
}
