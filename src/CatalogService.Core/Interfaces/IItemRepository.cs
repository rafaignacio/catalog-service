using CatalogService.Core.Models;

namespace CatalogService.Core.Interfaces;

public interface IItemRepository
{
    Task<ItemDetailsModel?> GetByName(string name, CancellationToken token = default);
    Task<IReadOnlyCollection<ItemDetailsModel>> GetAll(CancellationToken token = default);
    Task Add(ItemDetailsModel model, CancellationToken token = default);
    Task Delete(string name, CancellationToken token = default);
    Task Update(ItemDetailsModel model, CancellationToken token = default);
}
