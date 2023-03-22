using CatalogService.Core.Models;
using FluentValidation;

namespace CatalogService.Core.Validators;

public class CategoryValidator : AbstractValidator<CategoryModel> {
    public CategoryValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(50)
            .NotEmpty();
    }
}