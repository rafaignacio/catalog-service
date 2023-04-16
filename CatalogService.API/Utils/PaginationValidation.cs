using CatalogService.API.Exceptions;
using FluentValidation.Results;
using OneOf.Types;
using OneOf;

namespace CatalogService.API.Utils;

public static class PaginationValidation
{
    public static OneOf<Success, PaginationFailureException> ValidatePaginationFields(ushort page, int pageSize)
    {
        var exception = new List<ValidationFailure>();

        if (page == 0)
            exception.Add(new ValidationFailure("page", "Field 'page' should be 1 or higher."));

        if (pageSize == 0)
            exception.Add(new ValidationFailure("pageSize", "Field 'pageSize' should be 1 or higher."));

        if (exception.Count > 0)
            return new PaginationFailureException(exception);

        return new Success();
    }
}
