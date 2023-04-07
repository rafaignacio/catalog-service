using CatalogService.Core.Exceptions;
using CatalogService.Core.Helpers;
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

    public async Task<OneOf<long, CategoryFailureException>> Add(CategoryModel category)
    {
        var validate = _validator.Validate(category);

        if (!validate.IsValid)
            return new CategoryFailureException(validate.Errors);

        try
        {
            return await _repository.Add(category, _cancellationToken);
        }
        catch (Exception ex)
        {
            return new CategoryFailureException(ex.Message);
        }
    }

    public async Task<OneOf<Success, NotFound, CategoryFailureException>> Update(CategoryModel category)
    {

        try
        {
            var response = await GetById(category.Id);
            if (response.IsT1)
                return new NotFound();

            var retrieved = response.AsT0;

            category = MapChangedFields(category, retrieved);

            var validate = _validator.Validate(category);
            if (!validate.IsValid)
                return new CategoryFailureException(validate.Errors);

            await _repository.Update(category, _cancellationToken);
        }
        catch (Exception ex)
        {
            return new CategoryFailureException(ex.Message);
        }

        return new Success();
    }

    private CategoryModel MapChangedFields(CategoryModel newValue, CategoryModel existent) =>
        new(existent.Id,
            ComparerHelper.CompareField(newValue.Name, existent.Name),
            ComparerHelper.CompareField(newValue.Image!, existent.Image!),
            ComparerHelper.CompareField(newValue.Parent!, existent.Parent!));

    public async Task<OneOf<Success, NotFound, CategoryFailureException>> Delete(long id)
    {
        try
        {
            var response = await GetById(id);
            if (response.IsT1)
                return new NotFound();

            await _repository.Delete(id, _cancellationToken);
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

    public async Task<OneOf<CategoryModel, None>> GetById(long id)
    {
        var response = await _repository.GetById(id, _cancellationToken);

        if (response == null)
            return new None();

        return response;
    }
}