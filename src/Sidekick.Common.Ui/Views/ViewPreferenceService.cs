using Microsoft.EntityFrameworkCore;
using Sidekick.Common.Database;
using Sidekick.Common.Database.Tables;
using Sidekick.Common.Settings;

namespace Sidekick.Common.Ui.Views;

public class ViewPreferenceService
(
    DbContextOptions<SidekickDbContext> dbOptions,
    ISettingsService settingsService
) : IViewPreferenceService
{
    public async Task<ViewPreference?> Get(string? key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return null;
        }

        var dbContext = new SidekickDbContext(dbOptions);
        var viewPreference = await dbContext.ViewPreferences.FirstOrDefaultAsync(x => x.Key == key);
        return viewPreference;
    }

    public async Task Set(string? key, int width, int height, int? x, int? y)
    {
        if (string.IsNullOrEmpty(key))
        {
            return;
        }

        var saveWindowPosition = await settingsService.GetBool(SettingKeys.SaveWindowPositions);
        if (!saveWindowPosition)
        {
            x = null;
            y = null;
        }

        var dbContext = new SidekickDbContext(dbOptions);
        var viewPreference = await dbContext.ViewPreferences.FirstOrDefaultAsync(preference => preference.Key == key);
        if (viewPreference == null)
        {
            viewPreference = new ViewPreference
            {
                Key = key,
                Width = width,
                Height = height,
                X = x,
                Y = y,
            };
            dbContext.ViewPreferences.Add(viewPreference);
            await dbContext.SaveChangesAsync();
        }
        else
        {
            viewPreference.Width = width;
            viewPreference.Height = height;
            viewPreference.X = x;
            viewPreference.Y = y;
            await dbContext.SaveChangesAsync();
        }
    }
}
