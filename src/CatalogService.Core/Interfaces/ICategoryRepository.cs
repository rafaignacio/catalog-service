using CatalogService.Core.Models;

namespace CatalogService.Core.Interfaces;

public interface ICategoryRepository {
    Task<CategoryModel?> GetByName(string name, CancellationToken token = default);
    Task<IReadOnlyCollection<CategoryModel>> GetAll(CancellationToken token = default);
    Task Add(CategoryModel model, CancellationToken token = default);
    Task Delete(string name, CancellationToken token = default);
    Task Update(CategoryModel model, CancellationToken token = default);
}