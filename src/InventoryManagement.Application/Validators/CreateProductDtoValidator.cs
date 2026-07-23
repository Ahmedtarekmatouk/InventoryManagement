using FluentValidation;
using InventoryManagement.Application.DTOs.Products;

namespace InventoryManagement.Application.Validators;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(product => product.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(150);

        RuleFor(product => product.Description)
            .MaximumLength(1000);

        RuleFor(product => product.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(product => product.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative.");

        RuleFor(product => product.CategoryId)
            .GreaterThan(0).WithMessage("A valid category must be selected.");
    }
}