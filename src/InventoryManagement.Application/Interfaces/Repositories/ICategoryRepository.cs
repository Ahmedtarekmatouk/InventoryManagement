using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<IReadOnlyCollection<Category>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsWithNameAsync(string name, int? excludedCategoryId = null, CancellationToken cancellationToken = default);
    Task<bool> HasProductsAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(Category category, CancellationToken cancellationToken = default);
    void Update(Category category);
    void Remove(Category category);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}