using FluentValidation;
using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Application.Interfaces.Services;
using InventoryManagement.Application.Services;
using InventoryManagement.Infrastructure.Persistence;
using InventoryManagement.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace InventoryManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure( this IServiceCollection services,IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        return services;
    }
}