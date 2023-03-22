using FluentValidation.Results;

namespace CatalogService.Core.Exceptions;

public class ItemFailureException : Exception
{
    public IEnumerable<ValidationFailure>? Errors { get; private set; }
    public ItemFailureException(IEnumerable<ValidationFailure> errors)
    {
        Errors = errors;
    }

    public ItemFailureException(string message) : base(message)
    {

    }
}
