namespace Sidekick.Apis.PoeWiki.Api
{
    public record CargoQuery<T> where T : class
    {
        public T? Title { get; init; }
    }
}
