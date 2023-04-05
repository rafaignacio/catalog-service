using CatalogService.Core.Exceptions;
using CatalogService.Core.Interfaces;
using CatalogService.Core.Models;
using FluentValidation;
using OneOf;
using OneOf.Types;

namespace CatalogService.Core.Entities;

public class Item
{
    private readonly IValidator<ItemModel> _validator;
    private readonly IItemRepository _repository;
    private readonly CancellationToken _cancellationToken;

    public Item(IValidator<ItemModel> validator, IItemRepository repository, CancellationToken cancellationToken = default)
    {
        _validator = validator;
        _repository = repository;
        _cancellationToken = cancellationToken;
    }

    public async Task<OneOf<Success, ItemFailureException>> Add(ItemModel item)
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

    public async Task<OneOf<Success, NotFound, ItemFailureException>> Update(ItemModel item)
    {
        var validate = _validator.Validate(item);

        if (!validate.IsValid)
            return new ItemFailureException(validate.Errors);

        try
        {
            var response = await GetByName(item.Name);
            var (foundItem, itemReturned) = WasItemFound(response);

            if (!foundItem)
                return new NotFound();

            await _repository.Update(
                item with { Id = itemReturned!.Id }, 
                _cancellationToken);
        }
        catch (Exception ex)
        {
            return new ItemFailureException(ex.Message);
        }

        return new Success();
    }

    private static (bool, ItemModel?) WasItemFound(OneOf<ItemModel, None> response) =>
        response.Match<(bool,ItemModel?)>(
            item => (true, item),
            _ => (false, null));

    public async Task<OneOf<Success, NotFound, ItemFailureException>> Delete(string name)
    {
        try
        {
            var response = await GetByName(name);
            var (foundItem, item) = WasItemFound(response);

            if (!foundItem)
                return new NotFound();

            await _repository.Delete(item!.Id, _cancellationToken);
        }
        catch (Exception ex)
        {
            return new ItemFailureException(ex.Message);
        }

        return new Success();
    }

    public async Task<OneOf<IReadOnlyCollection<ItemModel>, None>> GetAll(string? categoryId = null, ushort page = 0, ushort pageSize = 20)
    {
        var response = await _repository.GetAll(categoryId, page, pageSize, _cancellationToken);

        if (response?.Count == 0)
            return new None();

        return response!.ToList().AsReadOnly();
    }

    public async Task<OneOf<ItemModel, None>> GetByName(string name)
    {
        var response = await _repository.GetByName(name, _cancellationToken);

        if (response == null)
            return new None();

        return response;
    }
}