using FluentValidation.Results;

namespace CatalogService.API.Exceptions;

public class PaginationFailureException : Exception
{
    public IEnumerable<ValidationFailure>? Errors { get; private set; }
    public PaginationFailureException(IEnumerable<ValidationFailure> errors)
    {
        Errors = errors;
    }

    public PaginationFailureException(string message) : base(message)
    {

    }
}
