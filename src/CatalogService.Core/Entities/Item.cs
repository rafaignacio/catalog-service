using CatalogService.Core.Exceptions;
using CatalogService.Core.Interfaces;
using CatalogService.Core.Models;
using FluentValidation;
using OneOf;
using OneOf.Types;

namespace CatalogService.Core.Entities;

public class Item
{
    private readonly IValidator<ItemDetailsModel> _validator;
    private readonly IItemRepository _repository;
    private readonly CancellationToken _cancellationToken;

    public Item(IValidator<ItemDetailsModel> validator, IItemRepository repository, CancellationToken cancellationToken = default)
    {
        _validator = validator;
        _repository = repository;
        _cancellationToken = cancellationToken;
    }

    public async Task<OneOf<Success, ItemFailureException>> Add(ItemDetailsModel item)
    {
        var validate = _validator.Validate(item);

        if (!validate.IsValid)
            return new ItemFailureException(validate.Errors);

        try
        {
            await _repository.Add(item, _cancellationToken);
        }
        catch (Exception ex)
        {
            return new ItemFailureException(ex.Message);
        }

        return new Success();
    }

    public async Task<OneOf<Success, NotFound, ItemFailureException>> Update(ItemDetailsModel category)
    {
        var validate = _validator.Validate(category);

        if (!validate.IsValid)
            return new ItemFailureException(validate.Errors);

        try
        {
            var response = await GetByName(category.Name);
            if (response.IsT1)
                return new NotFound();

            await _repository.Update(category, _cancellationToken);
        }
        catch (Exception ex)
        {
            return new ItemFailureException(ex.Message);
        }

        return new Success();
    }

    public async Task<OneOf<Success, NotFound, ItemFailureException>> Delete(string name)
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
            return new ItemFailureException(ex.Message);
        }

        return new Success();
    }

    public async Task<OneOf<IReadOnlyCollection<ItemDetailsModel>, None>> GetAll()
    {
        var response = await _repository.GetAll(_cancellationToken);

        if (response?.Count == 0)
            return new None();

        return response!.ToList().AsReadOnly();
    }

    public async Task<OneOf<ItemDetailsModel, None>> GetByName(string name)
    {
        var response = await _repository.GetByName(name, _cancellationToken);

        if (response == null)
            return new None();

        return response;
    }
}