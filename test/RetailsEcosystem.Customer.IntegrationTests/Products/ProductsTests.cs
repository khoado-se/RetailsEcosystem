using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace RetailsEcosystem.Customer.IntegrationTests.Products;

public class ProductsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_Anonymous_Returns200WithPagedShape()
    {
        var response = await _client.GetAsync("/api/products");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("items");
        body.Should().Contain("totalPage");
    }

    [Fact]
    public async Task GetProducts_WithPagination_Returns200()
    {
        var response = await _client.GetAsync("/api/products?pageNumber=1&pageSize=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProduct_NonExistentId_Returns404()
    {
        var response = await _client.GetAsync("/api/products/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetFeaturedProducts_Anonymous_Returns200WithPagedShape()
    {
        var response = await _client.GetAsync("/api/products/featured");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("items");
    }

    [Fact]
    public async Task PostProduct_Unauthenticated_Returns401()
    {
        var dto = new
        {
            Name = "Test Product",
            Price = 100,
            CategoryId = 1,
            IsFeatured = false
        };

        var response = await _client.PostAsJsonAsync("/api/products", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteProduct_Unauthenticated_Returns401()
    {
        var response = await _client.DeleteAsync("/api/products/1");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
