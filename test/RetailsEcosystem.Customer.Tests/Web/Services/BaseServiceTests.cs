using System.Net;
using System.Text;
using FluentAssertions;
using Moq;
using RetailsEcosystem.Customer.Web.Services;

namespace RetailsEcosystem.Customer.Tests.Web.Services;

public class BaseServiceTests
{
    private sealed class ConcreteService : BaseService
    {
        public ConcreteService(IHttpClientFactory factory) : base(factory) { }

        public Task<T> InvokeSend<T>(HttpRequestMessage request) => SendAsync<T>(request);
    }

    private static ConcreteService BuildService(HttpStatusCode status, string json)
    {
        var handler = new FakeHttpHandler(status, json);
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://localhost/") };
        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(f => f.CreateClient("MyApi")).Returns(client);
        return new ConcreteService(factory.Object);
    }

    [Fact]
    public async Task SendAsync_200Ok_DeserializesResponse()
    {
        var svc = BuildService(HttpStatusCode.OK, "{\"value\":42}");

        var result = await svc.InvokeSend<TestPayload>(
            new HttpRequestMessage(HttpMethod.Get, "api/test"));

        result.Value.Should().Be(42);
    }

    [Fact]
    public async Task SendAsync_404_ThrowsHttpRequestException()
    {
        var svc = BuildService(HttpStatusCode.NotFound, "{}");

        var act = () => svc.InvokeSend<TestPayload>(
            new HttpRequestMessage(HttpMethod.Get, "api/test"));

        await act.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task SendAsync_401_ThrowsHttpRequestException()
    {
        var svc = BuildService(HttpStatusCode.Unauthorized, "{}");

        var act = () => svc.InvokeSend<TestPayload>(
            new HttpRequestMessage(HttpMethod.Get, "api/test"));

        await act.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task SendAsync_InvalidJson_ThrowsException()
    {
        var svc = BuildService(HttpStatusCode.OK, "NOT JSON");

        var act = () => svc.InvokeSend<TestPayload>(
            new HttpRequestMessage(HttpMethod.Get, "api/test"));

        await act.Should().ThrowAsync<Exception>();
    }

    private record TestPayload(int Value);

    private sealed class FakeHttpHandler(HttpStatusCode status, string body) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken ct)
            => Task.FromResult(new HttpResponseMessage(status)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            });
    }
}
