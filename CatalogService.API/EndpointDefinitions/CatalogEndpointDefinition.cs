using CatalogService.API.Utils;
using CatalogService.Core.Entities;
using CatalogService.Core.Interfaces;
using CatalogService.Core.Models;
using CatalogService.Core.Validators;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.API.EndpointDefinitions;

public static class CatalogEndpointDefinition
{
    public static void RegisterEndpoints(WebApplication app)
    {
        app.MapGet("/categories", ListCategories);
        app.MapPost("/categories", AddCategory);

        app.MapGet("/items", ListItems);
    }

    private static async Task<IResult> AddCategory([FromBody] CategoryModel model, 
        [FromServices] ICategoryRepository repository, 
        CancellationToken token = default)
    {
        var category = new Category(new CategoryValidator(), repository, token);
        var result = await category.Add(model);

        var location = $"/categories/{model.Name}";

        return result.Match(
            _ => Results.Created(location, new
                {
                    Links = new[]
                    {
                        new { 
                            Rel = "update",
                            Verb = "PATCH",
                            Href = location
                        },
                        new {
                            Rel = "delete",
                            Verb = "DELETE",
                            Href = location
                        },
                    }
                }),
            exception => exception.Errors?.Count() > 0 ? 
                Results.BadRequest(exception) : 
                Results.Problem(exception.Message, statusCode: 500));
    }

    private static async Task<IResult> ListCategories([FromServices]ICategoryRepository repository,
        [FromQuery(Name ="page")] ushort page = 1,
        [FromQuery(Name ="pageSize")] ushort pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var paginationValidation = PaginationValidation.ValidatePaginationFields(page, pageSize);

        if (paginationValidation.IsT1)
            return Results.BadRequest(paginationValidation.AsT1);

        var catalog = new Category(new CategoryValidator(), repository, cancellationToken);
        var response = await catalog.GetAll();
        
        return response.Match(
            list => Results.Ok(list),
            _ => Results.Ok(
                Array.Empty<CategoryModel>()));
    }

    private static async Task<IResult> ListItems([FromServices] IItemRepository repository,
        [FromQuery(Name = "categoryId")] string? categoryId = null,
        [FromQuery(Name = "page")] ushort page = 1,
        [FromQuery(Name = "pageSize")] ushort pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var paginationValidation = PaginationValidation.ValidatePaginationFields(page, pageSize);

        if (paginationValidation.IsT1)
            return Results.BadRequest(paginationValidation.AsT1);

        var item = new Item(new ItemValidator(), repository, cancellationToken);
        var response = await item.GetAll(categoryId, page, page);

        return response.Match(
            list => Results.Ok(list),
            _ => Results.Ok(
                Array.Empty<ItemModel>()));
    }
}
