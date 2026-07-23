using FluentAssertions;
using InventoryManagement.Application.Common.Exceptions;
using InventoryManagement.Application.DTOs.Categories;
using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Application.Services;
using InventoryManagement.Domain.Entities;
using Moq;

namespace InventoryManagement.Tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _categoryRepository = new();
    private readonly CategoryService _CategoryService;

    public CategoryServiceTests()
    {
        _CategoryService = new CategoryService(_categoryRepository.Object);
    }

    [Fact]
    public async Task DeleteCategoryAsync_ShouldThrowConflict_WhenCategoryStillContainsProducts()
    {
        _categoryRepository.Setup(repository => repository.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(new Category { Id = 1, Name = "Electronics" });
        _categoryRepository.Setup(repository => repository.HasProductsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var act = async () => await _CategoryService.DeleteCategoryAsync(1);

        await act.Should().ThrowAsync<ConflictException>();
        _categoryRepository.Verify(repository => repository.Remove(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public async Task DeleteCategoryAsync_ShouldRemoveCategory_WhenItHasNoProducts()
    {
        var category = new Category { Id = 1, Name = "Electronics" };
        _categoryRepository.Setup(repository => repository.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(category);
        _categoryRepository.Setup(repository => repository.HasProductsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        await _CategoryService.DeleteCategoryAsync(1);
        _categoryRepository.Verify(repository => repository.Remove(category), Times.Once);
    }

    [Fact]
    public async Task CreateCategoryAsync_ShouldThrowConflict_WhenNameAlreadyExists()
    {
        _categoryRepository.Setup(repository => repository.ExistsWithNameAsync( "Electronics", null, It.IsAny<CancellationToken>())) .ReturnsAsync(true);
        var act = async () => await _CategoryService.CreateCategoryAsync(new CreateCategoryDto("Electronics", null));
        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task GetCategoryByIdAsync_ShouldThrowNotFound_WhenCategoryDoesNotExist()
    {
        _categoryRepository.Setup(repository => repository.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Category?)null);
        var act = async () => await _CategoryService.GetCategoryByIdAsync(99);
        await act.Should().ThrowAsync<NotFoundException>();
    }
}