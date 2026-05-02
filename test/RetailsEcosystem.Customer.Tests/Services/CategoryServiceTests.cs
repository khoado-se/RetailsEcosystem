using FluentAssertions;
using Moq;
using RetailsEcosystem.Customer.Application.Services;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Domain.Interface;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Shared.DTOs.Category;

namespace RetailsEcosystem.Customer.Tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _categoryRepoMock = new();
    private readonly CategoryService _sut;

    public CategoryServiceTests()
    {
        _sut = new CategoryService(_categoryRepoMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPagedResultWithCorrectCounts()
    {
        var categories = new[]
        {
            new Category { Id = 1, Name = "Electronics", Description = "Devices" },
            new Category { Id = 2, Name = "Books", Description = "Literature" }
        };
        _categoryRepoMock.Setup(r => r.GetAllAsync(1, 10)).ReturnsAsync(categories);
        _categoryRepoMock.Setup(r => r.GetTotalCategoriesAsync()).ReturnsAsync(2);

        var result = await _sut.GetAllAsync(new PagedRequest { PageNumber = 1, PageSize = 10 });

        result.Items.Should().HaveCount(2);
        result.TotalPage.Should().Be(1);
        result.PageNumber.Should().Be(1);
    }

    [Fact]
    public async Task UpdateAsync_CallsRepositoryWithCorrectEntity()
    {
        var dto = new UpdateCategoryDto { Id = 5, Name = "Updated", Description = "New desc" };

        await _sut.UpdateAsync(dto);

        _categoryRepoMock.Verify(r => r.UpdateAsync(
            It.Is<Category>(c => c.Id == 5 && c.Name == "Updated" && c.Description == "New desc")),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_DelegatesWithCorrectId()
    {
        await _sut.DeleteAsync(7);

        _categoryRepoMock.Verify(r => r.DeleteAsync(7), Times.Once);
    }
}
