namespace InventoryManagement.API.Middlewares;

public record ErrorResponse(int StatusCode, string Message);