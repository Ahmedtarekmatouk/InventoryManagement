using FluentValidation;
using InventoryManagement.Application.DTOs.Categories;

namespace InventoryManagement.Application.Validators;

public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryDtoValidator()
    {
        RuleFor(category => category.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(100);

        RuleFor(category => category.Description)
            .MaximumLength(500);
    }
}