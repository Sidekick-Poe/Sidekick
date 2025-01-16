using System.Text.Json;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.CloudFlare;

public class CloudflareService
(
    ISettingsService settingsService,
    ILogger<CloudflareService> logger
) : ICloudflareService
{
    private const string DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36";

    public event Action<Uri>? ChallengeStarted;

    private TaskCompletionSource<bool>? challengeCompletion;
    private bool isHandlingChallenge;

    public Task<bool> StartCaptchaChallenge(Uri? uri = null, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[CloudflareService] Starting Cloudflare challenge.");

        if (uri == null)
        {
            logger.LogInformation("[CloudflareService] No uri provided, skipping.");
            return Task.FromResult(false);
        }

        if (isHandlingChallenge && challengeCompletion != null)
        {
            return challengeCompletion.Task;
        }

        isHandlingChallenge = true;
        ChallengeStarted?.Invoke(uri);

        challengeCompletion = new TaskCompletionSource<bool>();
        return challengeCompletion.Task;
    }

    public async Task CaptchaChallengeCompleted(Dictionary<string, string> cookies)
    {
        logger.LogInformation("[CloudflareService] Cloudflare challenge completed.");
        var cookieString = string.Join("; ", cookies.Select(c => $"{c.Key}={c.Value}"));
        await settingsService.Set(SettingKeys.CloudflareCookies, cookieString);

        challengeCompletion?.TrySetResult(true);
        isHandlingChallenge = false;
    }

    public Task CaptchaChallengeFailed()
    {
        logger.LogInformation("[CloudflareService] Cloudflare challenge failed.");
        challengeCompletion?.TrySetResult(false);
        isHandlingChallenge = false;
        return Task.CompletedTask;
    }

    public async Task InitializeHttpRequest(HttpRequestMessage request)
    {
        request.Headers.UserAgent.Clear();

        var userAgent = await settingsService.GetString(SettingKeys.CloudflareUserAgent);
        if (!string.IsNullOrEmpty(userAgent))
        {
            request.Headers.UserAgent.ParseAdd(userAgent);
        }
        else
        {
            request.Headers.UserAgent.ParseAdd(DefaultUserAgent);
        }


        var cookies = await settingsService.GetString(SettingKeys.CloudflareCookies);
        if (!string.IsNullOrEmpty(cookies))
        {
            logger.LogInformation("[CloudflareService] Adding cookie to request");
            // Append the cookie to the `Cookie` header
            if (!request.Headers.Contains("Cookie"))
            {
                request.Headers.Add("Cookie", cookies);
            }
            else
            {
                request.Headers.Remove("Cookie");
                request.Headers.Add("Cookie", cookies);
            }
        }
        else
        {
            logger.LogInformation("[CloudflareService] No cookies found");
        }
    }

    public async Task SetUserAgent(string userAgent)
    {
        logger.LogInformation("[CloudflareService] Setting user agent to: " + userAgent);
        await settingsService.Set(SettingKeys.CloudflareUserAgent, userAgent);
    }
}
