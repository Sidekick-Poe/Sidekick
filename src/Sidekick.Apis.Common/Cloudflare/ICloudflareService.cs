namespace Sidekick.Apis.Common.Cloudflare;

public interface ICloudflareService
{
    Task<bool> Challenge(string clientName, Uri? uri, CancellationToken cancellationToken);

    Task InitializeHttpRequest(string clientName, HttpRequestMessage request, CancellationToken cancellationToken);
}
