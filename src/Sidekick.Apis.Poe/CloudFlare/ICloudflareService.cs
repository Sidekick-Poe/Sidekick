namespace Sidekick.Apis.Poe.CloudFlare;

public interface ICloudflareService
{
    event Action<Uri>? ChallengeStarted;

    Task<bool> StartCaptchaChallenge(Uri? uri = null, CancellationToken cancellationToken = default);

    Task CaptchaChallengeCompleted(Dictionary<string, string> cookies);

    Task CaptchaChallengeFailed();

    Task AddCookieToRequest(HttpRequestMessage request);
}
