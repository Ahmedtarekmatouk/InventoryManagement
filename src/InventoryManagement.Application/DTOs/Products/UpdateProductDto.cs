namespace InventoryManagement.Application.DTOs.Products;

public record UpdateProductDto(string Name,string? Description, decimal Price,int Quantity,int CategoryId);