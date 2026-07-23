using FluentAssertions;
using InventoryManagement.Application.Common;
using InventoryManagement.Application.Common.Exceptions;
using InventoryManagement.Application.DTOs.Products;
using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Application.Services;
using InventoryManagement.Domain.Entities;
using Moq;

namespace InventoryManagement.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<ICategoryRepository> _categoryRepository = new();
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _productService = new ProductService(_productRepository.Object, _categoryRepository.Object);
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldReturnProduct_WhenProductExists()
    {
        var product = CreateProduct(id: 1, name: "Wireless Mouse");
        _productRepository.Setup(repository => repository.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(product);

        var result = await _productService.GetProductByIdAsync(1);

        result.Id.Should().Be(1);
        result.Name.Should().Be("Wireless Mouse");
        result.CategoryName.Should().Be("Electronics");
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldThrowNotFound_WhenProductDoesNotExist()
    {
        _productRepository.Setup(repository => repository.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);
        var act = async () => await _productService.GetProductByIdAsync(99);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateProductAsync_ShouldThrowNotFound_WhenCategoryDoesNotExist()
    {
        _categoryRepository.Setup(repository => repository.ExistsAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var act = async () => await _productService.CreateProductAsync(CreateProductRequest(categoryId: 5));
        await act.Should().ThrowAsync<NotFoundException>();
        _productRepository.Verify(repository => repository.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateProductAsync_ShouldThrowConflict_WhenNameAlreadyExists()
    {
        _categoryRepository.Setup(repository => repository.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _productRepository.Setup(repository => repository.ExistsWithNameAsync( "Wireless Mouse", null, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var act = async () => await _productService.CreateProductAsync(CreateProductRequest(name: "Wireless Mouse"));

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task CreateProductAsync_ShouldPersistProduct_WhenRequestIsValid()
    {
        _categoryRepository.Setup(repository => repository.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _productRepository.Setup(repository => repository.ExistsWithNameAsync( It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _productRepository.Setup(repository => repository.GetByIdAsync( It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(CreateProduct(id: 10, name: "Keyboard"));

        var result = await _productService.CreateProductAsync(CreateProductRequest(name: "Keyboard"));

        result.Name.Should().Be("Keyboard");
        _productRepository.Verify(repository => repository.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),Times.Once);
        _productRepository.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()),Times.Once);
    }

    [Fact]
    public async Task UpdateProductAsync_ShouldExcludeItself_WhenCheckingNameUniqueness()
    {
        _productRepository.Setup(repository => repository.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(CreateProduct(id: 1, name: "Wireless Mouse"));
        _categoryRepository.Setup(repository => repository.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _productRepository.Setup(repository => repository.ExistsWithNameAsync( It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        await _productService.UpdateProductAsync(1, new UpdateProductDto("Wireless Mouse", null, 30m, 5, 1));

        _productRepository.Verify(repository => repository.ExistsWithNameAsync("Wireless Mouse", 1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProductAsync_ShouldMarkProductAsDeleted_InsteadOfRemovingIt()
    {
        var product = CreateProduct(id: 1, name: "Wireless Mouse");
        _productRepository.Setup(repository => repository.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(product);

        await _productService.DeleteProductAsync(1);

        product.IsDeleted.Should().BeTrue();
        _productRepository.Verify(repository => repository.Update(product), Times.Once);
        _productRepository.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()),Times.Once);
    }

    [Fact]
    public async Task GetProductsAsync_ShouldPreservePagingInformation()
    {
        var products = new List<Product> { CreateProduct(1, "Mouse"), CreateProduct(2, "Keyboard") };
        _productRepository.Setup(repository => repository.GetPagedAsync(
                It.IsAny<ProductQueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Product>(products, totalCount: 25, pageNumber: 2, pageSize: 10));

        var result = await _productService.GetProductsAsync(new ProductQueryParameters());

        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(25);
        result.TotalPages.Should().Be(3);
        result.HasNextPage.Should().BeTrue();
    }

    private static Product CreateProduct(int id, string name) => new()
    {
        Id = id, Name = name, Price = 25m, Quantity = 10,CategoryId = 1, Category = new Category { Id = 1, Name = "Electronics" }
    };

    private static CreateProductDto CreateProductRequest(string name = "New Product", int categoryId = 1) =>
        new(name, "Description", 25m, 10, categoryId);
}