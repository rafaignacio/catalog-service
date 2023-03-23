using CatalogService.Core.Interfaces;
using CatalogService.Core.Models;
using CatalogService.Infrastructure.Configurations;
using System.Collections.ObjectModel;

namespace CatalogService.Infrastructure.Repositories;

public class ItemRepository : BaseRepository, IItemRepository
{
    private static readonly ReadOnlyCollection<ItemModel> EmptyList = new List<ItemModel>().AsReadOnly();

    public ItemRepository(CatalogDatabaseConfiguration databaseConfiguration)
    {
        ConnectionString = databaseConfiguration.ConnectionString;
    }

    public async Task Add(ItemModel model, CancellationToken token = default) =>
        await ExecuteAsync(@"
            INSERT INTO Item ( Name, Description, Image, Category, Price, Amount )
            VALUES ( @Name, @Description, @Image, @Category, @Price, @Amount );", model, token);

    public async Task Delete(long id, CancellationToken token = default) =>
        await ExecuteAsync("DELETE FROM Item WHERE Id = @Id", new { Id = id }, token);

    public async Task<IReadOnlyCollection<ItemModel>> GetAll(CancellationToken token = default)
    {
        var list = await QueryAsync<ItemModel>("SELECT Id, Name, Description, Image, Category, Price, Amount FROM Item", token: token);

        if (list != null)
        {
            return new ReadOnlyCollection<ItemModel>(list.ToList());
        }

        return EmptyList;
    }

    public async Task<ItemModel?> GetByName(string name, CancellationToken token = default) =>
        (await QueryAsync<ItemModel>("SELECT Id, Name, Description, Image, Category, Price, Amount FROM Item WHERE Name = @Name", new { Name = name }, token: token))?.FirstOrDefault();

    public async Task Update(ItemModel model, CancellationToken token = default) =>
        await ExecuteAsync(@"
            UPDATE Item 
            SET Name = @Name, Description = @Description, Image = @Image, Category = @Category, Price = @Price, Amount = @Amount
            WHERE Id = @Id;", model, token);
}
