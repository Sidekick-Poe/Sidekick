namespace Sidekick.Data.Options;

public class DataOptions
{
    public string DataFolder { get; set; } = string.Empty;
    public string? Poe1League { get; set; }
    public string? Poe2League { get; set; }
    public int TimeoutSeconds { get; set; } = 60;
}