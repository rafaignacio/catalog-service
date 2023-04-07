using CatalogService.Core.Exceptions;
using CatalogService.Core.Helpers;
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

    public async Task<OneOf<long, ItemFailureException>> Add(ItemModel item)
    {
        var validate = _validator.Validate(item);

        if (!validate.IsValid)
            return new ItemFailureException(validate.Errors);

        try
        {
            return await _repository.Add(item, _cancellationToken);
        }
        catch (Exception ex)
        {
            return new ItemFailureException(ex.Message);
        }
    }

    public async Task<OneOf<Success, NotFound, ItemFailureException>> Update(ItemModel item)
    {
        try
        {
            var response = await GetById(item.Id);
            var (foundItem, itemReturned) = WasItemFound(response);

            if (!foundItem)
                return new NotFound();

            item = MapChangedFields(item, itemReturned!);

            var validate = _validator.Validate(item);

            if (!validate.IsValid)
                return new ItemFailureException(validate.Errors);

            item = item with
            {
                Description = string.IsNullOrEmpty(item.Description) ? null : item.Description,
                Image = string.IsNullOrEmpty(item.Image) ? null : item.Image,
            };

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

    private ItemModel MapChangedFields(ItemModel newValue, ItemModel existent) =>
        new(existent.Id,
            ComparerHelper.CompareField(newValue.Name, existent.Name),
            ComparerHelper.CompareField(newValue.Description, existent.Description),
            ComparerHelper.CompareField(newValue.Image, existent.Image),
            ComparerHelper.CompareField(newValue.Category, existent.Category),
            ComparerHelper.CompareField(newValue.Price, existent.Price),
            ComparerHelper.CompareField(newValue.Amount, existent.Amount));

    private static (bool, ItemModel?) WasItemFound(OneOf<ItemModel, None> response) =>
        response.Match<(bool,ItemModel?)>(
            item => (true, item),
            _ => (false, null));

    public async Task<OneOf<Success, NotFound, ItemFailureException>> Delete(long id)
    {
        try
        {
            var response = await GetById(id);
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

    public async Task<OneOf<ItemModel, None>> GetById(long id)
    {
        var response = await _repository.GetById(id, _cancellationToken);

        if (response == null)
            return new None();

        return response;
    }
}