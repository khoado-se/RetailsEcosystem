using System.Net;
using System.Text.Json;
using FluentAssertions;
using Moq;
using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Web.Services;

namespace RetailsEcosystem.Customer.Tests.Web.Services;

public class ProductWebServiceTests
{
    private static readonly JsonSerializerOptions JsonOpts =
        new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private static (ProductService svc, FakeHttpHandler handler) Build(HttpStatusCode status, object? body)
    {
        var json    = JsonSerializer.Serialize(body, JsonOpts);
        var handler = new FakeHttpHandler(status, json);
        var client  = new HttpClient(handler) { BaseAddress = new Uri("https://localhost/") };
        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(f => f.CreateClient("MyApi")).Returns(client);
        return (new ProductService(factory.Object), handler);
    }

    private sealed class FakeHttpHandler(HttpStatusCode status, string body) : HttpMessageHandler
    {
        public string? LastRequestUri { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken ct)
        {
            LastRequestUri = request.RequestUri?.ToString();
            return Task.FromResult(new HttpResponseMessage(status)
            {
                Content = new System.Net.Http.StringContent(body, System.Text.Encoding.UTF8, "application/json")
            });
        }
    }

    private static PagedResult<ProductDto> PagedProducts() => new()
    {
        Items      = [new ProductDto { Id = 1, Name = "Widget", Price = 9.99m }],
        TotalCount = 1,
        PageNumber = 1,
        PageSize   = 10
    };

    // ── GetAllAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAll_NoFilters_ReturnsPagedResult()
    {
        var (svc, _) = Build(HttpStatusCode.OK, PagedProducts());

        var result = await svc.GetAllAsync(new PagedRequest { PageNumber = 1, PageSize = 10 }, null, null);

        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAll_WithCategoryId_IncludesCategoryParam()
    {
        var (svc, handler) = Build(HttpStatusCode.OK, PagedProducts());

        await svc.GetAllAsync(new PagedRequest { PageNumber = 1, PageSize = 10 }, categoryId: 3, null);

        handler.LastRequestUri.Should().Contain("categoryId=3");
    }

    [Fact]
    public async Task GetAll_WithSearch_IncludesSearchParam()
    {
        var (svc, handler) = Build(HttpStatusCode.OK, PagedProducts());

        await svc.GetAllAsync(new PagedRequest { PageNumber = 1, PageSize = 10 }, null, search: "widget");

        handler.LastRequestUri.Should().Contain("search=widget");
    }

    [Fact]
    public async Task GetAll_EmptySearch_NoSearchParam()
    {
        var (svc, handler) = Build(HttpStatusCode.OK, PagedProducts());

        await svc.GetAllAsync(new PagedRequest { PageNumber = 1, PageSize = 10 }, null, search: "");

        handler.LastRequestUri.Should().NotContain("search=");
    }

    // ── GetByIdAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetById_200_ReturnsProductDto()
    {
        var dto      = new ProductDto { Id = 42, Name = "Gadget" };
        var (svc, _) = Build(HttpStatusCode.OK, dto);

        var result = await svc.GetByIdAsync(42);

        result.Name.Should().Be("Gadget");
    }

    // ── GetFeaturedProductsAsync ──────────────────────────────────────────────

    [Fact]
    public async Task GetFeatured_ReturnsPagedResult()
    {
        var (svc, handler) = Build(HttpStatusCode.OK, PagedProducts());

        var result = await svc.GetFeaturedProductsAsync(new PagedRequest { PageNumber = 1, PageSize = 6 });

        result.Items.Should().HaveCount(1);
        handler.LastRequestUri.Should().Contain("featured");
    }
}
