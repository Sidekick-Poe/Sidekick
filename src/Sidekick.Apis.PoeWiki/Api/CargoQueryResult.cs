namespace Sidekick.Apis.PoeWiki.Api
{
    public record CargoQueryResult<T> where T : class
    {
        public List<CargoQuery<T>> CargoQuery { get; init; } = new();
    }
}
