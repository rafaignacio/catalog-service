using CatalogService.API.Core;
using CatalogService.API.Models;
using CatalogService.API.Utils;
using CatalogService.Core.Entities;
using CatalogService.Core.Interfaces;
using CatalogService.Core.Models;
using CatalogService.Core.Validators;
using CatalogService.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.API.EndpointDefinitions;

public class CategoryEndpointDefinition : IEndpointDefinition
{
    private static object GetCategoryLinks(string location) => new
        {
            Links = new[]
                {
                    new {
                        Rel = "update",
                        Verb = "PUT",
                        Href = location
                    },
                    new {
                        Rel = "delete",
                        Verb = "DELETE",
                        Href = location
                    },
                }
        };

    public void DefineEndpoints(WebApplication app)
    {
        app.MapGet("/categories", ListCategories)
            .WithName("List Categories");
        app.MapPost("/categories", AddCategory)
            .WithName("Add Categories");
        app.MapPut("/categories/{name}", UpdateCategory)
            .WithName("Update Category");
        app.MapDelete("/categories/{name}", DeleteCategory)
            .WithName("Delete Category");
    }

    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<ICategoryRepository, CategoryRepository>();
        services.AddSingleton<IValidator<CategoryModel>, CategoryValidator>();
    }

    private static async Task<IResult> AddCategory([FromBody] CategoryModel model, 
        [FromServices] ICategoryRepository repository,
        [FromServices] IValidator<CategoryModel> validator,
        CancellationToken token = default)
    {
        var category = new Category(validator, repository, token);
        var result = await category.Add(model);

        var location = $"/categories/{model.Name}";

        return result.Match(
            _ => Results.Created(location, GetCategoryLinks(location)),
            exception => exception.Errors?.Count() > 0 ? 
                Results.BadRequest(exception) : 
                Results.Problem(exception.Message, statusCode: 500));
    }

    private static async Task<IResult> UpdateCategory([FromRoute] string name, [FromBody] UpdateCategoryModel model,
        [FromServices] ICategoryRepository repository,
        [FromServices] IValidator<CategoryModel> validator,
        CancellationToken token = default)
    {
        var category = new Category(validator, repository, token);
        var result = await category.Update(
            new (name, model.Image, model.Parent));

        return result.Match(
            _ => Results.NoContent(),
            _ => Results.NotFound(),
            exception => exception.Errors?.Count() > 0 ?
                Results.BadRequest(exception) :
                Results.Problem(exception.Message, statusCode: 500));
    }

    private static async Task<IResult> DeleteCategory([FromRoute] string name,
        [FromServices] IValidator<CategoryModel> validator,
        [FromServices] ICategoryRepository repository,
        CancellationToken token = default)
    {
        var category = new Category(validator, repository, token);
        var result = await category.Delete(name);

        return result.Match(
            _ => Results.NoContent(),
            _ => Results.NotFound(),
            exception => exception.Errors?.Count() > 0 ?
                Results.BadRequest(exception) :
                Results.Problem(exception.Message, statusCode: 500));
    }

    private static async Task<IResult> ListCategories([FromServices] ICategoryRepository repository,
        [FromServices] IValidator<CategoryModel> validator,
        CancellationToken cancellationToken = default)
    {
        var catalog = new Category(validator, repository, cancellationToken);
        var response = await catalog.GetAll();
        
        return response.Match(
            list => Results.Ok(list),
            _ => Results.Ok(
                Array.Empty<CategoryModel>()));
    }

}
