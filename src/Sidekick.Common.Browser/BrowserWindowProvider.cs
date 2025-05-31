using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Database;
using Sidekick.Common.Database.Tables;

namespace Sidekick.Common.Browser;

public class BrowserWindowProvider
(
    ILogger<BrowserWindowProvider> logger,
    DbContextOptions<SidekickDbContext> dbContextOptions
) : IBrowserWindowProvider
{
    public const string DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36";

    public event Action<BrowserRequest>? WindowOpened;

    public async Task<BrowserResult> OpenBrowserWindow(BrowserRequest options)
    {
        logger.LogInformation("[BrowserWindowProvider] Opening browser window at " + options.Uri);
        WindowOpened?.Invoke(options);
        return await options.TaskCompletion.Task;
    }

    public async Task SaveCookies(string clientName, BrowserResult result, CancellationToken cancellationToken)
    {
        await using var dbContext = new SidekickDbContext(dbContextOptions);
        var cookies = await dbContext.HttpClientCookies.Where(x => x.ClientName == clientName).ToListAsync(cancellationToken: cancellationToken);
        dbContext.HttpClientCookies.RemoveRange(cookies);
        await dbContext.SaveChangesAsync(cancellationToken);

        if (!result.Success) return;

        foreach (var cookie in result.Cookies)
        {
            if (cookie.Key == "POESESSID") continue;

            dbContext.HttpClientCookies.Add(new HttpClientCookie()
            {
                ClientName = clientName,
                Name = cookie.Key,
                Value = cookie.Value,
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<string> GetCookieString(string clientName, CancellationToken cancellationToken)
    {
        await using var dbContext = new SidekickDbContext(dbContextOptions);
        var cookies = await dbContext.HttpClientCookies.Where(x => x.ClientName == clientName).ToListAsync(cancellationToken: cancellationToken);
        return string.Join("; ", cookies.Select(x => $"{x.Name}={x.Value}"));
    }
}
