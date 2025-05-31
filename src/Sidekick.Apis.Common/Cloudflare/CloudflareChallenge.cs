namespace Sidekick.Apis.Common.Cloudflare;

public class CloudflareChallenge
{
    public required string ClientName { get; set; }

    public required Uri Uri { get; set; }

    public TaskCompletionSource<bool> TaskCompletion { get; } = new();

    public Task<bool> Task => TaskCompletion.Task;
}
