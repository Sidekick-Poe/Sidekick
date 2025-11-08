namespace Sidekick.Apis.PoeNinja.Items.Models;

public record NinjaStashItem(
    string? Name,
    string? DetailsId,
    int? GemLevel,
    int? GemQuality,
    int? MapTier,
    int? Links,
    int? ItemLevel,
    string? Variant,
    NinjaPage Page);
