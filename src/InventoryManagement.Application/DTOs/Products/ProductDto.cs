namespace InventoryManagement.Application.DTOs.Products;

public record ProductDto(int Id,string Name,string? Description,decimal Price,int Quantity,int CategoryId,string CategoryName,DateTime CreatedAt,DateTime? UpdatedAt);