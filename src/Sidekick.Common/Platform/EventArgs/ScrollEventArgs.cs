
namespace Sidekick.Common.Platform.EventArgs;

public class ScrollEventArgs
{
    public required string Masks { get; init; }

    public bool Suppress { get; set; }
}
