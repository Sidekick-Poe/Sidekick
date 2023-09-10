namespace Sidekick.Apis.PoeNinja.Api
{
    public record PoeNinjaQueryResultLanguage
    {
        public Dictionary<string, string> Translations { get; init; } = new();
    }
}
