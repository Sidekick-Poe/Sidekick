namespace Sidekick.Common.Ui.Forms;

public class SelectOption
{
    public required string Label { get; init; }

    public required string? Value { get; init; }

    public bool Disabled { get; init; }

    public string? Group { get; init; }
}
