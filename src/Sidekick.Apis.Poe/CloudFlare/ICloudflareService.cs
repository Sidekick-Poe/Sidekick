namespace Sidekick.Apis.Poe.CloudFlare;

public interface ICloudflareService
{
    event Action<Uri>? ChallengeStarted;

    Task<bool> StartCaptchaChallenge(Uri? uri = null, CancellationToken cancellationToken = default);

    Task CaptchaChallengeCompleted(string cookie);

    Task CaptchaChallengeFailed();
}
