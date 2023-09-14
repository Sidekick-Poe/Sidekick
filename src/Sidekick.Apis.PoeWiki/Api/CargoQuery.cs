namespace Sidekick.Apis.PoeWiki.Api
{
    internal record CargoQuery<T> where T : class
    {
        public T? Title { get; init; }
    }
}
