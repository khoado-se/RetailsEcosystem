using FluentAssertions;
using Moq;
using RetailsEcosystem.Customer.Application.Services;
using RetailsEcosystem.Customer.Domain.Interface;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Shared.DTOs.Category;
using RetailsEcosystem.Customer.Tests.Helpers.Builders;

namespace RetailsEcosystem.Customer.Tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _categoryRepoMock = new();
    private readonly CategoryService _sut;

    public CategoryServiceTests()
    {
        _sut = new CategoryService(_categoryRepoMock.Object);
    }

    // ── GetAllAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_WhenCalled_ReturnsPagedResultWithCorrectCounts()
    {
        var categories = new[]
        {
            new CategoryBuilder().WithId(1).WithName("Electronics").WithDescription("Devices").Build(),
            new CategoryBuilder().WithId(2).WithName("Books").WithDescription("Literature").Build()
        };
        _categoryRepoMock.Setup(r => r.GetAllAsync(1, 10)).ReturnsAsync(categories);
        _categoryRepoMock.Setup(r => r.GetTotalCategoriesAsync()).ReturnsAsync(2);

        var result = await _sut.GetAllAsync(new PagedRequest { PageNumber = 1, PageSize = 10 });

        result.Items.Should().HaveCount(2);
        result.TotalPage.Should().Be(1);
        result.PageNumber.Should().Be(1);
    }

    [Fact]
    public async Task GetAllAsync_MultiplePages_ComputesTotalPagesCorrectly()
    {
        _categoryRepoMock.Setup(r => r.GetAllAsync(1, 10)).ReturnsAsync([]);
        _categoryRepoMock.Setup(r => r.GetTotalCategoriesAsync()).ReturnsAsync(25);

        var result = await _sut.GetAllAsync(new PagedRequest { PageNumber = 1, PageSize = 10 });

        result.TotalPage.Should().Be(3);
    }

    // ── CreateAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_ValidDto_ReturnsCreatedCategoryDto()
    {
        var dto = new CreateCategoryDto { Name = "Electronics", Description = "Devices" };
        _categoryRepoMock.Setup(r => r.CreateAsync(It.IsAny<Domain.Entities.Category>())).ReturnsAsync(10);

        var result = await _sut.CreateAsync(dto);

        result.Id.Should().Be(10);
        result.Name.Should().Be("Electronics");
        result.Description.Should().Be("Devices");
    }

    // ── UpdateAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_CallsRepositoryWithCorrectEntity()
    {
        var dto = new UpdateCategoryDto { Id = 5, Name = "Updated", Description = "New desc" };

        await _sut.UpdateAsync(dto);

        _categoryRepoMock.Verify(r => r.UpdateAsync(
            It.Is<Domain.Entities.Category>(c => c.Id == 5 && c.Name == "Updated" && c.Description == "New desc")),
            Times.Once);
    }

    // ── DeleteAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_WhenCalled_CallsRepositoryWithCorrectId()
    {
        await _sut.DeleteAsync(7);

        _categoryRepoMock.Verify(r => r.DeleteAsync(7), Times.Once);
    }

    // ── Null-description branches ─────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_NullDescription_MapsToEmptyString()
    {
        var dto = new CreateCategoryDto { Name = "Empty Desc", Description = null };
        _categoryRepoMock.Setup(r => r.CreateAsync(It.IsAny<Domain.Entities.Category>())).ReturnsAsync(1);

        var result = await _sut.CreateAsync(dto);

        result.Description.Should().Be(string.Empty);
    }

    [Fact]
    public async Task UpdateAsync_NullDescription_MapsToEmptyString()
    {
        var dto = new UpdateCategoryDto { Id = 3, Name = "Test", Description = null };

        var result = await _sut.UpdateAsync(dto);

        result.Description.Should().BeNull();
        _categoryRepoMock.Verify(r => r.UpdateAsync(
            It.Is<Domain.Entities.Category>(c => c.Description == string.Empty)), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_CategoryWithNullDescription_MapsToEmptyString()
    {
        var categories = new[]
        {
            new CategoryBuilder().WithId(1).WithName("NullDesc").WithDescription(null!).Build()
        };
        _categoryRepoMock.Setup(r => r.GetAllAsync(1, 10)).ReturnsAsync(categories);
        _categoryRepoMock.Setup(r => r.GetTotalCategoriesAsync()).ReturnsAsync(1);

        var result = await _sut.GetAllAsync(new PagedRequest { PageNumber = 1, PageSize = 10 });

        result.Items.First().Description.Should().Be(string.Empty);
    }
}
