using InventoryManagement.Application.DTOs.Categories;

namespace InventoryManagement.Application.Services;

public interface ICategoryService
{
    Task<IReadOnlyCollection<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<CategoryDto> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default);
    Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto dto, CancellationToken cancellationToken = default);
    Task DeleteCategoryAsync(int id, CancellationToken cancellationToken = default);
}