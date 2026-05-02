using Sidekick.Common.Database.Tables;

namespace Sidekick.Common.Ui.Views;

public interface IViewPreferenceService
{
    Task<ViewPreference?> Get();

    Task Set(int width, int height, int? x, int? y);
}
