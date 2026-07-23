using InventoryManagement.Application.Common;
using InventoryManagement.Application.DTOs.Products;

namespace InventoryManagement.Application.Interfaces.Services;

public interface IProductService
{
    Task<PagedResult<ProductDto>> GetProductsAsync(ProductQueryParameters parameters, CancellationToken cancellationToken = default);
    Task<ProductDto> GetProductByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ProductDto> CreateProductAsync(CreateProductDto dto, CancellationToken cancellationToken = default);
    Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto dto, CancellationToken cancellationToken = default);
    Task DeleteProductAsync(int id, CancellationToken cancellationToken = default);
}