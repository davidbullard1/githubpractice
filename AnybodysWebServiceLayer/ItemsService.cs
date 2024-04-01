using AnybodysWebDatabaseLayer;
using AnybodysWebModels;

namespace AnybodysWebServiceLayer;

public class ItemsService : IItemsService
{
    private IItemsRepository _itemsRepository;

    public ItemsService(IItemsRepository itemsRepository)
    {
        _itemsRepository = itemsRepository;
    }

    public Task<List<Item>> GetAllAsync()
    {
        return _itemsRepository.GetAllAsync();
    }

    public Task<Item?> GetAsync(int id)
    {
        return _itemsRepository.GetAsync(id);
    }

    public Task<int> AddOrUpdateAsync(Item item)
    {
        return _itemsRepository.AddOrUpdateAsync(item);
    }

    public Task<int> DeleteAsync(Item item)
    {
        return _itemsRepository.DeleteAsync(item);
    }

    public Task<int> DeleteAsync(int id)
    {
        return _itemsRepository.DeleteAsync(id);
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _itemsRepository.ExistsAsync(id);
    }
}
