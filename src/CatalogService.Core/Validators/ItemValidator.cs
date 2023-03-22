using CatalogService.Core.Models;
using FluentValidation;

namespace CatalogService.Core.Validators;

public class ItemValidator : AbstractValidator<ItemModel>
{
    public ItemValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(50)
            .NotEmpty();

        RuleFor(x => x.Category)
            .MaximumLength(50) 
            .NotEmpty();

        RuleFor(x => x.Price)
            .GreaterThan(0);

        RuleFor(x => x.Amount)
            .GreaterThan(0);
    }
}
