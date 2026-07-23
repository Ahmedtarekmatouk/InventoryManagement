using System.Linq.Expressions;
using InventoryManagement.Application.Common;
using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private static readonly Dictionary<string, Expression<Func<Product, object>>> SortSelectors =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = product => product.Name,
            ["price"] = product => product.Price,
            ["quantity"] = product => product.Quantity,
            ["createdAt"] = product => product.CreatedAt
        };

    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Product>> GetPagedAsync(
        ProductQueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Products
            .AsNoTracking()
            .Include(product => product.Category)
            .AsQueryable();

        query = ApplySearchFilter(query, parameters.SearchTerm);
        query = ApplyCategoryFilter(query, parameters.CategoryId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await ApplySorting(query, parameters)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Product>(items, totalCount, parameters.PageNumber, parameters.PageSize);
    }

    public Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Products
            .Include(product => product.Category)
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);

    public Task<bool> ExistsWithNameAsync(
        string name,
        int? excludedProductId = null,
        CancellationToken cancellationToken = default) =>
        _context.Products.AnyAsync(
            product => product.Name == name && product.Id != excludedProductId,
            cancellationToken);

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default) =>
        await _context.Products.AddAsync(product, cancellationToken);

    public void Update(Product product) => _context.Products.Update(product);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);

    private static IQueryable<Product> ApplySearchFilter(IQueryable<Product> query, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return query;
        }

        var normalizedTerm = searchTerm.Trim();

        return query.Where(product =>
            product.Name.Contains(normalizedTerm) ||
            (product.Description != null && product.Description.Contains(normalizedTerm)));
    }

    private static IQueryable<Product> ApplyCategoryFilter(IQueryable<Product> query, int? categoryId) =>
        categoryId.HasValue
            ? query.Where(product => product.CategoryId == categoryId.Value)
            : query;

    private static IQueryable<Product> ApplySorting(IQueryable<Product> query, ProductQueryParameters parameters)
    {
        var selector = SortSelectors.GetValueOrDefault(parameters.SortBy ?? string.Empty)
                       ?? SortSelectors["name"];

        return parameters.SortDescending
            ? query.OrderByDescending(selector)
            : query.OrderBy(selector);
    }
}