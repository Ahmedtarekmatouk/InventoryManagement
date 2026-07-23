using InventoryManagement.Application.DTOs.Products;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Mappings;

public static class ProductMappingExtensions
{
    public static ProductDto ToDto(this Product product) =>
        new(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Quantity,
            product.CategoryId,
            product.Category?.Name ?? string.Empty,
            product.CreatedAt,
            product.UpdatedAt);

    public static Product ToEntity(this CreateProductDto dto) =>
        new()
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Quantity = dto.Quantity,
            CategoryId = dto.CategoryId
        };

    public static void ApplyChanges(this Product product, UpdateProductDto dto)
    {
        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Quantity = dto.Quantity;
        product.CategoryId = dto.CategoryId;
    }
}