
namespace Sidekick.Common.Ui;

public static class UiUtilities
{
    private static int Index { get; set; }

    public static string GenerateId()
    {
        return $"sidekick-{Index++}";
    }
}
