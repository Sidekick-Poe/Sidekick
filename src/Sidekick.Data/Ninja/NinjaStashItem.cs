namespace Sidekick.Data.Ninja;

public record NinjaStashItem(
    string? Name,
    string? DetailsId,
    bool? Corrupted,
    int? GemLevel,
    int? GemQuality,
    int? MapTier,
    int? Links,
    int? ItemLevel,
    string? Variant,
    NinjaPage Page);
