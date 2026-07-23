using System.Reflection;
using FluentValidation;
using InventoryManagement.Application.Interfaces.Services;
using InventoryManagement.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}