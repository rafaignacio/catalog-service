using CatalogService.Core.Models;

namespace CatalogService.Core.Interfaces;

public interface IItemRepository
{
    Task<ItemModel?> GetByName(string name, CancellationToken token = default);
    Task<ItemModel?> GetById(long id, CancellationToken token = default);
    Task<IReadOnlyCollection<ItemModel>> GetAll(string? categoryId = null, ushort page = 0, ushort pageSize = 20, CancellationToken token = default);
    Task<long> Add(ItemModel model, CancellationToken token = default);
    Task Delete(long id, CancellationToken token = default);
    Task Update(ItemModel model, CancellationToken token = default);
}
