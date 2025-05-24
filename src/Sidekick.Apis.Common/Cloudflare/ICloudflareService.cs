namespace Sidekick.Apis.Common.Cloudflare;

public interface ICloudflareService
{
    event Action<CloudflareChallenge>? ChallengeStarted;

    Task<bool> StartCaptchaChallenge(Uri? uri = null, CancellationToken cancellationToken = default);

    Task CaptchaChallengeCompleted(Dictionary<string, string> cookies);

    Task CaptchaChallengeFailed();

    Task InitializeHttpRequest(HttpRequestMessage request);

    Task SetUserAgent(string userAgent);
}
