using FluentValidation.AspNetCore;
using InventoryManagement.API.Middlewares;
using InventoryManagement.Infrastructure;
using InventoryManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

const string AngularCorsPolicy = "AngularClient";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>options.AddPolicy(AngularCorsPolicy, policy => policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

await ApplyMigrationsAndSeedAsync(app);

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(AngularCorsPolicy);
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