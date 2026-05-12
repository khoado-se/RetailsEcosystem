using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace RetailsEcosystem.Customer.IntegrationTests.Categories;

public class CategoriesTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CategoriesTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCategories_Anonymous_Returns200WithPagedShape()
    {
        var response = await _client.GetAsync("/api/categories");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("items");
        body.Should().Contain("totalPage");
    }

    [Fact]
    public async Task GetCategories_WithPagination_Returns200()
    {
        var response = await _client.GetAsync("/api/categories?pageNumber=1&pageSize=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PostCategory_Unauthenticated_Returns401()
    {
        var dto = new { Name = "Test Category", Description = "desc" };

        var response = await _client.PostAsJsonAsync("/api/categories", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteCategory_Unauthenticated_Returns401()
    {
        var response = await _client.DeleteAsync("/api/categories/1");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
