using FluentValidation.Results;

namespace CatalogService.Core.Exceptions;

public class CategoryFailureException : Exception
{
    public IEnumerable<ValidationFailure>? Errors { get; private set; }
    public CategoryFailureException(IEnumerable<ValidationFailure> errors) { 
        Errors = errors;
    }

    public CategoryFailureException(string message) : base(message)
    {

    }
}