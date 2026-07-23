using InventoryManagement.Application.Common;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<PagedResult<Product>> GetPagedAsync(ProductQueryParameters parameters, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsWithNameAsync(string name, int? excludedProductId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    void Update(Product product);
    Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetTotalQuantityAsync(CancellationToken cancellationToken = default);
    Task<decimal> GetAveragePriceAsync(CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}