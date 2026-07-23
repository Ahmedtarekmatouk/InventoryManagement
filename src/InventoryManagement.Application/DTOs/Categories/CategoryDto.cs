namespace InventoryManagement.Application.DTOs.Categories;

public record CategoryDto(int Id,string Name,string? Description,int ProductCount,DateTime CreatedAt);