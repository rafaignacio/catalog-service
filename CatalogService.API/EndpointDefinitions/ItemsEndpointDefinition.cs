using CatalogService.API.Core;
using CatalogService.API.Models;
using CatalogService.API.Utils;
using CatalogService.Core.Entities;
using CatalogService.Core.Interfaces;
using CatalogService.Core.Models;
using CatalogService.Core.Validators;
using CatalogService.Infrastructure.EventHandlers;
using CatalogService.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.API.EndpointDefinitions;

public class ItemsEndpointDefinition : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.MapGet("/items", ListItems);
        app.MapPost("/items", AddItem);
        app.MapPut("/items/{id}", UpdateItem);
        app.MapDelete("/items/{id}", DeleteItem);
    }

    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IItemRepository, ItemRepository>();
        services.AddSingleton<IValidator<ItemModel>, ItemValidator>();
        services.AddDomainEventHandlers();
    }

    private static async Task<IResult> ListItems([FromServices] IItemRepository repository,
        [FromServices] IValidator<ItemModel> validator,
        [FromQuery(Name = "categoryId")] string? categoryId = null,
        [FromQuery(Name = "page")] ushort page = 1,
        [FromQuery(Name = "pageSize")] ushort pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var paginationValidation = PaginationValidation.ValidatePaginationFields(page, pageSize);

        if (paginationValidation.IsT1)
            return Results.BadRequest(paginationValidation.AsT1);

        var item = new Item(validator, repository, cancellationToken);
        var response = await item.GetAll(categoryId, --page, pageSize);

        return response.Match(
            list => Results.Ok(list.Select( i => new ItemDetailResponse(i))),
            _ => Results.Ok(
                Array.Empty<ItemModel>()));
    }

    private static async Task<IResult> AddItem([FromBody] AddItemModel model,
        [FromServices] IItemRepository repository,
        [FromServices] IValidator<ItemModel> validator,
        [FromServices] IEventDispatcher eventDispatcher,
        CancellationToken token = default)
    {
        var item = new Item(validator, repository, token);
        var result = await item.Add(model.ToItemModel());

        return result.Match(
            id => Results.Created($"/items/{id}", new ItemDetailResponse(id, model.ToItemModel())),
            exception => exception.Errors?.Count() > 0 ?
                Results.BadRequest(exception) :
                Results.Problem(exception.Message, statusCode: 500));
    }

    private static async Task<IResult> UpdateItem([FromRoute] long id, [FromBody] UpdateItemModel model,
        [FromServices] IItemRepository repository,
        [FromServices] IValidator<ItemModel> validator,
        [FromServices] IEventDispatcher eventDispatcher,
        CancellationToken token = default)
    {
        var item = new Item(validator, repository, token);
        var result = await item.Update(
            new(id, model.Name, model.Description, model.Image, model.Category, model.Price, model.Amount));

        await item.DispatchEvents(eventDispatcher);

        return result.Match(
            _ => Results.NoContent(),
            _ => Results.NotFound(),
            exception => exception.Errors?.Count() > 0 ?
                Results.BadRequest(exception) :
                Results.Problem(exception.Message, statusCode: 500));
    }

    private static async Task<IResult> DeleteItem([FromRoute] long id,
        [FromServices] IValidator<ItemModel> validator,
        [FromServices] IItemRepository repository,
        [FromServices] IEventDispatcher eventDispatcher,
        CancellationToken token = default)
    {
        var item = new Item(validator, repository, token);
        var result = await item.Delete(id);

        await item.DispatchEvents(eventDispatcher);

        return result.Match(
            _ => Results.NoContent(),
            _ => Results.NotFound(),
            exception => exception.Errors?.Count() > 0 ?
                Results.BadRequest(exception) :
                Results.Problem(exception.Message, statusCode: 500));
    }
}
