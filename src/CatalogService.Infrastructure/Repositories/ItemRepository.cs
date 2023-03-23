using CatalogService.Core.Interfaces;
using CatalogService.Core.Models;
using CatalogService.Infrastructure.Configurations;
using System.Collections.ObjectModel;

namespace CatalogService.Infrastructure.Repositories;

public class ItemRepository : BaseRepository, IItemRepository
{
    private static readonly ReadOnlyCollection<ItemDetailsModel> EmptyList = new List<ItemDetailsModel>().AsReadOnly();

    public ItemRepository(CatalogDatabaseConfiguration databaseConfiguration)
    {
        ConnectionString = databaseConfiguration.ConnectionString;
    }

    public async Task Add(ItemDetailsModel model, CancellationToken token = default) =>
        await ExecuteAsync(@"
            INSERT INTO Item ( Name, Description, Image, Category, Price, Amount )
            VALUES ( @Name, @Description, @Image, @Category, @Price, @Amount );", model, token);

    public async Task Delete(string name, CancellationToken token = default) =>
        await ExecuteAsync("DELETE FROM Item WHERE Name = @Name", new { Name = name }, token);

    public async Task<IReadOnlyCollection<ItemDetailsModel>> GetAll(CancellationToken token = default)
    {
        var list = await QueryAsync<ItemDetailsModel>("SELECT Name, Description, Image, Category, Price, Amount FROM Item", token: token);

        if (list != null)
        {
            return new ReadOnlyCollection<ItemDetailsModel>(list.ToList());
        }

        return EmptyList;
    }

    public async Task<ItemDetailsModel?> GetByName(string name, CancellationToken token = default) =>
        (await QueryAsync<ItemDetailsModel>("SELECT Name, Description, Image, Category, Price, Amount FROM Item WHERE Name = @Name", new { Name = name }, token: token))?.FirstOrDefault();

    public async Task Update(ItemDetailsModel model, CancellationToken token = default) =>
        await ExecuteAsync(@"
            UPDATE Item 
            SET Description = @Description, Image = @Image, Category = @Category, Price = @Price, Amount = @Amount
            WHERE Name = @Name;", model, token);
}
