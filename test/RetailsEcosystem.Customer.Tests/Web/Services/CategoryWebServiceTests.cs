using System.Linq;
using System.Net;
using System.Text.Json;
using FluentAssertions;
using Moq;
using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Web.Services;

namespace RetailsEcosystem.Customer.Tests.Web.Services;

public class CategoryWebServiceTests
{
    private static readonly JsonSerializerOptions JsonOpts =
        new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private static CategoryService Build(HttpStatusCode status, object? body)
    {
        var json    = JsonSerializer.Serialize(body, JsonOpts);
        var handler = new FakeHttpHandler(status, json);
        var client  = new HttpClient(handler) { BaseAddress = new Uri("https://localhost/") };
        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(f => f.CreateClient("MyApi")).Returns(client);
        return new CategoryService(factory.Object);
    }

    private static PagedResult<CategoryDto> PagedCategories() => new()
    {
        Items      = [new CategoryDto { Id = 1, Name = "Electronics" }],
        TotalCount = 1,
        PageNumber = 1,
        PageSize   = 20
    };

    // ── GetAllAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAll_WithPagedRequest_ReturnsPagedResult()
    {
        var svc = Build(HttpStatusCode.OK, PagedCategories());

        var result = await svc.GetAllAsync(new PagedRequest { PageNumber = 1, PageSize = 20 });

        result.Items.Should().HaveCount(1);
        result.Items.First().Name.Should().Be("Electronics");
    }

    [Fact]
    public async Task GetAll_NullPagedRequest_ReturnsPagedResult()
    {
        var svc = Build(HttpStatusCode.OK, PagedCategories());

        var result = await svc.GetAllAsync(null);

        result.Should().NotBeNull();
    }

    // ── GetByIdAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetById_200_ReturnsCategoryDto()
    {
        var dto = new CategoryDto { Id = 5, Name = "Books" };
        var svc = Build(HttpStatusCode.OK, dto);

        var result = await svc.GetByIdAsync(5);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Books");
    }

    [Fact]
    public async Task GetById_404_ReturnsNull()
    {
        var svc = Build(HttpStatusCode.NotFound, (object?)null);

        var act = async () => await svc.GetByIdAsync(999);

        await act.Should().ThrowAsync<HttpRequestException>();
    }

    private sealed class FakeHttpHandler(HttpStatusCode status, string body) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken ct)
            => Task.FromResult(new HttpResponseMessage(status)
            {
                Content = new System.Net.Http.StringContent(body, System.Text.Encoding.UTF8, "application/json")
            });
    }
}
