namespace Sidekick.Apis.PoeNinja.Items.Models;

public record NinjaPage(
    string Type,
    string Url,
    bool SupportsExchange,
    bool SupportsStash);
