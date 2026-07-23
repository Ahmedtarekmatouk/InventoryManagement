using InventoryManagement.Application.DTOs.Categories;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Mappings;

public static class CategoryMappingExtensions
{
    public static CategoryDto ToDto(this Category category) =>
        new(
            category.Id,
            category.Name,
            category.Description,
            category.Products.Count,
            category.CreatedAt);

    public static Category ToEntity(this CreateCategoryDto dto) =>
        new()
        {
            Name = dto.Name,
            Description = dto.Description
        };

    public static void ApplyChanges(this Category category, UpdateCategoryDto dto)
    {
        category.Name = dto.Name;
        category.Description = dto.Description;
    }
}