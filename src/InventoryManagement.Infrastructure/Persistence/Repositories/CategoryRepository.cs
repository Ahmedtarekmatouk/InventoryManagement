using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Persistence.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Category>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.Categories
            .AsNoTracking()
            .Include(category => category.Products)
            .OrderBy(category => category.Name)
            .ToListAsync(cancellationToken);

    public Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Categories
            .Include(category => category.Products)
            .FirstOrDefaultAsync(category => category.Id == id, cancellationToken);

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Categories.AnyAsync(category => category.Id == id, cancellationToken);

    public Task<bool> ExistsWithNameAsync(
        string name,
        int? excludedCategoryId = null,
        CancellationToken cancellationToken = default) =>
        _context.Categories.AnyAsync(
            category => category.Name == name && category.Id != excludedCategoryId,
            cancellationToken);

    public Task<bool> HasProductsAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Products.AnyAsync(product => product.CategoryId == id, cancellationToken);

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default) =>
        await _context.Categories.AddAsync(category, cancellationToken);

    public void Update(Category category) => _context.Categories.Update(category);

    public void Remove(Category category) => _context.Categories.Remove(category);

    public Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default) =>
        _context.Categories.CountAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}