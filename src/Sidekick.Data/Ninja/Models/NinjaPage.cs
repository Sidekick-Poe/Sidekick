namespace Sidekick.Data.Ninja.Models;

public record NinjaPage(
    string Type,
    string Url,
    bool SupportsExchange,
    bool SupportsStash);
