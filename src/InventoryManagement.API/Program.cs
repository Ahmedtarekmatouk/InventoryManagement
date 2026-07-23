using InventoryManagement.Infrastructure;
using InventoryManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Application;
using InventoryManagement.Application.Interfaces.Services;
using InventoryManagement.Application.Services;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

await ApplyMigrationsAndSeedAsync(app);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

static async Task ApplyMigrationsAndSeedAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    await context.Database.MigrateAsync();
    await ApplicationDbContextSeeder.SeedAsync(context);
}
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();

        return services;
    }
}