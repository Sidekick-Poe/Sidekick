using System.Net;

namespace Sidekick.Apis.Poe.Tests.Poe2.Query;

public class MockHttpClient : HttpClient
{
    public List<string> Requests { get; } = [];

    public override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Content is not null)
        {
            var content = await request.Content.ReadAsStringAsync(cancellationToken);
            Requests.Add(content);
        }

        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        };
    }
}
