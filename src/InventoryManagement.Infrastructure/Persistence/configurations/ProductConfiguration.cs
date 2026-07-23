using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(product => product.Id);

        builder.Property(product => product.Name).IsRequired().HasMaxLength(150);

        builder.Property(product => product.Description).HasMaxLength(1000);

        builder.Property(product => product.Price).HasPrecision(18, 2);

        builder.HasOne(product => product.Category).WithMany(category => category.Products).HasForeignKey(product => product.CategoryId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(product => product.Name);

        builder.HasQueryFilter(product => !product.IsDeleted);
    }
}