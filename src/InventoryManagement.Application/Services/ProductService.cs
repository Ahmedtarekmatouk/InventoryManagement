using InventoryManagement.Application.Common;
using InventoryManagement.Application.Common.Exceptions;
using InventoryManagement.Application.DTOs.Products;
using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Application.Interfaces.Services;
using InventoryManagement.Application.Mappings;
using InventoryManagement.Domain.Entities;
namespace InventoryManagement.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<PagedResult<ProductDto>> GetProductsAsync(
        ProductQueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var pagedProducts = await _productRepository.GetPagedAsync(parameters, cancellationToken);

        var items = pagedProducts.Items
            .Select(product => product.ToDto())
            .ToList();

        return new PagedResult<ProductDto>(
            items,
            pagedProducts.TotalCount,
            pagedProducts.PageNumber,
            pagedProducts.PageSize);
    }

    public async Task<ProductDto> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await GetExistingProductAsync(id, cancellationToken);
        return product.ToDto();
    }

    public async Task<ProductDto> CreateProductAsync(
        CreateProductDto dto,
        CancellationToken cancellationToken = default)
    {
        await EnsureCategoryExistsAsync(dto.CategoryId, cancellationToken);
        await EnsureProductNameIsUniqueAsync(dto.Name, null, cancellationToken);

        var product = dto.ToEntity();

        await _productRepository.AddAsync(product, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return await GetProductByIdAsync(product.Id, cancellationToken);
    }

    public async Task<ProductDto> UpdateProductAsync(
        int id,
        UpdateProductDto dto,
        CancellationToken cancellationToken = default)
    {
        var product = await GetExistingProductAsync(id, cancellationToken);

        await EnsureCategoryExistsAsync(dto.CategoryId, cancellationToken);
        await EnsureProductNameIsUniqueAsync(dto.Name, id, cancellationToken);

        product.ApplyChanges(dto);

        _productRepository.Update(product);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return await GetProductByIdAsync(product.Id, cancellationToken);
    }

    public async Task DeleteProductAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await GetExistingProductAsync(id, cancellationToken);

        product.IsDeleted = true;

        _productRepository.Update(product);
        await _productRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task<Product> GetExistingProductAsync(int id, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);

        return product ?? throw new NotFoundException(nameof(Product), id);
    }

    private async Task EnsureCategoryExistsAsync(int categoryId, CancellationToken cancellationToken)
    {
        if (!await _categoryRepository.ExistsAsync(categoryId, cancellationToken))
        {
            throw new NotFoundException(nameof(Category), categoryId);
        }
    }

    private async Task EnsureProductNameIsUniqueAsync(
        string name,
        int? excludedProductId,
        CancellationToken cancellationToken)
    {
        if (await _productRepository.ExistsWithNameAsync(name, excludedProductId, cancellationToken))
        {
            throw new ConflictException($"A product named '{name}' already exists.");
        }
    }
}