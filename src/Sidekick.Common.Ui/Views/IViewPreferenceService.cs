using Sidekick.Common.Database.Tables;

namespace Sidekick.Common.Ui.Views;

public interface IViewPreferenceService
{
    Task<ViewPreference?> Get(string? key);

    Task Set(string? key, int width, int height);
}
