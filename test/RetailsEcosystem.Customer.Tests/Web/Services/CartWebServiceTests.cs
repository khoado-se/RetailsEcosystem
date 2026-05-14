using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Moq;
using RetailsEcosystem.Customer.Shared.DTOs.Cart;
using RetailsEcosystem.Customer.Web.Services;

namespace RetailsEcosystem.Customer.Tests.Web.Services;

public class CartWebServiceTests
{
    private static readonly JsonSerializerOptions JsonOpts =
        new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private static CartService BuildService(HttpStatusCode status, object? body)
    {
        var json = body is string s ? s : JsonSerializer.Serialize(body, JsonOpts);
        var handler = new FakeHttpHandler(status, json);
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://localhost/") };
        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(f => f.CreateClient("MyApi")).Returns(client);
        return new CartService(factory.Object);
    }

    private static CartDto EmptyCart() => new() { Items = [] };

    private static CartDto CartWithItem() => new()
    {
        Items = [new CartItemDto { Id = 1, ProductId = 10, ProductName = "Widget", Quantity = 2, UnitPrice = 5m }]
    };

    // ── GetCartAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetCart_200_ReturnsCartDto()
    {
        var svc = BuildService(HttpStatusCode.OK, EmptyCart());

        var result = await svc.GetCartAsync("access-token");

        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCart_401_ThrowsHttpRequestException()
    {
        var svc = BuildService(HttpStatusCode.Unauthorized, "{}");

        var act = () => svc.GetCartAsync("bad-token");

        await act.Should().ThrowAsync<HttpRequestException>();
    }

    // ── AddItemAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task AddItem_200_ReturnsUpdatedCart()
    {
        var svc = BuildService(HttpStatusCode.OK, CartWithItem());

        var result = await svc.AddItemAsync("access-token", 10, 2);

        result.Items.Should().HaveCount(1);
        result.Items[0].ProductId.Should().Be(10);
    }

    [Fact]
    public async Task AddItem_400_ThrowsHttpRequestException()
    {
        var svc = BuildService(HttpStatusCode.BadRequest, "{}");

        var act = () => svc.AddItemAsync("access-token", 99, 100);

        await act.Should().ThrowAsync<HttpRequestException>();
    }

    // ── UpdateItemAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateItem_200_ReturnsUpdatedCart()
    {
        var cart = CartWithItem();
        cart.Items[0].Quantity = 5;
        var svc = BuildService(HttpStatusCode.OK, cart);

        var result = await svc.UpdateItemAsync("access-token", 1, 5);

        result.Items[0].Quantity.Should().Be(5);
    }

    [Fact]
    public async Task UpdateItem_400_ThrowsHttpRequestException()
    {
        var svc = BuildService(HttpStatusCode.BadRequest, "{}");

        var act = () => svc.UpdateItemAsync("access-token", 1, 999);

        await act.Should().ThrowAsync<HttpRequestException>();
    }

    // ── RemoveItemAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task RemoveItem_200_ReturnsCartDto()
    {
        var svc = BuildService(HttpStatusCode.OK, EmptyCart());

        var result = await svc.RemoveItemAsync("access-token", 1);

        result.Items.Should().BeEmpty();
    }

    // ── ClearCartAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task ClearCart_200_DoesNotThrow()
    {
        var svc = BuildService(HttpStatusCode.OK, "{}");

        var act = () => svc.ClearCartAsync("access-token");

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ClearCart_401_ThrowsHttpRequestException()
    {
        var svc = BuildService(HttpStatusCode.Unauthorized, "{}");

        var act = () => svc.ClearCartAsync("bad-token");

        await act.Should().ThrowAsync<HttpRequestException>();
    }

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
