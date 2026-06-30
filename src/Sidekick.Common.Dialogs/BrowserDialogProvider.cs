using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Database;
using Sidekick.Common.Database.Tables;
namespace Sidekick.Common.Dialogs;

/// <summary>
/// Provides an interface for managing browser window interactions, such as opening
/// browser windows, handling cookie management, and notifying when a browser window
/// is opened.
/// </summary>
public class BrowserDialogProvider
(
    ILogger<BrowserDialogProvider> logger,
    DbContextOptions<SidekickDbContext> dbContextOptions
)
{
    public const string DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36";

    public record ShouldCompleteArgs(Uri? Uri, Dictionary<string, string> Cookies, string? JsonContent, bool IsJson);
    public record OpenedArgs(Uri Uri, Func<ShouldCompleteArgs, bool> ShouldComplete, TaskCompletionSource<Result> TaskCompletion);
    public event Action<OpenedArgs>? Opened;

    public record Result(Uri? Uri, bool Success, string? UserAgent, Dictionary<string, string> Cookies, string? JsonContent);

    public async Task<Result> OpenBrowserWindow(Uri uri, Func<ShouldCompleteArgs, bool>? shouldComplete = null)
    {
        var taskCompletionSource = new TaskCompletionSource<Result>();
        var options = new OpenedArgs(uri, shouldComplete ?? (_ => false), taskCompletionSource);
        logger.LogInformation("[BrowserWindowProvider] Opening browser window at " + options.Uri);
        Opened?.Invoke(options);
        return await options.TaskCompletion.Task;
    }

    public async Task SaveCookies(string clientName, Result result, CancellationToken cancellationToken)
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
