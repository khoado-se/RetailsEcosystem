using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Moq;
using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs.Order;
using RetailsEcosystem.Customer.Shared.Enums;
using RetailsEcosystem.Customer.Web.Services;

namespace RetailsEcosystem.Customer.Tests.Web.Services;

public class OrderWebServiceTests
{
    private static readonly JsonSerializerOptions JsonOpts =
        new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private static OrderService Build(HttpStatusCode status, object? body)
    {
        var json    = body is null ? "null" : JsonSerializer.Serialize(body, JsonOpts);
        var handler = new FakeHttpHandler(status, json);
        var client  = new HttpClient(handler) { BaseAddress = new Uri("https://localhost/") };
        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(f => f.CreateClient("MyApi")).Returns(client);
        return new OrderService(factory.Object);
    }

    private static OrderDto SampleOrder() => new()
    {
        Id            = 1,
        UserId        = "u1",
        TotalAmount   = 50m,
        ShippingAddress = "123 Street",
        Status        = OrderStatus.Pending
    };

    // ── CreateOrderAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task CreateOrder_200_ReturnsOrderDto()
    {
        var svc = Build(HttpStatusCode.OK, SampleOrder());

        var result = await svc.CreateOrderAsync("tok", new CreateOrderDto
        {
            ShippingAddress = "123 Street",
            PaymentMethod   = PaymentMethod.COD
        });

        result.Id.Should().Be(1);
        result.TotalAmount.Should().Be(50m);
    }

    [Fact]
    public async Task CreateOrder_BadRequest_Throws()
    {
        var svc = Build(HttpStatusCode.BadRequest, null);

        var act = async () => await svc.CreateOrderAsync("tok", new CreateOrderDto());

        await act.Should().ThrowAsync<HttpRequestException>();
    }

    // ── GetOrdersAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task GetOrders_200_ReturnsPagedResult()
    {
        var paged = new PagedResult<OrderDto>
        {
            Items      = [SampleOrder()],
            TotalCount = 1,
            PageNumber = 1,
            PageSize   = 10
        };
        var svc = Build(HttpStatusCode.OK, paged);

        var result = await svc.GetOrdersAsync("tok", 1, 10);

        result.Items.Should().HaveCount(1);
    }

    // ── GetOrderByIdAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetOrderById_200_ReturnsOrderDto()
    {
        var svc = Build(HttpStatusCode.OK, SampleOrder());

        var result = await svc.GetOrderByIdAsync("tok", 1);

        result.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetOrderById_404_Throws()
    {
        var svc = Build(HttpStatusCode.NotFound, null);

        var act = async () => await svc.GetOrderByIdAsync("tok", 999);

        await act.Should().ThrowAsync<HttpRequestException>();
    }

    // ── CancelOrderAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task CancelOrder_200_ReturnsOrderDto()
    {
        var svc = Build(HttpStatusCode.OK, SampleOrder());

        var result = await svc.CancelOrderAsync("tok", 1);

        result.Id.Should().Be(1);
    }

    // ── InitiatePaymentAsync ──────────────────────────────────────────────────

    [Fact]
    public async Task InitiatePayment_200_ReturnsPaymentUrl()
    {
        var result = new InitiatePaymentResult { PaymentUrl = "https://pay.vnpay.vn/abc", TxnRef = "1-xyz" };
        var svc    = Build(HttpStatusCode.OK, result);

        var payment = await svc.InitiatePaymentAsync("tok", 1);

        payment.PaymentUrl.Should().Be("https://pay.vnpay.vn/abc");
    }

    // ── ConfirmReturnAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task ConfirmReturn_200_NoException()
    {
        var svc = Build(HttpStatusCode.OK, new { });

        var act = async () => await svc.ConfirmReturnAsync("tok",
            new Dictionary<string, string> { ["vnp_ResponseCode"] = "00" });

        await act.Should().NotThrowAsync();
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
