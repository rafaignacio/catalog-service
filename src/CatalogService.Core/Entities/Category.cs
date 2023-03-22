using CatalogService.Core.Exceptions;
using CatalogService.Core.Interfaces;
using CatalogService.Core.Models;
using FluentValidation;
using OneOf;
using OneOf.Types;

namespace CatalogService.Core.Entities;

public class Category {
    private readonly IValidator<CategoryModel> _validator;
    private readonly ICategoryRepository _repository;
    private readonly CancellationToken _cancellationToken;

    public Category(IValidator<CategoryModel> validator, ICategoryRepository repository, CancellationToken cancellationToken = default)
    {
        _validator = validator;
        _repository = repository;
        _cancellationToken = cancellationToken;
    }

    public async Task<OneOf<Success, CategoryFailureException>> Add(CategoryModel category)
    {
        var validate = _validator.Validate(category);

        if (!validate.IsValid)
            return new CategoryFailureException(validate.Errors);

        try
        {
            await _repository.Add(category, _cancellationToken);
        }
        catch (Exception ex)
        {
            return new CategoryFailureException(ex.Message);
        }

        return new Success();
    }

    public async Task<OneOf<Success, NotFound, CategoryFailureException>> Update(CategoryModel category)
    {
        var validate = _validator.Validate(category);

        if (!validate.IsValid)
            return new CategoryFailureException(validate.Errors);

        try
        {
            var response = await GetByName(category.Name);
            if (response.IsT1)
                return new NotFound();

            await _repository.Update(category, _cancellationToken);
        }
        catch (Exception ex)
        {
            return new CategoryFailureException(ex.Message);
        }

        return new Success();
    }

    public async Task<OneOf<Success, NotFound, CategoryFailureException>> Delete(string name)
    {
        try
        {
            var response = await GetByName(name);
            if (response.IsT1)
                return new NotFound();

            await _repository.Delete(name, _cancellationToken);
        }
        catch (Exception ex)
        {
            return new CategoryFailureException(ex.Message);
        }

        return new Success();
    }

    public async Task<OneOf<IReadOnlyCollection<CategoryModel>, None>> GetAll()
    {
        var response = await _repository.GetAll(_cancellationToken);

        if (response?.Count == 0)
            return new None();

        return response!.ToList().AsReadOnly();
    }

    public async Task<OneOf<CategoryModel, None>> GetByName(string name)
    {
        var response = await _repository.GetByName(name, _cancellationToken);

        if (response == null) 
            return new None();

        return response;
    }
}