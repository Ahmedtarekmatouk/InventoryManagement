using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Persistence;

public static class ApplicationDbContextSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Categories.AnyAsync())
        {
            return;
        }

        var categories = new List<Category>
        {
            new() { Name = "Electronics", Description = "Phones, laptops and accessories" },
            new() { Name = "Furniture", Description = "Office and home furniture" },
            new() { Name = "Stationery", Description = "Office supplies and paper products" }
        };

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();

        var products = new List<Product>
        {
            new() { Name = "Wireless Mouse", Description = "Ergonomic wireless mouse", Price = 25.50m, Quantity = 120, CategoryId = categories[0].Id },
            new() { Name = "Mechanical Keyboard", Description = "RGB mechanical keyboard", Price = 89.99m, Quantity = 45, CategoryId = categories[0].Id },
            new() { Name = "27 Inch Monitor", Description = "QHD display", Price = 249.00m, Quantity = 30, CategoryId = categories[0].Id },
            new() { Name = "Office Chair", Description = "Adjustable ergonomic chair", Price = 180.00m, Quantity = 18, CategoryId = categories[1].Id },
            new() { Name = "Standing Desk", Description = "Electric height adjustable desk", Price = 420.00m, Quantity = 12, CategoryId = categories[1].Id },
            new() { Name = "Notebook A5", Description = "Hardcover ruled notebook", Price = 4.75m, Quantity = 500, CategoryId = categories[2].Id },
            new() { Name = "Gel Pen Pack", Description = "Pack of 10 gel pens", Price = 6.20m, Quantity = 300, CategoryId = categories[2].Id }
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }
}