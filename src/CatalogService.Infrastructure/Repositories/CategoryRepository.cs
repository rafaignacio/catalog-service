using CatalogService.Core.Interfaces;
using CatalogService.Core.Models;
using CatalogService.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;

namespace CatalogService.Infrastructure.Repositories;

public class CategoryRepository : BaseRepository, ICategoryRepository
{
    private static readonly ReadOnlyCollection<CategoryModel> EmptyList = new List<CategoryModel>().AsReadOnly();

    public CategoryRepository(IOptions<CatalogDatabaseConfiguration> databaseConfiguration)
    {
        ConnectionString = databaseConfiguration.Value.ConnectionString;
    }

    public async Task<long> Add(CategoryModel model, CancellationToken token = default) => 
        await ExecuteScalarAsync<long>(@"
            INSERT INTO Category ( Name, Image, Parent )
            VALUES ( @Name, @Image, @Parent );
            SELECT last_insert_rowid();", model, token);

    public async Task Delete(long id, CancellationToken token = default) => 
        await ExecuteAsync("DELETE FROM Category WHERE Id = @Id", new { Id = id}, token);

    public async Task<IReadOnlyCollection<CategoryModel>> GetAll(CancellationToken token = default)
    {
        var list = await QueryAsync<CategoryModel>("SELECT Id, Name, Image, Parent FROM Category", token: token);

        if (list != null)
        {
            return new ReadOnlyCollection<CategoryModel>(list.ToList());
        }

        return EmptyList;
    }

    public async Task<CategoryModel?> GetByName(string name, CancellationToken token = default) =>
        (await QueryAsync<CategoryModel>("SELECT Id, Name, Image, Parent FROM Category WHERE Name = @Name", new { Name = name }, token: token))?.FirstOrDefault();

    public async Task<CategoryModel?> GetById(long id, CancellationToken token = default) =>
        (await QueryAsync<CategoryModel>("SELECT Id, Name, Image, Parent FROM Category WHERE Id = @Id", new { Id = id }, token: token))?.FirstOrDefault();

    public async Task Update(CategoryModel model, CancellationToken token = default) =>
        await ExecuteAsync(@"
            UPDATE Category 
            SET Name = @Name, Image = @Image, Parent = @Parent
            WHERE Id = @Id;", model, token);
}
