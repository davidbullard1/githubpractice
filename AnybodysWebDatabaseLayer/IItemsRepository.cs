using AnybodysWebModels;

namespace AnybodysWebDatabaseLayer;

public interface IItemsReadOnlyRepository
{
    Task<List<Item>> GetAllAsync();
    Task<Item?> GetAsync(int id);
    Task<bool> ExistsAsync(int id);
}

public interface IItemsWriteOnlyRepository
{
    Task<int> AddOrUpdateAsync(Item item);
    Task<int> DeleteAsync(Item item);
    Task<int> DeleteAsync(int id);
}

public interface IItemsRepository : IItemsReadOnlyRepository, IItemsWriteOnlyRepository
{

}
