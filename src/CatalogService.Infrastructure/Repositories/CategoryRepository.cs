using CatalogService.Core.Interfaces;
using CatalogService.Core.Models;
using CatalogService.Infrastructure.Configurations;
using System.Collections.ObjectModel;

namespace CatalogService.Infrastructure.Repositories;

public class CategoryRepository : BaseRepository, ICategoryRepository
{
    private static readonly ReadOnlyCollection<CategoryModel> EmptyList = new List<CategoryModel>().AsReadOnly();

    public CategoryRepository(CatalogDatabaseConfiguration databaseConfiguration)
    {
        ConnectionString = databaseConfiguration.ConnectionString;
    }

    public async Task Add(CategoryModel model, CancellationToken token = default) => 
        await ExecuteAsync(@"
            INSERT INTO Category ( Name, Image, Parent )
            VALUES ( @Name, @Image, @Parent );", new { model.Name, model.Image, model.Parent  }, token);

    public async Task Delete(string name, CancellationToken token = default) => 
        await ExecuteAsync("DELETE FROM Category WHERE Name = @Name", new { Name = name }, token);

    public async Task<IReadOnlyCollection<CategoryModel>> GetAll(CancellationToken token = default)
    {
        var list = await QueryAsync<CategoryModel>("SELECT Name, Image, Parent FROM Category", token: token);

        if (list != null)
        {
            return new ReadOnlyCollection<CategoryModel>(list.ToList());
        }

        return EmptyList;
    }

    public async Task<CategoryModel?> GetByName(string name, CancellationToken token = default) =>
        (await QueryAsync<CategoryModel>("SELECT Name, Image, Parent FROM Category WHERE Name = @Name", new { Name = name }, token: token))?.FirstOrDefault();

    public async Task Update(CategoryModel model, CancellationToken token = default) =>
        await ExecuteAsync(@"
            UPDATE Category 
            SET Image = @Image, Parent = @Parent
            WHERE Name = @Name;", model, token);
}
