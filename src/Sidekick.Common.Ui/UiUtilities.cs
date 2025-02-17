
namespace Sidekick.Common.Ui;

public static class UiUtilities
{
    public static string GenerateId(Guid? guid = null)
    {
        guid ??= Guid.NewGuid();
        var shortened = Convert.ToBase64String(guid.Value.ToByteArray()).Replace("/", "_").Replace("+", "_").TrimEnd('=');
        return $"ui{shortened}";
    }
}
