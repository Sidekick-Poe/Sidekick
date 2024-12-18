using Microsoft.Extensions.Logging;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.CloudFlare;

public class CloudflareService
(
    ISettingsService settingsService,
    ILogger<CloudflareService> logger
) : ICloudflareService
{
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

    public async Task CaptchaChallengeCompleted(string cookie)
    {
        logger.LogInformation("[CloudflareService] Cloudflare challenge completed.");
        await settingsService.Set(SettingKeys.CloudflareCookie, cookie);

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
}
