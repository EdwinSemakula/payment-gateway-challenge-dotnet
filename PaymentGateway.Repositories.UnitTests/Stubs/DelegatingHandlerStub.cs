using System.Net;

namespace PaymentGateway.Repositories.UnitTests.Stubs;

public class DelegatingHandlerStub : DelegatingHandler
{
    private HttpResponseMessage _httpResponseMessage = new() { Content = new StringContent(""), StatusCode = HttpStatusCode.OK };
    public List<HttpRequestMessage> RequestsMade { get; } = new();

    public void SetResponseMessage(HttpStatusCode statusCode, string content = null)
    {
        _httpResponseMessage = content != null
            ? new HttpResponseMessage { StatusCode = statusCode, Content = new StringContent(content) }
            : new HttpResponseMessage { StatusCode = statusCode };
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        RequestsMade.Add(request);

        return Task.FromResult(_httpResponseMessage);
    }
}