namespace Sidekick.Apis.PoeNinja.Api
{
    internal record PoeNinjaQueryResultLanguage
    {
        public Dictionary<string, string> Translations { get; init; } = new();
    }
}
