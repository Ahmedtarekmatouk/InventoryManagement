using InventoryManagement.Application.Common.Exceptions;
using InventoryManagement.Application.DTOs.Categories;
using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Application.Mappings;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IReadOnlyCollection<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);

        return categories.Select(category => category.ToDto()).ToList();
    }

    public async Task<CategoryDto> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await GetExistingCategoryAsync(id, cancellationToken);
        return category.ToDto();
    }

    public async Task<CategoryDto> CreateCategoryAsync(
        CreateCategoryDto dto,
        CancellationToken cancellationToken = default)
    {
        await EnsureCategoryNameIsUniqueAsync(dto.Name, null, cancellationToken);

        var category = dto.ToEntity();

        await _categoryRepository.AddAsync(category, cancellationToken);
        await _categoryRepository.SaveChangesAsync(cancellationToken);

        return category.ToDto();
    }

    public async Task<CategoryDto> UpdateCategoryAsync(
        int id,
        UpdateCategoryDto dto,
        CancellationToken cancellationToken = default)
    {
        var category = await GetExistingCategoryAsync(id, cancellationToken);

        await EnsureCategoryNameIsUniqueAsync(dto.Name, id, cancellationToken);

        category.ApplyChanges(dto);

        _categoryRepository.Update(category);
        await _categoryRepository.SaveChangesAsync(cancellationToken);

        return category.ToDto();
    }

    public async Task DeleteCategoryAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await GetExistingCategoryAsync(id, cancellationToken);

        if (await _categoryRepository.HasProductsAsync(id, cancellationToken))
        {
            throw new ConflictException("A category that still contains products cannot be deleted.");
        }

        _categoryRepository.Remove(category);
        await _categoryRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task<Category> GetExistingCategoryAsync(int id, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);

        return category ?? throw new NotFoundException(nameof(Category), id);
    }

    private async Task EnsureCategoryNameIsUniqueAsync(
        string name,
        int? excludedCategoryId,
        CancellationToken cancellationToken)
    {
        if (await _categoryRepository.ExistsWithNameAsync(name, excludedCategoryId, cancellationToken))
        {
            throw new ConflictException($"A category named '{name}' already exists.");
        }
    }
}