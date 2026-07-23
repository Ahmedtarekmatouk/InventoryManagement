using FluentValidation;
using InventoryManagement.Application.DTOs.Categories;

namespace InventoryManagement.Application.Validators;

public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
{
    public UpdateCategoryDtoValidator()
    {
        RuleFor(category => category.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(100);

        RuleFor(category => category.Description)
            .MaximumLength(500);
    }
}