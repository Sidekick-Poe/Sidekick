namespace Sidekick.Data.Ninja;

public record NinjaPage(
    string Type,
    string Url,
    bool SupportsExchange,
    bool SupportsStash);
